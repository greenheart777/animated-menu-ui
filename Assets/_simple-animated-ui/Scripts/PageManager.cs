using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleAnimatedUI
{
    public class PageManager : MonoBehaviour
    {
        public static PageManager Instance;

        [SerializeField] private List<Page> pages;
        [SerializeField] private PageEnum startPage;

        private Dictionary<PageEnum, Page> pageMap;
        private Page currentPage;

        private Sequence mainSequence;


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

                if (!pageMap.ContainsKey(page.PageID))
                    pageMap.Add(page.PageID, page);
                else
                    Debug.LogError($"Duplicate PageID: {page.PageID}");
            }
        }

        private void Start()
        {
            OpenPage(startPage);
        }



        public void OpenPage(PageEnum pageID)
        {
            if (!pageMap.TryGetValue(pageID, out var nextPage))
            {
                Debug.LogWarning($"Page {pageID} not found");
                return;
            }

            if (currentPage == nextPage)
                return;


            if (mainSequence != null && mainSequence.IsActive())
            {
                mainSequence.Kill();
            }

            mainSequence = DOTween.Sequence();

            if (currentPage != null)
            {
                mainSequence.Append(GetOutAnimationSequence(currentPage));

                mainSequence.AppendCallback(() =>
                {
                    SetPageActive(currentPage, false);
                    SetPageActive(nextPage, true);
                    currentPage = nextPage;
                });

                mainSequence.Append(GetInAnimationSequence(nextPage));
            }
            else
            {
                SetPageActive(nextPage, true);
                currentPage = nextPage;
                mainSequence.Append(GetInAnimationSequence(nextPage));
            }

            mainSequence.Play();
        }


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


        private Sequence GetOutAnimationSequence(Page page)
        {
            Sequence outSequence = DOTween.Sequence();

            if (page == null) return outSequence;

            ImageAnimator[] headerAnimations = page.Header.GetComponentsInChildren<ImageAnimator>();
            ImageAnimator[] bodyAnimations = page.Body.GetComponentsInChildren<ImageAnimator>();

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

            //outSequence.Join(page)

            return outSequence;
        }

        private Sequence GetInAnimationSequence(Page page)
        {
            Sequence inSequence = DOTween.Sequence();

            if (page == null) return inSequence;

            ImageAnimator[] headerAnimations = page.Header.GetComponentsInChildren<ImageAnimator>();
            ImageAnimator[] bodyAnimations = page.Body.GetComponentsInChildren<ImageAnimator>();

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

            return inSequence;
        }
    }
}
