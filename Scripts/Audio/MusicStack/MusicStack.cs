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
    /// Manages a stack of songs that should be playing for any given moment.
    /// </summary>
    [ResourceSingleton("MusicStack")]
    public class MusicStack : Singleton<MusicStack> {

        // Links
        public AudioMixerGroup mixerGroup;

        // State
        private SortedList<PrioritySortingKey, IMusicStackElement> musicStack = new SortedList<PrioritySortingKey, IMusicStackElement>();
        private Dictionary<int, MusicStackAudioPlayerController> musicPlayerControllers = new Dictionary<int, MusicStackAudioPlayerController>();

        // Helpers
        private IMusicStackElement currentMusic { get { return musicStackView.Count == 0 ? null : musicStackView[musicStackView.Count - 1]; } }
        private IList<IMusicStackElement> musicStackView;
        private Dictionary<int, MusicStackAudioPlayerController>.ValueCollection musicControllersView;

        private void Awake() {
            // Avoid creating garbage.
            musicStackView = musicStack.Values;
            musicControllersView = musicPlayerControllers.Values;
        }

        public PrioritySortingKey AddToMusicStack(IMusicStackElement addedMusic, MusicStackPriorty priority) {
            var prevMusic = currentMusic;

            var newKey = new PrioritySortingKey((int)priority);
            musicStack.Add(newKey, addedMusic);

            if (currentMusic != prevMusic) {
                Asserts.AssertTrue(addedMusic == currentMusic, "Somehow we added to the music stack and yet a different music rose to the top?");
                TransitionToMusic(prevMusic, currentMusic, currentMusic.GetTakeControlTransition());
            }
            return newKey;
        }

        public void RemoveFromMusicStack(PrioritySortingKey key) {
            Asserts.AssertTrue(musicStack.ContainsKey(key), "This key is not in the music stack!");
            var keyMusic = musicStack[key];
            bool wasPlayingKeyMusic = keyMusic == currentMusic;

            musicStack.Remove(key);
            if (wasPlayingKeyMusic) {
                TransitionToMusic(keyMusic, currentMusic, keyMusic.GetReleaseControlTransition());
            }
        }

        private void TransitionToMusic(IMusicStackElement oldMusicElement, IMusicStackElement newMusicElement, MusicStackTransition transition) {
            var oldMusic = GetAudioData(oldMusicElement);
            var newMusic = GetAudioData(newMusicElement);
            if (oldMusic != null) {
                musicPlayerControllers[oldMusic.GetMusicID()].StopMusic(transition.fadeOutTime);
            }
            if (newMusic != null) {
                var id = newMusic.GetMusicID();
                if (musicPlayerControllers.ContainsKey(id)) {
                    // Reuse an existing player if one exists.
                    musicPlayerControllers[id].StartMusic(transition.fadeInTime, transition.fadeInDelay, newMusicElement.GetDesiredVolume());
                } else {
                    var player = newMusic.CreatePlayer();
                    player.outputAudioMixerGroup = mixerGroup;
                    var controller = new MusicStackAudioPlayerController(player);
                    musicPlayerControllers.Add(id, controller);
                    controller.StartMusic(transition.fadeInTime, transition.fadeInDelay, newMusicElement.GetDesiredVolume());
                }
            }
        }

        private void Update() {
            bool cleanUp = false;
            foreach (var controller in musicControllersView) {
                controller.Update();
                cleanUp |= controller.canCleanUp;
            }
            if (cleanUp) {
                musicPlayerControllers.BufferedForEach(pair => pair.Value.canCleanUp, pair => {
                    pair.Value.CleanUp();
                    musicPlayerControllers.Remove(pair.Key);
                });
            }
        }

        private static IMusicStackAudioData GetAudioData(IMusicStackElement element) {
            return element == null ? null : element.GetAudioData();
        }

        /// <summary>
        /// Provides an API for controling a music player that includes methods for starting, stopping, and fading the music.
        /// </summary>
        public class MusicStackAudioPlayerController {

            public bool canCleanUp { get { return !isPlaying && player.volume == 0; } }

            private IMusicStackAudioPlayer player;
            private float normalVolume;

            private float fadeDelayCountdown;
            private float destinationVolume;
            private float? fadeSpeed = null;

            private bool isPlaying { get { return destinationVolume > 0; } }

            public MusicStackAudioPlayerController(IMusicStackAudioPlayer player) {
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

            public void Update() {
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
    }

    /// <summary>
    /// Defines how the current music ends and the new music starts.
    /// </summary>
    [Serializable]
    public struct MusicStackTransition {
        public float fadeOutTime;
        public float fadeInTime;
        public float fadeInDelay;

        public readonly static MusicStackTransition INSTANT = new MusicStackTransition() { };
        public readonly static MusicStackTransition CROSS_FADE = new MusicStackTransition() { fadeOutTime = 2f, fadeInTime = 2 };
    }

    /// <summary>
    /// Represents 
    /// </summary>
    public interface IMusicStackAudioData {
        IMusicStackAudioPlayer CreatePlayer();
        int GetMusicID();
    }

    public interface IMusicStackElement {
        IMusicStackAudioData GetAudioData();
        float GetDesiredVolume();
        MusicStackTransition GetTakeControlTransition();
        MusicStackTransition GetReleaseControlTransition();
    }

    public class AudioClipMusicStackAudioData : IMusicStackAudioData {

        private AudioClip clip;

        public AudioClipMusicStackAudioData(AudioClip clip) {
            this.clip = clip;
        }

        public int GetMusicID() {
            return clip.GetInstanceID();
        }

        public IMusicStackAudioPlayer CreatePlayer() {
            return new AudioClipMusicStackAudioPlayer(clip);
        }

        public class AudioClipMusicStackAudioPlayer : IMusicStackAudioPlayer {

            private AudioSource source;

            public AudioClipMusicStackAudioPlayer(AudioClip clip) {
                GameObject obj = new GameObject("AudioSourceMusicStackAudioPlayer: " + clip);
                AudioSource source = obj.AddComponent<AudioSource>();
                source.clip = clip;
                this.source = source;
                UnityEngine.Object.DontDestroyOnLoad(obj);
            }

            public float volume {
                get { return source.volume; }
                set { source.volume = value; }
            }

            public bool isPlaying { get { return source.isPlaying; } }

            public AudioMixerGroup outputAudioMixerGroup {
                set { source.outputAudioMixerGroup = value; }
            }

            public void CleanUp() {
                GameObject.Destroy(source.gameObject);
            }

            public void Play() {
                source.Play();
            }

            public void Stop() {
                source.Stop();
            }
        }
    }

    public interface IMusicStackAudioPlayer {
        AudioMixerGroup outputAudioMixerGroup { set; }
        float volume { get; set; }
        bool isPlaying { get; }
        void Play();
        void Stop();
        void CleanUp();
    }
}