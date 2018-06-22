using UnityEngine;
using System.Collections;
using System;

namespace QuickUnityTools.Audio {

    [Serializable]
    public class BasicMusicStackElement : IMusicStackElement {

        public AudioClip music;
        public MusicStackPriorty priority = MusicStackPriorty.Low;
        public MusicStack.Transition transitionIn = MusicStack.Transition.CROSS_FADE;
        public MusicStack.Transition transitionOut = MusicStack.Transition.CROSS_FADE;
        public float desiredVolume = 1;

        public BasicMusicStackElement(AudioClip music) {
            this.music = music;
        }

        public MusicStack.IMusicAsset GetMusicAsset() {
            return music != null ? new AudioClipMusicAsset(music) : null;
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

        public MusicStackPriorty GetPriority() {
            return priority;
        }
    }
}