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
        [SerializeField] private Ease outMoveEase = Ease.InSine;
        [SerializeField] private float outDelay = 0f;

        [Header("Size Animation")]
        [SerializeField] private bool animateSize = false;

        [Space]
        [Header("In Size Options")]
        [SerializeField] private Vector2 inStartSize = Vector2.one * 100f;
        [SerializeField] private Vector2 inEndSize = Vector2.one * 100f;
        [Range(0f, 10f), SerializeField] private float inSizeDuration = 1f;
        [SerializeField] private Ease inSizeEase = Ease.InSine;
        [SerializeField] private float inSizeDelay = 0f;

        [Space]
        [Header("Out Size Options")]
        [SerializeField] private Vector2 outStartSize = Vector2.one * 100f;
        [SerializeField] private Vector2 outEndSize = Vector2.one * 100f;
        [Range(0f, 10f), SerializeField] private float outSizeDuration = 1f;
        [SerializeField] private Ease outSizeEase = Ease.InSine;
        [SerializeField] private float outSizeDelay = 0f;

        [Header("Anchor Settings")]
        [SerializeField] private bool useAnchoredPosition = true;
        [SerializeField] private bool resetPositionOnAwake = true;
        [SerializeField] private bool resetSizeOnAwake = true;

        [Header("Animation Mode")]
        [SerializeField] private AnimationMode animationMode = AnimationMode.Sequential;

        private Tween currentTween;
        private Vector2 originalPosition;
        private Vector2 originalSize;

        public enum AnimationMode
        {
            Sequential,     // Позиция и размер анимируются последовательно
            Parallel,       // Позиция и размер анимируются одновременно
            PositionOnly,   // Только позиция
            SizeOnly        // Только размер
        }

        private void Awake()
        {
            if (targetRectTransform == null)
            {
                targetRectTransform = GetComponent<RectTransform>();
            }

            originalPosition = useAnchoredPosition ?
                targetRectTransform.anchoredPosition :
                targetRectTransform.localPosition;

            originalSize = targetRectTransform.sizeDelta;

            if (resetPositionOnAwake)
            {
                ResetToOriginalPosition();
            }

            if (resetSizeOnAwake)
            {
                ResetToOriginalSize();
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

            // Устанавливаем начальные значения
            if (animatePosition)
            {
                SetRectPosition(inStartPosition);
            }

            if (animateSize)
            {
                SetRectSize(inStartSize);
            }

            Sequence sequence = DOTween.Sequence();

            // Добавляем задержку для позиции
            if (inDelay > 0)
            {
                sequence.AppendInterval(inDelay);
            }

            // Определяем режим анимации
            switch (animationMode)
            {
                case AnimationMode.Sequential:
                    AddSequentialAnimation(sequence, true);
                    break;

                case AnimationMode.Parallel:
                    AddParallelAnimation(sequence, true);
                    break;

                case AnimationMode.PositionOnly:
                    if (animatePosition)
                    {
                        AddPositionAnimation(sequence, true);
                    }
                    break;

                case AnimationMode.SizeOnly:
                    if (animateSize)
                    {
                        AddSizeAnimation(sequence, true);
                    }
                    break;
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

            // Устанавливаем начальные значения
            if (animatePosition)
            {
                SetRectPosition(outStartPosition);
            }

            if (animateSize)
            {
                SetRectSize(outStartSize);
            }

            Sequence sequence = DOTween.Sequence();

            // Добавляем задержку для позиции
            if (outDelay > 0)
            {
                sequence.AppendInterval(outDelay);
            }

            // Определяем режим анимации
            switch (animationMode)
            {
                case AnimationMode.Sequential:
                    AddSequentialAnimation(sequence, false);
                    break;

                case AnimationMode.Parallel:
                    AddParallelAnimation(sequence, false);
                    break;

                case AnimationMode.PositionOnly:
                    if (animatePosition)
                    {
                        AddPositionAnimation(sequence, false);
                    }
                    break;

                case AnimationMode.SizeOnly:
                    if (animateSize)
                    {
                        AddSizeAnimation(sequence, false);
                    }
                    break;
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
        private void AddSequentialAnimation(Sequence sequence, bool isInAnimation)
        {
            if (animatePosition)
            {
                AddPositionAnimation(sequence, isInAnimation);
            }

            if (animateSize)
            {
                // Добавляем задержку для размера если нужно
                if (isInAnimation && inSizeDelay > 0)
                {
                    sequence.AppendInterval(inSizeDelay);
                }
                else if (!isInAnimation && outSizeDelay > 0)
                {
                    sequence.AppendInterval(outSizeDelay);
                }

                AddSizeAnimation(sequence, isInAnimation);
            }
        }

        private void AddParallelAnimation(Sequence sequence, bool isInAnimation)
        {
            Sequence parallelSequence = DOTween.Sequence();

            if (animatePosition)
            {
                Tween posTween = CreatePositionTween(isInAnimation);
                parallelSequence.Join(posTween);
            }

            if (animateSize)
            {
                Tween sizeTween = CreateSizeTween(isInAnimation);

                // Добавляем задержку для размера если нужно
                float sizeDelay = isInAnimation ? inSizeDelay : outSizeDelay;
                if (sizeDelay > 0)
                {
                    parallelSequence.Join(
                        DOTween.Sequence()
                            .AppendInterval(sizeDelay)
                            .Append(sizeTween)
                    );
                }
                else
                {
                    parallelSequence.Join(sizeTween);
                }
            }

            sequence.Append(parallelSequence);
        }

        private void AddPositionAnimation(Sequence sequence, bool isInAnimation)
        {
            Tween posTween = CreatePositionTween(isInAnimation);
            sequence.Append(posTween);
        }

        private void AddSizeAnimation(Sequence sequence, bool isInAnimation)
        {
            Tween sizeTween = CreateSizeTween(isInAnimation);
            sequence.Append(sizeTween);
        }

        private Tween CreatePositionTween(bool isInAnimation)
        {
            Vector2 targetPos = isInAnimation ? inEndPosition : outEndPosition;
            float duration = isInAnimation ? inMoveDuration : outMoveDuration;
            Ease ease = isInAnimation ? inMoveEase : outMoveEase;

            return useAnchoredPosition ?
                targetRectTransform.DOAnchorPos(targetPos, duration).SetEase(ease) :
                targetRectTransform.DOLocalMove(targetPos, duration).SetEase(ease);
        }

        private Tween CreateSizeTween(bool isInAnimation)
        {
            Vector2 targetSize = isInAnimation ? inEndSize : outEndSize;
            float duration = isInAnimation ? inSizeDuration : outSizeDuration;
            Ease ease = isInAnimation ? inSizeEase : outSizeEase;

            return targetRectTransform.DOSizeDelta(targetSize, duration).SetEase(ease);
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

        private void SetRectSize(Vector2 size)
        {
            targetRectTransform.sizeDelta = size;
        }
        #endregion

        #region Public Methods
        public void ResetToOriginalPosition()
        {
            SetRectPosition(originalPosition);
        }

        public void ResetToOriginalSize()
        {
            SetRectSize(originalSize);
        }

        public void ResetToOriginal()
        {
            ResetToOriginalPosition();
            ResetToOriginalSize();
        }

        public void SaveCurrentPositionAsOriginal()
        {
            originalPosition = useAnchoredPosition ?
                targetRectTransform.anchoredPosition :
                targetRectTransform.localPosition;
        }

        public void SaveCurrentSizeAsOriginal()
        {
            originalSize = targetRectTransform.sizeDelta;
        }

        public void SaveCurrentAsOriginal()
        {
            SaveCurrentPositionAsOriginal();
            SaveCurrentSizeAsOriginal();
        }

        public void SetPositionImmediately(Vector2 position)
        {
            KillAnimation();
            SetRectPosition(position);
        }

        public void SetSizeImmediately(Vector2 size)
        {
            KillAnimation();
            SetRectSize(size);
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

        public void SetInSizes(Vector2 startSize, Vector2 endSize)
        {
            inStartSize = startSize;
            inEndSize = endSize;
        }

        public void SetOutSizes(Vector2 startSize, Vector2 endSize)
        {
            outStartSize = startSize;
            outEndSize = endSize;
        }

        public void SetAllPositions(Vector2 inStart, Vector2 inEnd, Vector2 outStart, Vector2 outEnd)
        {
            inStartPosition = inStart;
            inEndPosition = inEnd;
            outStartPosition = outStart;
            outEndPosition = outEnd;
        }

        public void SetAllSizes(Vector2 inStart, Vector2 inEnd, Vector2 outStart, Vector2 outEnd)
        {
            inStartSize = inStart;
            inEndSize = inEnd;
            outStartSize = outStart;
            outEndSize = outEnd;
        }

        public void EnablePositionAnimation(bool enable)
        {
            animatePosition = enable;
        }

        public void EnableSizeAnimation(bool enable)
        {
            animateSize = enable;
        }

        public void SetAnimationMode(AnimationMode mode)
        {
            animationMode = mode;
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (targetRectTransform == null)
            {
                targetRectTransform = GetComponent<RectTransform>();
            }

            if (!animatePosition && !animateSize)
            {
                Debug.LogWarning("[RectAnimator] Both position and size animations are disabled!");
            }
            else if (!animatePosition && animateSize)
            {
                animationMode = AnimationMode.SizeOnly;
            }
            else if (animatePosition && !animateSize)
            {
                animationMode = AnimationMode.PositionOnly;
            }
        }
#endif
    }
}