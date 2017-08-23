using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickUnityTools.Audio {
    public class IntroLoopMusicSource : MonoBehaviour {

        private const int SECONDS_IN_MINUTE = 60;

        public MusicData music;

        private AudioSource[] audioSources;
        private float introTime { get { return ((music.beatsPerMeasure * music.introMeasures) / music.beatsPerMinute) * SECONDS_IN_MINUTE; } }
        private float loopTime { get { return music.length - introTime; } }

        private double startDpsTime;
        private int nextAudioSourceIndex;
        private int numberOfLoopsScheduled;
        private Timer loopTimer;

        void Start() {
            audioSources = new AudioSource[2];
            for (int i = 0; i < audioSources.Length; i++) {
                audioSources[i] = gameObject.AddComponent<AudioSource>();
                audioSources[i].clip = music.clip;
            }
            Play();
        }

        public void Play() {
            // Reset the play state.
            for (int i = 0; i < audioSources.Length; i++) {
                audioSources[i].Stop();
            }
            if (loopTimer != null) {
                loopTimer.Cancel();
            }
            startDpsTime = AudioSettings.dspTime;

            // Play the full track once.
            audioSources[0].Play();

            // Schedule the track to play again from its looping position.
            audioSources[1].PlayScheduled(startDpsTime + music.length);
            audioSources[1].time = introTime;

            nextAudioSourceIndex = 0;
            numberOfLoopsScheduled = 1;

            // After the first run of the track finishes, schedule the second loop while the first loop is playing.
            loopTimer = this.RegisterTimer(music.length, ScheduleNextLoop);
        }

        /// <summary>
        /// Schedules the next loop to play. When this function executes, the last scheduled loop
        /// should just be beginning to play.
        /// 
        /// This method will call itself to schedule another loop after each loop finishes.
        /// </summary>
        private void ScheduleNextLoop() {
            audioSources[nextAudioSourceIndex].PlayScheduled(startDpsTime + music.length + loopTime * numberOfLoopsScheduled);
            audioSources[nextAudioSourceIndex].time = introTime;

            nextAudioSourceIndex = (nextAudioSourceIndex + 1) % audioSources.Length;
            numberOfLoopsScheduled++;

            loopTimer = this.RegisterTimer(loopTime, ScheduleNextLoop);
        }
    }
}