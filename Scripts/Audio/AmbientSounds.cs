using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace QuickUnityTools.Audio {
    public class AmbientSounds : ServiceMonoBehaviour {

        [Serializable]
        public class AmbientSound {
            public AudioClip clip;
            public float volume = 1;
            public bool fadeIn = true;
            public AudioSource existingSource;
        }

        public AmbientSound[] soundsInScene;
        public AudioMixerGroup mixerGroup;

        private void Awake() {
            AmbientSoundManager.instance.Initalize(this);
        }

        protected override void OnEnable() {
            base.OnEnable();
            AmbientSoundManager.instance.UpdateSounds(soundsInScene);
        }

        public void SetSounds(AmbientSound[] sounds) {
            soundsInScene = sounds;
            AmbientSoundManager.instance.UpdateSounds(sounds);
        }

        private class AmbientSoundManager : Singleton<AmbientSoundManager> {
            private AudioMixerGroup mixerGroup;
            private float fadeTime = 1f;
            private Dictionary<AudioClip, AudioSource> playingSounds = new Dictionary<AudioClip, AudioSource>();
            private List<Coroutine> soundFades = new List<Coroutine>();
            private bool initalized = false;

            public void Initalize(AmbientSounds soundGroup) {
                if (!initalized) {
                    mixerGroup = soundGroup.mixerGroup;
                    UpdateSounds(soundGroup.soundsInScene);
                    initalized = true;
                }
            }

            /**
             * This is the old way of triggering
            private void Awake() {
                SceneManager.activeSceneChanged += OnActiveSceneChanged;
            }

            private void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
                var soundGroups = GameObject.FindObjectsOfType<AmbientSounds>();
                if (soundGroups.Length > 1) {
                    Debug.LogWarning("Duplicate ambient sounds in this scene! There should only be one.");
                }
                var sounds = soundGroups.FirstOrDefault();
                UpdateSounds(sounds != null ? sounds.soundsInScene : new AmbientSound[0]);
            }
            */

            public void UpdateSounds(AmbientSound[] newSounds) {
                CancelPreviousFades();

                // Create sources or update sources for the sounds in the new set.
                foreach (AmbientSound sound in newSounds) {
                    if (!playingSounds.ContainsKey(sound.clip)) {
                        var newSource = sound.existingSource != null ? sound.existingSource : new GameObject(sound.clip.name).AddComponent<AudioSource>();
                        newSource.transform.parent = this.transform;
                        newSource.clip = sound.clip;
                        newSource.volume = sound.fadeIn ? 0 : sound.volume;
                        newSource.loop = true;
                        newSource.outputAudioMixerGroup = mixerGroup;
                        newSource.Play();
                        playingSounds[sound.clip] = newSource;
                    }
                    var source = playingSounds[sound.clip];
                    if (source.volume != sound.volume) {
                        FadeTowardsVolume(source, sound.volume);
                    }
                }

                // Remove sounds that are not in the new set.
                var soundsNotInScene = playingSounds.Where(playingSound => !newSounds.Any(newSound => newSound.clip == playingSound.Key)).Select(s => s.Value);
                foreach (AudioSource source in soundsNotInScene.ToArray()) {
                    source.gameObject.AddComponent<FadeOutAudioSource>();
                    playingSounds.Remove(source.clip);
                }
            }

            private void FadeTowardsVolume(AudioSource source, float volume) {
                soundFades.Add(StartCoroutine(FadeSourceVolumeRoutine(source, volume, fadeTime)));
            }

            private IEnumerator FadeSourceVolumeRoutine(AudioSource source, float targetVolume, float fadeTime) {
                float originalVolume = source.volume;
                float startTime = Time.time;
                while (Time.time - startTime < fadeTime) {
                    source.volume = Mathf.Lerp(originalVolume, targetVolume, (Time.time - startTime) / fadeTime);
                    yield return null;
                }
                source.volume = targetVolume;
            }

            private void CancelPreviousFades() {
                foreach (var fade in soundFades) {
                    StopCoroutine(fade);
                }
            }
        }
    }
}