using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace QuickUnityTools.Audio {

    /// <summary>
    /// Music that plays in the background of the scene. It is added to the MusicStack for the
    /// scene's lifetime.
    /// </summary>
    public class SceneBackgroundMusic : MonoBehaviour, IMusicStackElement {

        public AudioClip music;

        public MusicStackPriorty priority = MusicStackPriorty.Low;
        public MusicStack.Transition transitionIn = MusicStack.Transition.CROSS_FADE;
        public MusicStack.Transition transitionOut = MusicStack.Transition.CROSS_FADE;
        public float desiredVolume = 1;

        private PrioritySortingKey musicStackKey;

        private void Awake() {
            musicStackKey = MusicStack.instance.AddToMusicStack(this, priority);
        }

        private void OnDestroy() {
            MusicStack.instance.RemoveFromMusicStack(musicStackKey);
        }

        public MusicStack.IMusicAsset GetMusicAsset() {
            return new AudioClipMusicAsset(music);
        }

        public MusicStack.Transition GetReleaseControlTransition() {
            return transitionOut;
        }

        public MusicStack.Transition GetTakeControlTransition() {
            return transitionIn;
        }

        public float GetDesiredVolume() {
            return desiredVolume;
        }
    }
}