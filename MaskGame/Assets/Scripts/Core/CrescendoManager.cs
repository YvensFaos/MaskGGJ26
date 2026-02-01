using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core
{
    public class CrescendoManager : MonoBehaviour
    {
        [SerializeField] private int maxScore;
        [SerializeField] private AudioSource soundTrackSource;
        [SerializeField] private Volume postVolume;
        [SerializeField] private AnimationCurve crescendoCurve;
        [SerializeField] private float tweenTime = 0.5f;

        private Tweener _tweener;

        public void UpdateCrescendo(int currentScore)
        {
            var ratio = Mathf.Clamp(currentScore, 0, maxScore) / (float)maxScore;
            var curveValue = crescendoCurve.Evaluate(ratio);
            _tweener.Kill();
            _tweener = DOTween.To(() => soundTrackSource.volume, value =>
            {
                soundTrackSource.volume = value;
                postVolume.weight = value;
            }, curveValue, tweenTime);
        }
    }
}