using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio {
    public class SmoothLoopMusicAsset : MusicStack.IMusicAsset {

        private SmoothLoopAudioClip clip;

        public SmoothLoopMusicAsset(SmoothLoopAudioClip clip) {
            this.clip = clip;
        }

        public MusicStack.IMusicPlayer CreatePlayer() {
            return new SmoothLoopMusicPlayer(clip);
        }

        public int GetMusicID() {
            return clip.GetInstanceID();
        }
    }

    public class SmoothLoopMusicPlayer : MusicStack.IMusicPlayer {
        private SmoothLoopAudioSource source;

        public SmoothLoopMusicPlayer(SmoothLoopAudioClip clip) {
            GameObject obj = new GameObject("SmoothLoopMusicPlayer (" + clip + ")");
            SmoothLoopAudioSource source = obj.AddComponent<SmoothLoopAudioSource>();
            source.music = clip;
            this.source = source;
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }

        public AudioMixerGroup outputAudioMixerGroup {
            set { source.mixerGroup = value; }
        }

        public float volume {
            get { return source.volume; }
            set { source.volume = value; }
        }

        public bool isPlaying { get { return source.isPlaying; } }

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