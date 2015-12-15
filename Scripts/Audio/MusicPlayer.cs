using UnityEngine;
using System.Collections;

namespace QuickUnityTools.Audio {
    public class MusicPlayer : Singleton<MusicPlayer> {

        public AudioSource currentAudio { get; private set; }
        private AudioSource fadeOutAudio = null;
        private bool isFadingOutAudioClip { get { return this.fadeOutAudio != null; } }

        public float fadeInRate = 0.25f;
        public float fadeOutRate = 0.5f;
        public float musicVolume {
            get { return this._musicVolume; }
            set {
                this._musicVolume = value;
                this.currentAudio.volume = this._musicVolume;
                this.fadeOutAudio.volume = 0;
            }
        }
        private float _musicVolume = 0.45f;

        void Update() {
            if (this.fadeOutAudio != null) {
                // If the old audio is not null, fade it out.
                if (this.fadeOutAudio.volume > 0.1f * this.musicVolume) {
                    this.fadeOutAudio.volume -= fadeOutRate * this.musicVolume * Time.deltaTime;
                } else {
                    // Destroy audio once we are done fading it out.
                    GameObject.Destroy(fadeOutAudio);
                    this.fadeOutAudio = null;
                }
            }

            if (this.currentAudio != null) {
                // If the current audio is low volume, turn it up!
                if (this.currentAudio.volume < 1 * this.musicVolume) {
                    this.currentAudio.volume += fadeInRate * this.musicVolume * Time.deltaTime;
                } else {
                    this.currentAudio.volume = 1f * this.musicVolume;
                }
            }
        }

        /// <summary>
        /// Pass null to fade out current music.
        /// </summary>
        public void TransitionToMusic(AudioClip newMusic) {
            // Don't transition if the music is the same.
            if (this.currentAudio != null && this.currentAudio.clip == newMusic) {
                return;
            }

            // Always fade out the current music.
            bool musicWasPlayingBeforeTransition = this.currentAudio != null;
            if (this.currentAudio != null) {
                this.FadeOutCurrentMusic();
            }

            // Don't play any new music if none is specified.
            if (newMusic == null) {
                return;
            }

            // Play the new audio.
            this.currentAudio = this.gameObject.AddComponent<AudioSource>();
            this.currentAudio.clip = newMusic;
            this.currentAudio.Play();
            this.currentAudio.loop = true;

            if (musicWasPlayingBeforeTransition) {
                // Fade in the new music.
                this.currentAudio.volume = 0.1f * this.musicVolume;
            }
        }

        public void StopMusic() {
            if (this.currentAudio != null) {
                GameObject.Destroy(this.currentAudio);
                this.currentAudio = null;
            }
            if (this.isFadingOutAudioClip) {
                // Destroy the previous audio clip that hasn't finished fading out.
                GameObject.Destroy(this.fadeOutAudio);
                this.fadeOutAudio = null;
            }
        }

        private void FadeOutCurrentMusic() {
            if (this.isFadingOutAudioClip) {
                // Destroy the previous audio clip that hasn't finished fading out.
                GameObject.Destroy(this.fadeOutAudio);
                this.fadeOutAudio = null;
            }
            this.fadeOutAudio = this.currentAudio;
            this.currentAudio = null;
        }
    }
}
