using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleAnimatedUI
{
    public class RectAnimator : MonoBehaviour, IAnimator
    {
        [Header("RequireComponents")]
        [SerializeField] private RectTransform targetRectTransform;

        [Header("Position Animation")]
        [SerializeField] private bool animatePosition = true;

        [Space]
        [Header("In Animation Options")]
        [SerializeField] private Vector2 inStartPosition = Vector2.zero;
        [SerializeField] private Vector2 inEndPosition = Vector2.zero;
        [Range(0f, 10f), SerializeField] private float inMoveDuration = 1f;
        [SerializeField] private Ease inMoveEase = Ease.InSine;
        [SerializeField] private float inDelay = 0f;

        [Space]
        [Header("Out Animation Options")]
        [SerializeField] private Vector2 outStartPosition = Vector2.zero;
        [SerializeField] private Vector2 outEndPosition = Vector2.zero;
        [Range(0f, 10f), SerializeField] private float outMoveDuration = 1f;
        [SerializeField] private Ease outMoveEase = Ease.OutSine;
        [SerializeField] private float outDelay = 0f;

        [Space]
        [Header("Scale Animation")]
        [SerializeField] private bool animateScale = false;

        [Space]
        [Header("In Scale Options")]
        [SerializeField] private Vector3 inStartScale = Vector3.one;
        [SerializeField] private Vector3 inEndScale = Vector3.one;
        [Range(0f, 10f), SerializeField] private float inScaleDuration = 1f;
        [SerializeField] private Ease inScaleEase = Ease.InSine;
        [SerializeField] private float inScaleDelay = 0f;

        [Space]
        [Header("Out Scale Options")]
        [SerializeField] private Vector3 outStartScale = Vector3.one;
        [SerializeField] private Vector3 outEndScale = Vector3.one;
        [Range(0f, 10f), SerializeField] private float outScaleDuration = 1f;
        [SerializeField] private Ease outScaleEase = Ease.OutSine;
        [SerializeField] private float outScaleDelay = 0f;

        [Header("Anchor Settings")]
        [SerializeField] private bool useAnchoredPosition = true;
        [SerializeField] private bool resetPositionOnAwake = true;
        [SerializeField] private bool resetScaleOnAwake = true;

        private Tween currentTween;
        private Vector2 originalPosition;
        private Vector3 originalScale;

        private void Awake()
        {
            if (targetRectTransform == null)
            {
                targetRectTransform = GetComponent<RectTransform>();
            }

            originalPosition = useAnchoredPosition ?
                targetRectTransform.anchoredPosition :
                targetRectTransform.localPosition;

            originalScale = targetRectTransform.localScale;

            if (resetPositionOnAwake)
            {
                ResetToOriginalPosition();
            }

            if (resetScaleOnAwake)
            {
                ResetToOriginalScale();
            }
        }

        public Tween GetInAnimation()
        {
            if (targetRectTransform == null)
            {
                Debug.LogError("[RectAnimator] RectTransform component not found");
                return null;
            }

            if (currentTween != null)
            {
                currentTween.Kill();
            }

            if (animatePosition)
            {
                SetRectPosition(inStartPosition);
            }

            if (animateScale)
            {
                SetRectScale(inStartScale);
            }

            Sequence sequence = DOTween.Sequence();

            if (inDelay > 0)
            {
                sequence.AppendInterval(inDelay);
            }

            Sequence parallelSequence = DOTween.Sequence();

            // Анимация позиции (если включена)
            if (animatePosition)
            {
                Tween posTween = CreatePositionTween(true);

                if (inDelay > 0)
                {
                    parallelSequence.Join(
                        DOTween.Sequence()
                            .AppendInterval(inDelay)
                            .Append(posTween)
                    );
                }
                else
                {
                    parallelSequence.Join(posTween);
                }
            }

            if (animateScale)
            {
                Tween scaleTween = CreateScaleTween(true);

                float scaleDelay = inScaleDelay;
                if (scaleDelay > 0)
                {
                    parallelSequence.Join(
                        DOTween.Sequence()
                            .AppendInterval(scaleDelay)
                            .Append(scaleTween)
                    );
                }
                else
                {
                    parallelSequence.Join(scaleTween);
                }
            }

            if (animatePosition || animateScale)
            {
                sequence.Append(parallelSequence);
            }

            currentTween = sequence;
            return sequence;
        }

        public Tween GetOutAnimation()
        {
            if (targetRectTransform == null)
            {
                Debug.LogError("[RectAnimator] RectTransform component not found");
                return null;
            }

            if (currentTween != null)
            {
                currentTween.Kill();
            }

            if (animatePosition)
            {
                SetRectPosition(outStartPosition);
            }

            if (animateScale)
            {
                SetRectScale(outStartScale);
            }

            Sequence sequence = DOTween.Sequence();

            if (outDelay > 0)
            {
                sequence.AppendInterval(outDelay);
            }

            Sequence parallelSequence = DOTween.Sequence();

            if (animatePosition)
            {
                Tween posTween = CreatePositionTween(false);

                if (outDelay > 0)
                {
                    parallelSequence.Join(
                        DOTween.Sequence()
                            .AppendInterval(outDelay)
                            .Append(posTween)
                    );
                }
                else
                {
                    parallelSequence.Join(posTween);
                }
            }

            if (animateScale)
            {
                Tween scaleTween = CreateScaleTween(false);

                float scaleDelay = outScaleDelay;
                if (scaleDelay > 0)
                {
                    parallelSequence.Join(
                        DOTween.Sequence()
                            .AppendInterval(scaleDelay)
                            .Append(scaleTween)
                    );
                }
                else
                {
                    parallelSequence.Join(scaleTween);
                }
            }

            if (animatePosition || animateScale)
            {
                sequence.Append(parallelSequence);
            }

            currentTween = sequence;
            return sequence;
        }

        public void KillAnimation()
        {
            if (currentTween != null && currentTween.IsActive())
            {
                currentTween.Kill();
                currentTween = null;
            }
        }

        #region Helper Methods
        private Tween CreatePositionTween(bool isInAnimation)
        {
            Vector2 targetPos = isInAnimation ? inEndPosition : outEndPosition;
            float duration = isInAnimation ? inMoveDuration : outMoveDuration;
            Ease ease = isInAnimation ? inMoveEase : outMoveEase;

            return useAnchoredPosition ?
                targetRectTransform.DOAnchorPos(targetPos, duration).SetEase(ease) :
                targetRectTransform.DOLocalMove(targetPos, duration).SetEase(ease);
        }

        private Tween CreateScaleTween(bool isInAnimation)
        {
            Vector3 targetScale = isInAnimation ? inEndScale : outEndScale;
            float duration = isInAnimation ? inScaleDuration : outScaleDuration;
            Ease ease = isInAnimation ? inScaleEase : outScaleEase;

            return targetRectTransform.DOScale(targetScale, duration).SetEase(ease);
        }

        private void SetRectPosition(Vector2 position)
        {
            if (useAnchoredPosition)
            {
                targetRectTransform.anchoredPosition = position;
            }
            else
            {
                targetRectTransform.localPosition = position;
            }
        }

        private void SetRectScale(Vector3 scale)
        {
            targetRectTransform.localScale = scale;
        }
        #endregion

        #region Public Methods
        public void ResetToOriginalPosition()
        {
            SetRectPosition(originalPosition);
        }

        public void ResetToOriginalScale()
        {
            SetRectScale(originalScale);
        }

        public void ResetToOriginal()
        {
            ResetToOriginalPosition();
            ResetToOriginalScale();
        }

        public void SaveCurrentPositionAsOriginal()
        {
            originalPosition = useAnchoredPosition ?
                targetRectTransform.anchoredPosition :
                targetRectTransform.localPosition;
        }

        public void SaveCurrentScaleAsOriginal()
        {
            originalScale = targetRectTransform.localScale;
        }

        public void SaveCurrentAsOriginal()
        {
            SaveCurrentPositionAsOriginal();
            SaveCurrentScaleAsOriginal();
        }

        public void SetPositionImmediately(Vector2 position)
        {
            KillAnimation();
            SetRectPosition(position);
        }

        public void SetScaleImmediately(Vector3 scale)
        {
            KillAnimation();
            SetRectScale(scale);
        }

        public void SetInPositions(Vector2 startPos, Vector2 endPos)
        {
            inStartPosition = startPos;
            inEndPosition = endPos;
        }

        public void SetOutPositions(Vector2 startPos, Vector2 endPos)
        {
            outStartPosition = startPos;
            outEndPosition = endPos;
        }

        public void SetInScales(Vector3 startScale, Vector3 endScale)
        {
            inStartScale = startScale;
            inEndScale = endScale;
        }

        public void SetOutScales(Vector3 startScale, Vector3 endScale)
        {
            outStartScale = startScale;
            outEndScale = endScale;
        }

        public void SetAllPositions(Vector2 inStart, Vector2 inEnd, Vector2 outStart, Vector2 outEnd)
        {
            inStartPosition = inStart;
            inEndPosition = inEnd;
            outStartPosition = outStart;
            outEndPosition = outEnd;
        }

        public void SetAllScales(Vector3 inStart, Vector3 inEnd, Vector3 outStart, Vector3 outEnd)
        {
            inStartScale = inStart;
            inEndScale = inEnd;
            outStartScale = outStart;
            outEndScale = outEnd;
        }

        public void EnablePositionAnimation(bool enable)
        {
            animatePosition = enable;
        }

        public void EnableScaleAnimation(bool enable)
        {
            animateScale = enable;
        }

        public void SetInScaleUniform(float startScale, float endScale)
        {
            inStartScale = Vector3.one * startScale;
            inEndScale = Vector3.one * endScale;
        }

        public void SetOutScaleUniform(float startScale, float endScale)
        {
            outStartScale = Vector3.one * startScale;
            outEndScale = Vector3.one * endScale;
        }

        public void SetScaleUniform(float scale)
        {
            SetScaleImmediately(Vector3.one * scale);
        }

        public bool HasActiveAnimations()
        {
            return animatePosition || animateScale;
        }
        #endregion
    }
}