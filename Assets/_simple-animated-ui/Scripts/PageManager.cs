using System.Collections.Generic;
using UnityEngine;

namespace SimpleAnimatedUI
{
    public class PageManager : MonoBehaviour
    {
        public static PageManager Instance;

        [SerializeField] private List<Page> pages;
        [SerializeField] private PageEnum startPage;

        private Dictionary<PageEnum, Page> pageMap;
        private Page currentPage;

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

            if (currentPage != null)
                SetPageActive(currentPage, false);

            SetPageActive(nextPage, true);
            currentPage = nextPage;
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

            // если CanvasGroup нужен для кликов / альфы
            group.alpha = active ? 1f : 0f;
            group.interactable = active;
            group.blocksRaycasts = active;
        }
    }
}
