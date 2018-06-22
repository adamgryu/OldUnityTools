using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio {

    /// <summary>
    /// An adapter for <see cref="AudioClip"/> so they can be used as a music stack asset.
    /// </summary>
    public class AudioClipMusicAsset : MusicStack.IMusicAsset {

        private AudioClip clip;

        public AudioClipMusicAsset(AudioClip clip) {
            this.clip = clip;
        }

        public int GetMusicID() {
            return clip.GetInstanceID();
        }

        public MusicStack.IMusicPlayer CreatePlayer() {
            return new AudioClipMusicPlayer(clip);
        }

        /// <summary>
        /// Creates an audio source to play the related audio clip on the music stack.
        /// </summary>
        public class AudioClipMusicPlayer : MusicStack.IMusicPlayer {

            private AudioSource source;

            public AudioClipMusicPlayer(AudioClip clip) {
                GameObject obj = new GameObject("AudioClipMusicPlayer (" + clip + ")");
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
}