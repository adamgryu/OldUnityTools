using UnityEngine;
using System.Collections;

namespace QuickUnityTools.Audio {

    public class FadeOutAudioSource : MonoBehaviour {

        public float fadeOutTime = 1f;
        public bool destroyGameObjectOnFinish = true;

        private Timer fadeOutTimer;
        private AudioSource audioSource;
        private float startVolume;

        void Start() {
            this.audioSource = this.GetComponent<AudioSource>();
            this.startVolume = this.audioSource.volume;
            this.fadeOutTimer = this.RegisterTimer(this.fadeOutTime, () => {
                if (this.destroyGameObjectOnFinish) {
                    GameObject.Destroy(this.gameObject);
                }
            });
        }

        void Update() {
            if (this.fadeOutTimer != null) {
                this.audioSource.volume = Mathf.Lerp(this.startVolume, 0, this.fadeOutTimer.GetPercentageComplete());
            }
        }
    }
}
