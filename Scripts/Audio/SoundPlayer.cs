using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio {

    /// <summary>
    /// A class for quickly importing and playing one-off sounds.
    /// </summary>
    [ResourceSingleton("SoundPlayer")]
    public class SoundPlayer : Singleton<SoundPlayer> {

        public AudioMixerGroup soundMixerGroup;

        private Dictionary<string, AudioClip> loadedSounds = new Dictionary<string, AudioClip>();

        public AudioSource PlayVaried(string soundName, float spread = 0.3f) {
            return this.PlayVaried(this.LookUpAudioClip(soundName), spread);
        }

        public AudioSource PlayVaried(AudioClip soundName, float spread = 0.3f) {
            AudioSource source = this.Play(soundName);
            source.pitch += UnityEngine.Random.Range(-spread / 2, spread / 2);
            return source;
        }

        public AudioSource Play(string soundName) {
            return this.Play(this.LookUpAudioClip(soundName));
        }

        public AudioSource Play(AudioClip clip, float volume = 1) {
            return this.Play(clip, this.transform.position, volume);
        }

        public AudioSource Play(AudioClip clip, Vector3 position, float volume = 1, float spatialBlend = 0) {
            return this.PlayOneOff(clip, position, volume, spatialBlend);
        }

        public AudioSource PlayLooped(AudioClip clip, Vector3 position) {
            AudioSource source = this.CreateAudioObject(clip, position);
            source.loop = true;
            source.Play();
            return source;
        }

        public AudioClip LookUpAudioClip(string soundName) {
            if (loadedSounds.ContainsKey(soundName)) {
                return this.loadedSounds[soundName];
            } else {
                AudioClip clip = Resources.Load<AudioClip>(soundName);
                if (clip == null) {
                    Debug.LogWarning("Tried to play sound from string, but failed: " + soundName);
                    return null;
                } else {
                    this.loadedSounds.Add(soundName, clip);
                    return clip;
                }
            }
        }

        private AudioSource PlayOneOff(AudioClip clip, Vector3 position, float volume = 1, float spatialBlend = 0) {
            if (clip == null) {
                Debug.LogWarning("Audio clip is not assigned to a value!");
                return null;
            }

            AudioSource audioSource = this.CreateAudioObject(clip, position);
            audioSource.volume = volume;
            audioSource.spatialBlend = spatialBlend;
            audioSource.outputAudioMixerGroup = this.soundMixerGroup;

            audioSource.Play();

            Timer.Register(clip.length + 0.1f, () => {
                if (audioSource != null) {
                    GameObject.Destroy(audioSource.gameObject);
                }
            }, false, true);
            return audioSource;
        }

        private AudioSource CreateAudioObject(AudioClip clip, Vector3 position) {
            GameObject soundGameObject = new GameObject("AudioSource (Temp)");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            return audioSource;
        }
    }

    public static class SoundPlayerExtensions {
        public static AudioSource Play(this AudioClip clip) {
            return SoundPlayer.instance.Play(clip);
        }
    }
}
