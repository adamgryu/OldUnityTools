using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickUnityTools.Audio {
    /// <summary>
    /// An audio clip and metadata required for smooth looping.
    /// </summary>
    [CreateAssetMenu]
    public class MusicData : ScriptableObject {
        public AudioClip clip;
        public float introMeasures = 1;
        public float beatsPerMeasure = 4;
        public float beatsPerMinute = 130;
        public float length { get { return clip.length; } }
    }
}