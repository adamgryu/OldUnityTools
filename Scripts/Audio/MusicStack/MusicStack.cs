using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

namespace QuickUnityTools.Audio {

    /// <summary>
    /// Music on the music stack is sorted by priority and then timestamp.
    /// </summary>
    public enum MusicStackPriorty {
        Low = 0,
        Normal = 50,
        High = 100,
    }

    /// <summary>
    /// Represents a class that can register itself into the music stack. A music stack element can
    /// define the <see cref="IMusicAsset"/> it wishes to play, the volume that audio should be played
    /// at, and how the music transitions when the element takes or loses control.
    /// </summary>
    public interface IMusicStackElement {
        MusicStack.IMusicAsset GetMusicAsset();
        MusicStack.Transition GetTakeControlTransition();
        MusicStack.Transition GetReleaseControlTransition();
        float GetDesiredVolume();
        MusicStackPriorty GetPriority();
    }

    /// <summary>
    /// Manages a sorted stack of songs that should be playing for any given moment.
    /// The element on the top of the stack is the one that should be currently playing.
    /// </summary>
    [ResourceSingleton("MusicStack")]
    public class MusicStack : Singleton<MusicStack> {

        // Links
        public AudioMixerGroup mixerGroup;

        // State
        private SortedList<PrioritySortingKey, IMusicStackElement> musicStack = new SortedList<PrioritySortingKey, IMusicStackElement>();
        private Dictionary<int, MusicPlayerController> playerControllers = new Dictionary<int, MusicPlayerController>();

        // Helpers
        private IMusicStackElement currentMusic { get { return musicStackView.Count == 0 ? null : musicStackView[0]; } }
        private IList<IMusicStackElement> musicStackView;
        private Dictionary<int, MusicPlayerController>.ValueCollection musicControllersView;

        private void Awake() {
            // Avoid creating garbage.
            musicStackView = musicStack.Values;
            musicControllersView = playerControllers.Values;
        }

        /// <summary>
        /// Adds an elment to the music stack. If this becomes the top element, it will transition to these music settings.
        /// </summary>
        public PrioritySortingKey AddToMusicStack(IMusicStackElement addedMusic) {
            var prevMusic = currentMusic;

            var newKey = new PrioritySortingKey((int)addedMusic.GetPriority());
            musicStack.Add(newKey, addedMusic);

            if (currentMusic != prevMusic) {
                Asserts.AssertTrue(addedMusic == currentMusic, "Somehow we added to the music stack and yet a different music rose to the top?");
                TransitionToMusic(prevMusic, currentMusic, currentMusic.GetTakeControlTransition());
            }
            return newKey;
        }

        /// <summary>
        /// Removes an element from the music stack. If this was the top element, this will trigger a transition to the next element.
        /// </summary>
        public void RemoveFromMusicStack(PrioritySortingKey key) {
            Asserts.AssertTrue(musicStack.ContainsKey(key), "This key is not in the music stack!");
            var keyMusic = musicStack[key];
            bool wasPlayingKeyMusic = keyMusic == currentMusic;

            musicStack.Remove(key);
            if (wasPlayingKeyMusic) {
                TransitionToMusic(keyMusic, currentMusic, keyMusic.GetReleaseControlTransition());
            }
        }

        /// <summary>
        /// Transitions from one music configuration to another, using the specified transition.
        /// 
        /// MusicStack elements that share the same <see cref="IMusicAsset"/> can reuse the existing audio player
        /// when transitioning between them. This lets the music keep going.
        /// </summary>
        private void TransitionToMusic(IMusicStackElement oldElement, IMusicStackElement newElement, Transition transition) {
            // We get the music data associated with each stack element.
            var oldMusicData = GetAudioData(oldElement);
            var newMusicData = GetAudioData(newElement);

            if (oldMusicData != null) {
                playerControllers[oldMusicData.GetMusicID()].StopMusic(transition.fadeOutTime);
            }

            if (newMusicData != null) {
                var id = newMusicData.GetMusicID();
                if (playerControllers.ContainsKey(id)) {
                    // Reuse an existing player if one exists.
                    playerControllers[id].StartMusic(transition.fadeInTime, transition.fadeInDelay, newElement.GetDesiredVolume());
                } else {
                    // Create a new player 
                    var player = newMusicData.CreatePlayer();
                    player.outputAudioMixerGroup = mixerGroup;
                    var controller = new MusicPlayerController(player);
                    playerControllers.Add(id, controller);
                    controller.StartMusic(transition.fadeInTime, transition.fadeInDelay, newElement.GetDesiredVolume());
                }
            }
        }

        private void Update() {
            bool cleanUp = false;
            foreach (var controller in musicControllersView) {
                controller.ManualUpdate();
                cleanUp |= controller.canCleanUp;
            }
            if (cleanUp) {
                playerControllers.BufferedForEach(pair => pair.Value.canCleanUp, pair => {
                    pair.Value.CleanUp();
                    playerControllers.Remove(pair.Key);
                });
            }
        }

        private static IMusicAsset GetAudioData(IMusicStackElement element) {
            return element == null ? null : element.GetMusicAsset();
        }

        /// <summary>
        /// Provides an API for controling a music player that includes methods for starting, stopping, and fading the music.
        /// </summary>
        public class MusicPlayerController {

            public bool canCleanUp { get { return !isPlaying && player.volume == 0; } }

            private IMusicPlayer player;
            private float normalVolume;

            private float fadeDelayCountdown;
            private float destinationVolume;
            private float? fadeSpeed = null;

            private bool isPlaying { get { return destinationVolume > 0; } }

            public MusicPlayerController(IMusicPlayer player) {
                this.player = player;
                this.normalVolume = player.volume;
                player.volume = 0;
            }

            public void StartMusic(float fadeInTime, float fadeInDelay, float? newVolume) {
                if (newVolume.HasValue) {
                    normalVolume = newVolume.Value;
                }
                FadeTowardsVolume(normalVolume, fadeInTime, fadeInDelay);
            }

            public void StopMusic(float fadeOutTime) {
                FadeTowardsVolume(0, fadeOutTime, 0);
            }

            private void FadeTowardsVolume(float toVolume, float fadeTime, float fadeDelay) {
                fadeDelayCountdown = fadeDelay;
                destinationVolume = toVolume;
                fadeSpeed = fadeTime > 0 ? Mathf.Abs(destinationVolume - player.volume) / fadeTime : (float?)null;
            }

            public void ManualUpdate() {
                if (fadeDelayCountdown > 0) {
                    fadeDelayCountdown -= Time.unscaledDeltaTime;
                } else {
                    if (isPlaying && !player.isPlaying) {
                        player.Play();
                    }
                    if (!isPlaying && player.isPlaying && player.volume <= 0) {
                        player.Stop();
                    }
                    float volumeDelta = fadeSpeed.HasValue ? Time.unscaledDeltaTime * fadeSpeed.Value : 1000f;
                    player.volume = Mathf.MoveTowards(player.volume, destinationVolume, volumeDelta);
                }
            }

            public void CleanUp() {
                player.CleanUp();
            }
        }

        /// <summary>
        /// Represents a Unity music asset that can be played and has a unqiue ID.
        /// </summary>
        public interface IMusicAsset {

            /// <summary>
            /// Creates a Unity object that will play this music asset.
            /// </summary>
            IMusicPlayer CreatePlayer();

            /// <summary>
            /// Returns the unique music ID that the music stack uses to detect if two stack elements are playing the same music.
            /// </summary>
            int GetMusicID();
        }

        /// <summary>
        /// Represents a Unity object that plays an associated <see cref="IMusicAsset"/>.
        /// </summary>
        public interface IMusicPlayer {
            AudioMixerGroup outputAudioMixerGroup { set; }
            float volume { get; set; }
            bool isPlaying { get; }
            void Play();
            void Stop();
            void CleanUp();
        }

        /// <summary>
        /// Defines how the current music ends and the new music starts.
        /// </summary>
        [Serializable]
        public struct Transition {
            public float fadeOutTime;
            public float fadeInTime;
            public float fadeInDelay;

            public readonly static Transition INSTANT = new Transition() { };
            public readonly static Transition CROSS_FADE = new Transition() { fadeOutTime = 2f, fadeInTime = 2 };
            public static readonly Transition QUICK_OUT = new Transition() { fadeOutTime = 0.1f, fadeInDelay = 0.5f };
        }
    }
}