using UnityEngine;
using UnityEngine.Rendering;

namespace Core
{
    public class CrescendoManager : MonoBehaviour
    {
        [SerializeField]
        private int maxScore;
        [SerializeField]
        private AudioSource soundTrackSource;
        [SerializeField]
        private Volume postVolume;
        [SerializeField]
        private AnimationCurve crescendoCurve;

        public void UpdateCrescendo(int currentScore)
        {
            var ratio = Mathf.Clamp(currentScore, 0, maxScore) / (float)maxScore;
            var curveValue = crescendoCurve.Evaluate(ratio);
            soundTrackSource.volume = curveValue;
            postVolume.weight = curveValue;
        }
    }
}
