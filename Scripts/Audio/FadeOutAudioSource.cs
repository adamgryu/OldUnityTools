using UnityEngine;
using System.Collections;

namespace QuickUnityTools.Audio {

    public class FadeOutAudioSource : MonoBehaviour {

        public float fadeOutTime = 1f;
        public bool destroyGameObjectOnFinish = true;

        private Timer fadeOutTimer;
        private AudioSource audioSource;
        private float startVolume;

        private void Start() {
            audioSource = GetComponent<AudioSource>();
            startVolume = audioSource.volume;
            fadeOutTimer = this.RegisterTimer(fadeOutTime, () => {
                if (destroyGameObjectOnFinish) {
                    GameObject.Destroy(gameObject);
                }
            });
        }

        private void Update() {
            audioSource.volume = Mathf.Lerp(startVolume, 0, fadeOutTimer.GetPercentageComplete());
        }
    }
}
