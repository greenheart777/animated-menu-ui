using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleAnimatedUI
{
    public class ImageAnimator : MonoBehaviour
    {
        [Header("RequireComponents")]
        [SerializeField] private Image targetImage;


        [Header("Options")]
        [Range(0f, 1f), SerializeField] private float inStartAlpha = 0f;
        [Range(0f, 1f), SerializeField] private float inEndAlpha = 1f;
        [Range(0f, 10f), SerializeField] private float inDelay = 0f;
        [Range(0f, 10f), SerializeField] private float inFadeDuration = 1f;
        [SerializeField] private Ease inFadeEase = Ease.InSine;
        [Space]
        [Range(0f, 1f), SerializeField] private float outStartAlpha = 1f;
        [Range(0f, 1f), SerializeField] private float outEndAlpha = 0f;
        [Range(0f, 10f), SerializeField] private float outDelay = 0f;
        [Range(0f, 10f), SerializeField] private float outFadeDuration = 1f;
        [SerializeField] private Ease outFadeEase = Ease.InSine;

        private Tween currentTween;


        public Tween GetInAnimation()
        {
            if (targetImage == null)
            {
                Debug.LogError("[ImageAnimator] Image component not found");
                return null;
            }

            if (currentTween != null)
            {
                currentTween.Kill();
            }

            Color startColor = targetImage.color;
            startColor.a = inStartAlpha;
            targetImage.color = startColor;

            Sequence sequence = DOTween.Sequence();

            if (inDelay > 0)
            {
                sequence.AppendInterval(inDelay);
            }

            sequence.Join(targetImage.DOFade(inEndAlpha, inFadeDuration).SetEase(inFadeEase));

            currentTween = sequence;
            return sequence;
        }

        public Tween GetOutAnimation()
        {
            if (targetImage == null)
            {
                Debug.LogError("[ImageAnimator] Image component not found");
                return null;
            }

            if (currentTween != null)
            {
                currentTween.Kill();
            }

            Color startColor = targetImage.color;
            startColor.a = outStartAlpha;
            targetImage.color = startColor;

            Sequence sequence = DOTween.Sequence();

            if (outDelay > 0)
            {
                sequence.AppendInterval(outDelay);
            }

            sequence.Join(targetImage.DOFade(outEndAlpha, outFadeDuration).SetEase(outFadeEase));

            currentTween = sequence;
            return sequence;
        }
    } 
}
