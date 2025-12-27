using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAnimatedUI
{
    public class PageManager : MonoBehaviour
    {
        public static PageManager Instance;

        [Header("Components")]
        [SerializeField] private PageEnum startPage;
        [SerializeField] private List<Page> pages;
        [Space]
        [Header("Settings")]
        [Range(0f, 1f), SerializeField] private float pageFadeInDuration = 0.4f;
        [SerializeField] private Ease fadeInEase = Ease.InSine;
        [Space]
        [Range(0f, 1f), SerializeField] private float pageFadeOutDuration = 0.4f;
        [SerializeField] private Ease fadeOutEase = Ease.InSine;

        [Header("Inactivity Timer")]
        [SerializeField] private float inactivityTimeout = 180f;
        [SerializeField] private PageEnum mainMenuPage = PageEnum.Main;
        [SerializeField] private bool enableInactivityTimer = true;

        private Dictionary<PageEnum, Page> pageMap;
        private Page currentPage;
        private Sequence mainSequence;

        private float lastInputTime;
        private float currentInactiveTime;
        private bool isTimerRunning;
        private PageEnum lastOpenedPage;

        private float PageFadeInDuration => ConfigManager.Instance?.GetFadeInDuration() ?? 0.4f;
        private float PageFadeOutDuration => ConfigManager.Instance?.GetFadeOutDuration() ?? 0.4f;
        private Ease FadeInEase => ConfigManager.Instance?.GetFadeInEase() ?? Ease.InSine;
        private Ease FadeOutEase => ConfigManager.Instance?.GetFadeOutEase() ?? Ease.InSine;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            pageMap = new Dictionary<PageEnum, Page>();

            foreach (var page in pages)
            {
                SetPageActive(page, false);

                if (!pageMap.ContainsKey(page.Id))
                {
                    pageMap.Add(page.Id, page);
                }
                else
                {
                    Debug.LogError($"Duplicate PageID: {page.Id}");
                }
            }

            DOTween.SetTweensCapacity(500, 125);

            // Убеждаемся, что ConfigManager существует
            EnsureConfigManager();
        }

        private void EnsureConfigManager()
        {
            if (ConfigManager.Instance == null)
            {
                GameObject configManager = new GameObject("ConfigManager");
                configManager.AddComponent<ConfigManager>();
                DontDestroyOnLoad(configManager);
            }
        }

        private void Start()
        {
            foreach (var page in pages)
            {
                if (page.Id == startPage)
                {
                    currentPage = page;
                }
            }

            if (currentPage != null)
            {
                SetPageActive(currentPage, true);
            }
            else
            {
                Debug.LogWarning($"[PageManager.Start] currentPage == null");
            }
        }

        private void Update()
        {
            if (!enableInactivityTimer || !isTimerRunning)
                return;

            if (IsAnyInput())
            {
                ResetInactivityTimer();
                return;
            }

            currentInactiveTime += Time.deltaTime;

            if (currentInactiveTime >= inactivityTimeout)
            {
                OnInactivityTimeout();
            }
        }

        public void OpenPage(PageEnum pageID)
        {
            if (!pageMap.TryGetValue(pageID, out var nextPage))
            {
                Debug.LogWarning($"[PageManager.OpenPage] Page {pageID} not found");
                return;
            }
            if (mainSequence != null && mainSequence.IsActive())
            {
                mainSequence.Kill();
            }

            mainSequence = DOTween.Sequence();


            if (currentPage == nextPage)
            {
                Debug.LogWarning($"[PageManager.OpenPage] CurrentPage == {nextPage}");

                Sequence fadeOut = DOTween.Sequence();
                fadeOut.Append(currentPage.Header.DOFade(0f, PageFadeOutDuration).SetEase(FadeOutEase));
                fadeOut.Join(currentPage.Body.DOFade(0f, PageFadeOutDuration).SetEase(FadeOutEase));

                Sequence fadeIn = DOTween.Sequence();
                fadeIn.Append(currentPage.Header.DOFade(1f, PageFadeInDuration).SetEase(FadeInEase));
                fadeIn.Join(currentPage.Body.DOFade(1f, PageFadeInDuration).SetEase(FadeInEase));

                mainSequence.Append(fadeOut);
                mainSequence.Append(fadeIn);

                mainSequence.Play();
                return;
            }

            if (currentPage != null)
            {
                DisableInteractivity(currentPage);

                mainSequence.Append(GetOutAnimationSequence(currentPage));

                mainSequence.AppendCallback(() =>
                {
                    SetPageActive(currentPage, false);
                });

                ShowNextPage(nextPage);

                mainSequence.Append(GetInAnimationSequence(nextPage));

                mainSequence.AppendCallback(() =>
                {
                    SetPageActive(nextPage, true);
                    currentPage = nextPage;
                });

                mainSequence.Play();
            }
            else
            {
                Debug.LogError($"Can't found currentPage");
            }
        }


        #region PageControl

        private void SetPageActive(Page page, bool active)
        {
            SetObjectActive(page.Header, active);
            SetObjectActive(page.Body, active);
        }

        private void SetObjectActive(CanvasGroup group, bool active)
        {
            if (group == null)
                return;

            var go = group.gameObject;
            go.SetActive(active);

            group.alpha = active ? 1f : 0f;
            group.interactable = active;
            group.blocksRaycasts = active;
        }

        private void ShowNextPage(Page page)
        {
            page.Header.gameObject.SetActive(true);
            page.Body.gameObject.SetActive(true);
        }

        private void DisableInteractivity(Page page)
        {
            page.Header.interactable = false;
            page.Body.interactable = false;
        }

        #endregion


        #region Animations

        private Sequence GetOutAnimationSequence(Page page)
        {
            Sequence outSequence = DOTween.Sequence();

            if (page == null) return outSequence;

            IAnimator[] headerAnimations = page.Header.GetComponentsInChildren<IAnimator>();
            IAnimator[] bodyAnimations = page.Body.GetComponentsInChildren<IAnimator>();

            foreach (var tweenAnimation in headerAnimations)
            {
                Tween tween = tweenAnimation.GetOutAnimation();
                if (tween != null)
                {
                    outSequence.Join(tween);
                }
            }

            foreach (var tweenAnimation in bodyAnimations)
            {
                Tween tween = tweenAnimation.GetOutAnimation();
                if (tween != null)
                {
                    outSequence.Join(tween);
                }
            }

            outSequence.Join(page.Header.DOFade(0f, PageFadeOutDuration)).SetEase(FadeOutEase);
            outSequence.Join(page.Body.DOFade(0f, PageFadeOutDuration)).SetEase(FadeOutEase);

            return outSequence;
        }

        private Sequence GetInAnimationSequence(Page page)
        {
            Sequence inSequence = DOTween.Sequence();

            if (page == null) return inSequence;

            IAnimator[] headerAnimations = page.Header.GetComponentsInChildren<IAnimator>();
            IAnimator[] bodyAnimations = page.Body.GetComponentsInChildren<IAnimator>();

            foreach (var tweenAnimation in headerAnimations)
            {
                Tween tween = tweenAnimation.GetInAnimation();
                if (tween != null)
                {
                    inSequence.Join(tween);
                }
            }

            foreach (var tweenAnimation in bodyAnimations)
            {
                Tween tween = tweenAnimation.GetInAnimation();
                if (tween != null)
                {
                    inSequence.Join(tween);
                }
            }

            page.Header.alpha = 0f;
            page.Body.alpha = 0f;

            inSequence.Join(page.Header.DOFade(1f, PageFadeInDuration)).SetEase(FadeInEase);
            inSequence.Join(page.Body.DOFade(1f, PageFadeInDuration)).SetEase(FadeInEase);

            return inSequence;
        }

        #endregion



        #region Timer

        private void StartInactivityTimer()
        {
            if (!enableInactivityTimer)
                return;

            ResetInactivityTimer();
            isTimerRunning = true;
        }

        private void StopInactivityTimer()
        {
            isTimerRunning = false;
            currentInactiveTime = 0f;
        }

        private void ResetInactivityTimer()
        {
            if (!isTimerRunning)
                return;

            currentInactiveTime = 0f;
            lastInputTime = Time.time;
        }

        private void OnInactivityTimeout()
        {
            if (!enableInactivityTimer)
                return;

            if (currentPage != null && currentPage.Id == mainMenuPage)
            {
                ResetInactivityTimer();
                return;
            }

            OpenPage(mainMenuPage);

            ResetInactivityTimer();

            lastOpenedPage = mainMenuPage;
        }

        private bool IsAnyInput()
        {
            if (Input.anyKeyDown)
                return true;

            if (Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2))
                return true;

            if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f ||
                Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f)
                return true;

#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began || 
                        touch.phase == TouchPhase.Moved)
                        return true;
                }
            }
#endif

            return false;
        }
        #endregion
    }
}
