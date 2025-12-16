using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.PaginatedScrollView.AssistingUI
{
    /// <summary>
    /// Manages the <see cref="PageDot"/>s for a <see cref="DTT.PaginatedScrollView.PaginatedScrollRect"/>.
    /// </summary>
    public class PageDotManager : MonoBehaviour
    {
        /// <summary>
        /// Offset for the page number.
        /// </summary>
        [SerializeField]
        private int _indexOffset = 1;

        /// <summary>
        /// The <see cref="PaginatedScrollRect"/> to create the <see cref="PageDot"/>s for.
        /// </summary>
        public PaginatedScrollRect PaginatedScrollRect
        {
            get => _paginatedScrollRect;
            set => _paginatedScrollRect = value;
        }

        /// <summary>
        /// The prefab to use to create the <see cref="PageDot"/>s.
        /// </summary>
        public GameObject PageDotPrefab
        {
            get => _pageDotPrefab;
            set => _pageDotPrefab = value;
        }

        /// <summary>
        /// The <see cref="PaginatedScrollRect"/> to create the <see cref="PageDot"/>s for.
        /// </summary>
        [SerializeField]
        private PaginatedScrollRect _paginatedScrollRect;

        /// <summary>
        /// The prefab to use to create the <see cref="PageDot"/>s.
        /// </summary>
        [SerializeField]
        private GameObject _pageDotPrefab;

        /// <summary>
        /// All <see cref="PageDot"/>s that are created by this script.
        /// </summary>
        [SerializeField]
        private List<PageDot> _pageDots = new List<PageDot>();

        /// <summary>
        /// Set index offset for page buttons.
        /// </summary>
        private void OnValidate() => SetPageDotsIndex();

        /// <summary>
        /// Creates <see cref="PageDot"/>s based on the current amount of pages in the scroll view.
        /// </summary>
        private void Awake() => Initialize();

        /// <summary>
        /// Subscribes the event listeners.
        /// </summary>
        private void OnEnable()
        {
            _paginatedScrollRect.OnSelectedPageChanged += Toggle;
            _paginatedScrollRect.OnPageAmountChanged += updatePageDots;

            foreach (PageDot pageDot in _pageDots)
                if (pageDot.Button)
                    pageDot.OnClick += ScrollToPage;
        }

        /// <summary>
        /// Unsubscribes the event listeners.
        /// </summary>
        private void OnDisable()
        {
            _paginatedScrollRect.OnSelectedPageChanged -= Toggle;
            _paginatedScrollRect.OnPageAmountChanged -= updatePageDots;

            foreach (PageDot pageDot in _pageDots)
                if (pageDot.Button)
                    pageDot.OnClick -= ScrollToPage;
        }

        /// <summary>
        /// Creates Page Dots based on the <see cref="_pageDotPrefab"/>.
        /// </summary>
        public void Initialize()
        {
            int amountOfPages = _paginatedScrollRect.PaginatedElementsCount;
            int amountOfDots = _pageDots.Count;

            RemovePageDots(amountOfDots);

            if (amountOfPages == 0)
                return;

            AddPageDots(amountOfPages);

            SetPageDotsIndex();
        }

        /// <summary>
        /// Adds or removes <see cref="PageDot"/>s based on the current amount and amount needed.
        /// </summary>
        /// <param name="pageDotsNeeded">Them amount of pages to have <see cref="PageDot"/>s for.</param>
        private void updatePageDots(int pageDotsNeeded)
        {
            int amountOfDots = _pageDots.Count;
            if (amountOfDots == pageDotsNeeded)
                return;

            if (amountOfDots < pageDotsNeeded)
                AddPageDots(pageDotsNeeded - amountOfDots);

            if (amountOfDots > pageDotsNeeded)
                RemovePageDots(amountOfDots - pageDotsNeeded);

            SetPageDotsIndex();
        }

        /// <summary>
        /// Add new Page Dots based on the <see cref="_pageDotPrefab"/>.
        /// </summary>
        /// <param name="amountToAdd"></param>
        /// <exception cref="Exception">Amount to add.</exception>
        private void AddPageDots(int amountToAdd)
        {
            PageDot pageDot;

            for (int i = 0; i < amountToAdd; i++)
            {
                try
                {
                    pageDot = Instantiate(_pageDotPrefab, this.transform, false).GetComponent<PageDot>();
                }
                catch
                {
                    throw new Exception("The referenced prefab does not contain a component that inherits from the PageDot class.");
                }

                pageDot.OnClick += ScrollToPage;
                _pageDots.Add(pageDot);
            }
        }

        /// <summary>
        /// Removes page dots.
        /// </summary>
        /// <param name="amountToRemove">Amount to remove.</param>
        private void RemovePageDots(int amountToRemove)
        {
            PageDot pageDot;

            for (int i = 0; i < amountToRemove; i++)
            {
                pageDot = _pageDots.Last();
                _pageDots.Remove(pageDot);
                pageDot.OnClick -= ScrollToPage;

#if UNITY_EDITOR
                DestroyImmediate(pageDot.gameObject);
#else
                Destroy(pageDot.gameObject);
#endif
            }
        }

        /// <summary>
        /// Sets the index of the page the dot should point to.
        /// </summary>
        private void SetPageDotsIndex()
        {
            for (int i = 0; i < _pageDots.Count; i++)
                _pageDots[i].SetIndex(i, _indexOffset);
        }

        /// <summary>
        /// Tells the <see cref="PaginatedScrollRect"/> to which page it should scroll.
        /// </summary>
        /// <param name="index">Which page the <see cref="PaginatedScrollRect"/> should scroll to.</param>
        private void ScrollToPage(int index)
        {
            if (_paginatedScrollRect)
                _paginatedScrollRect.GotoPage(index);
        }

        /// <summary>
        /// Sets the <see cref="PageDot"/> with the same index to on and every other <see cref="PageDot"/> to off.
        /// </summary>
        /// <param name="index">Which <see cref="PageDot"/> should be turned on.</param>
        private void Toggle(int index)
        {
            foreach (PageDot pageDot in _pageDots)
                pageDot.Toggle(index);
        }
    }
}
