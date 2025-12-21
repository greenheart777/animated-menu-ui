using DG.Tweening;
using TMPro;
using UnityEngine;

namespace SimpleAnimatedUI
{
    public class TextAnimator : MonoBehaviour, IAnimator
    {
        [Header("RequireComponents")]
        [SerializeField] private TMP_Text targetText;

        [Header("In Animation")]
        [Range(0f, 1f), SerializeField] private float inStartAlpha = 0f;
        [Range(0f, 1f), SerializeField] private float inEndAlpha = 1f;
        [Range(0f, 10f), SerializeField] private float inFadeDuration = 1f;
        [SerializeField] private Ease inFadeEase = Ease.InSine;
        [SerializeField] private float inDelay = 0f;
        [SerializeField] private bool playOnEnable = false;

        [Header("Out Animation")]
        [Range(0f, 1f), SerializeField] private float outStartAlpha = 1f;
        [Range(0f, 1f), SerializeField] private float outEndAlpha = 0f;
        [Range(0f, 10f), SerializeField] private float outFadeDuration = 1f;
        [SerializeField] private Ease outFadeEase = Ease.InSine;
        [SerializeField] private float outDelay = 0f;

        private Tween currentTween;

        private void OnDisable()
        {
            KillAnimation();
        }

        public Tween GetInAnimation()
        {
            if (!ValidateText()) return null;
            KillAnimation();

            Color startColor = targetText.color;
            startColor.a = inStartAlpha;
            targetText.color = startColor;

            Sequence sequence = DOTween.Sequence();

            if (inDelay > 0)
            {
                sequence.AppendInterval(inDelay);
            }

            sequence.Append(targetText.DOFade(inEndAlpha, inFadeDuration).SetEase(inFadeEase));

            currentTween = sequence;
            return sequence;
        }

        public Tween GetOutAnimation()
        {
            if (!ValidateText()) return null;
            KillAnimation();

            Color startColor = targetText.color;
            startColor.a = outStartAlpha;
            targetText.color = startColor;

            Sequence sequence = DOTween.Sequence();

            if (outDelay > 0)
            {
                sequence.AppendInterval(outDelay);
            }

            sequence.Append(targetText.DOFade(outEndAlpha, outFadeDuration).SetEase(outFadeEase));

            currentTween = sequence;
            return sequence;
        }



        public void KillAnimation()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
                currentTween = null;
            }
        }

        private bool ValidateText()
        {
            if (targetText == null)
            {
                Debug.LogError($"[{nameof(TextAnimator)}] TMP_Text component is not assigned", this);
                return false;
            }

            return true;
        }
    }
}