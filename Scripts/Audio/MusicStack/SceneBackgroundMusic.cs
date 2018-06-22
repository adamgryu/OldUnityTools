using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace QuickUnityTools.Audio {

    public class SceneBackgroundMusic : MonoBehaviour, IMusicStackElement {

        public AudioClip music;

        public MusicStackPriorty priority = MusicStackPriorty.Low;
        public MusicStackTransition transitionIn = MusicStackTransition.CROSS_FADE;
        public MusicStackTransition transitionOut = MusicStackTransition.CROSS_FADE;
        public float desiredVolume = 1;

        private PrioritySortingKey musicStackKey;

        private void Awake() {
            musicStackKey = MusicStack.instance.AddToMusicStack(this, priority);
        }

        private void OnDestroy() {
            MusicStack.instance.RemoveFromMusicStack(musicStackKey);
        }

        public IMusicStackAudioData GetAudioData() {
            return new AudioClipMusicStackAudioData(music);
        }

        public MusicStackTransition GetReleaseControlTransition() {
            return transitionOut;
        }

        public MusicStackTransition GetTakeControlTransition() {
            return transitionIn;
        }

        public float GetDesiredVolume() {
            return desiredVolume;
        }
    }
}