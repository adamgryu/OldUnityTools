using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace QuickUnityTools.Audio {

    /// <summary>
    /// Music that plays in the background of the scene. It is added to the MusicStack for the
    /// scene's lifetime.
    /// </summary>
    public class SceneBackgroundMusic : MonoBehaviour {

        public BasicMusicStackElement musicConfig;

        private PrioritySortingKey musicStackKey;

        private void Start() {
            musicStackKey = MusicStack.instance.AddToMusicStack(musicConfig);
        }

        private void OnDestroy() {
            if (MusicStack.instance != null) {
                MusicStack.instance.RemoveFromMusicStack(musicStackKey);
            }
        }
    }
}