using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DTT.PaginatedScrollView
{
    /// <summary>
    /// Component to add paginated scroll effect to a viewport.
    /// </summary>
    public class PaginatedScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        /// <summary>
        /// How the snapping should behave.
        /// </summary>
        public SnapType SnapType
        {
            get => _snapType;
            set => _snapType = value;
        }

        /// <summary>
        /// To which closest page should be snapped.
        /// </summary>
        public SnapTowards SnapTowards
        {
            get => _snapTowards;
            set => _snapTowards = value;
        }

        /// <summary>
        /// Which scroll axis should be used.
        /// </summary>
        public ScrollAxis ScrollAxis
        {
            get => _scrollAxis;
            set => _scrollAxis = value;
        }

        /// <summary>
        /// The behaviour of the scroll rect when one of the ends is reached.
        /// </summary>
        public EndOfScrollBehaviour EndOfScrollBehaviour
        {
            get => _endOfScrollBehaviour;
            set => _endOfScrollBehaviour = value;
        }

        /// <summary>
        /// <para>
        /// How fast a page should be snapped towards.
        /// </para>
        /// <para>
        /// Set value is clamped between 0.001f (included) and 100f (included).
        /// </para>
        /// </summary>
        public float SnapSpeed
        {
            get => _snapSpeed;
            set => _snapSpeed = Mathf.Clamp(value, 1f, 100f);
        }

        /// <summary>
        /// What the minimum distance between the snapping point and the snapped page should be.
        /// </summary>
        public float MinimumSnapDistance
        {
            get => _minimumSnapDistance;
            set => _minimumSnapDistance = value;
        }

        /// <summary>
        /// Which page to snap towards.
        /// </summary>
        public int IndexToSnapTowards
        {
            get => _selectedPageIndex;
            set => _selectedPageIndex = Mathf.Clamp(value, 0, PaginatedElementsCount - 1);
        }

        /// <summary>
        /// True is the user is currenty dragging.
        /// </summary>
        public bool Dragging => _dragging;

        /// <summary>
        /// True if the desired page has reached the snapping point.
        /// </summary>
        public bool SnappedPageReachedDestination => _snappedPageReachedDestination;

        /// <summary>
        /// The previous position of the content rect used to determine the velocity.
        /// </summary>
        public Vector2 PreviousContentPosition => _previousContentPosition;

        /// <summary>
        /// Reference towards the layoutgroup component of the content.
        /// </summary>
        public HorizontalOrVerticalLayoutGroup HorizontalOrVerticalLayoutGroup
        {
            get => _horizontalOrVerticalLayoutGroup;
            set => _horizontalOrVerticalLayoutGroup = value;
        }

        /// <summary>
        /// Reference towards the content.
        /// </summary>
        public RectTransform Content
        {
            get => _content;
            set => _content = value;
        }

        /// <summary>
        /// Reference towards the viewport.
        /// </summary>
        public RectTransform ViewPort
        {
            get => _viewport;
            set => _viewport = value;
        }

        /// <summary>
        /// The button to go to the next page.
        /// </summary>
        public Button NextButton
        {
            get => _nextButton;
            set
            {
                if (_nextButton)
                    _nextButton.onClick.RemoveListener(NextPage);
                _nextButton = value;
                _nextButton.onClick.AddListener(NextPage);
            }
        }

        /// <summary>
        /// The button to go to the previous page.
        /// </summary>
        public Button PreviousButton
        {
            get => _previousButton;
            set
            {
                if (_previousButton)
                    _previousButton.onClick.RemoveListener(PreviousPage);
                _previousButton = value;
                _previousButton.onClick.AddListener(NextPage);
            }
        }

        /// <summary>
        /// Get a <see cref="PaginatedElement"/> at index i.
        /// </summary>
        /// <param name="i">The index of the <see cref="PaginatedElement"/>. Must be smaller than <see cref="PaginatedElementsCount"/></param>
        /// <returns>A <see cref="PaginatedElement"/> at index i.</returns>
        public PaginatedElement this[int i] => _paginatedElements[i];

        /// <summary>
        /// The number of <see cref="PaginatedElement"/>s.
        /// </summary>
        public int PaginatedElementsCount => _paginatedElements.Count;

        /// <summary>
        /// How the snapping should behave.
        /// </summary>
        [SerializeField]
        private SnapType _snapType;

        /// <summary>
        /// To which closest page should be snapped.
        /// </summary>
        [SerializeField]
        private SnapTowards _snapTowards;

        /// <summary>
        /// The behaviour of the scroll rect when one of the ends is reached.
        /// </summary>
        [SerializeField]
        private EndOfScrollBehaviour _endOfScrollBehaviour;

        /// <summary>
        /// Which scroll axis should be used.
        /// </summary>
        [SerializeField]
        private ScrollAxis _scrollAxis;

        /// <summary>
        /// How fast a page should be snapped towards.
        /// </summary>
        [SerializeField]
        [Range(0f, 100f)]
        private float _snapSpeed;

        /// <summary>
        /// What the minimum distance between the snapping point and the snapped page should be.
        /// </summary>
        [SerializeField]
        private float _minimumSnapDistance = 0.01f;

        /// <summary>
        /// Index value of the current selected page.
        /// </summary>
        private int _selectedPageIndex;

        /// <summary>
        /// True is the user is currenty dragging.
        /// </summary>
        [SerializeField]
        private bool _dragging;

        /// <summary>
        /// True if the desired page has reached the snapping point.
        /// </summary>
        [SerializeField]
        private bool _snappedPageReachedDestination = false;

        /// <summary>
        /// The previous position of the content rect used to determine the velocity.
        /// </summary>
        [SerializeField]
        private Vector2 _previousContentPosition;

        /// <summary>
        /// Spacing between elements in the layout group component in the content game object.
        /// </summary>
        [SerializeField]
        private float _layoutElementSpacing;

        /// <summary>
        /// Reference towards the layoutgroup component of the content.
        /// </summary>
        [SerializeField]
        private HorizontalOrVerticalLayoutGroup _horizontalOrVerticalLayoutGroup;

        /// <summary>
        /// Reference towards the content.
        /// </summary>
        [SerializeField]
        private RectTransform _content;

        /// <summary>
        /// Reference towards the viewport.
        /// </summary>
        [SerializeField]
        private RectTransform _viewport;

        /// <summary>
        /// The button to go to the next page.
        /// </summary>
        [SerializeField]
        private Button _nextButton;

        /// <summary>
        /// The button to go to the previous page.
        /// </summary>
        [SerializeField]
        private Button _previousButton;

        /// <summary>
        /// The paginated elements that contains a reference to the rect and holds the distances to the viewport rect.
        /// </summary>
        [SerializeField]
        private List<PaginatedElement> _paginatedElements = new List<PaginatedElement>();

        /// <summary>
        /// When the desired page to snap towards changes.
        /// </summary>
        public event Action<int> OnSelectedPageChanged;

        /// <summary>
        /// When the amount of pages changes.
        /// </summary>
        public event Action<int> OnPageAmountChanged;

        /// <summary>
        /// When the desired page arrives at the snapping point.
        /// </summary>
        public event Action onPageArrivedAtSnappingPoint;

        /// <summary>
        /// Cache the previous used scrolling axis, neede for checking if scrolling axis has been changed during runtime.
        /// </summary>
        private ScrollAxis _previousScrollAxis;

        /// <summary>
        /// Subscribes listeners to the previous and next button's onClick events.
        /// </summary>
        private void OnEnable()
        {
            if (_previousButton)
                _previousButton.onClick.AddListener(PreviousPage);
            if (_nextButton)
                _nextButton.onClick.AddListener(NextPage);
        }

        /// <summary>
        /// Unsubscribes listeners from the previous and next button's onClick events.
        /// </summary>
        private void OnDisable()
        {
            if (_previousButton)
                _previousButton.onClick.RemoveListener(PreviousPage);
            if (_nextButton)
                _nextButton.onClick.RemoveListener(NextPage);
        }

        /// <summary>
        /// Checks if there are changes in the amount of pages and updates the pagedots.
        /// </summary>
        private void OnValidate()
        {
            GetChildren();
            SelectedPageChanged();

            if (_minimumSnapDistance <= 0)
            {
                Debug.LogWarning("MinimumSnapDistance must be greater than zero. The value is changed to 0.1f");
                _minimumSnapDistance = 0.1f;
            }
        }

        /// <summary>
        /// Retrieves the layout group and initializes the <see cref="_paginatedElements"/> and the pagedots.
        /// </summary>
        private IEnumerator Start()
        {
            _previousScrollAxis = _scrollAxis;
            _horizontalOrVerticalLayoutGroup = _content.GetComponent<HorizontalOrVerticalLayoutGroup>();

            if (_horizontalOrVerticalLayoutGroup == null)
                if (_scrollAxis == ScrollAxis.HORIZONTAL)
                    _horizontalOrVerticalLayoutGroup = _content.gameObject.AddComponent<HorizontalLayoutGroup>();
                else if (_scrollAxis == ScrollAxis.VERTICAL)
                    _horizontalOrVerticalLayoutGroup = _content.gameObject.AddComponent<VerticalLayoutGroup>();

            _horizontalOrVerticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            _horizontalOrVerticalLayoutGroup.spacing = _layoutElementSpacing;
            _horizontalOrVerticalLayoutGroup.childControlHeight = false;
            _horizontalOrVerticalLayoutGroup.childControlWidth = false;

            // Waits for UI to update on start.
            yield return null;

            GetChildren();
            SelectedPageChanged();
            _previousContentPosition = _content.anchoredPosition;

            GotoPage(0);
        }

        /// <summary>
        /// Move the content rect when the user is not dragging.
        /// </summary>
        private void Update()
        {
            if (!_dragging)
            {
                UpdateDistances();
                MoveToCenter();
            }

            if (_previousScrollAxis != _scrollAxis)
            {
                _horizontalOrVerticalLayoutGroup = _horizontalOrVerticalLayoutGroup.SwitchBetweenHorizontalAndVerticalLayoutGroup();
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_horizontalOrVerticalLayoutGroup.transform);
                _content.anchoredPosition = Vector2.zero;                
                GotoPage(_selectedPageIndex);
                _previousScrollAxis = _scrollAxis;
            }
        }

        /// <summary>
        /// Called when the user starts dragging.
        /// </summary>
        /// <param name="eventData">Not used in this method.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
            _snappedPageReachedDestination = false;
        }

        /// <summary>
        /// Called when the user finished dragging.
        /// </summary>
        /// <param name="eventData">Not used in this method.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
            _selectedPageIndex = GetClosestPaginatedElement();
            SelectedPageChanged();
        }

        /// <summary>
        /// Called when the user drags the scroll view.
        /// </summary>
        /// <param name="eventData">Data about the drag event containing the drag delta.</param>
        public void OnDrag(PointerEventData eventData)
        {
            UpdateDistances();

            // The drag delta. How much the mouse has moved since the last frame.
            Vector2 delta = eventData.delta;

            _previousContentPosition = _content.anchoredPosition;

            // Positioning.
            Vector2 newPosition = _content.anchoredPosition;

            // Clamp if EndOfScrollBehaviour is stop.
            if (_endOfScrollBehaviour == EndOfScrollBehaviour.STOP)
            {
                if (_scrollAxis == ScrollAxis.HORIZONTAL)
                {
                    // Min x position for content rect (Last Element).
                    float minPosition = -_content.sizeDelta.x / 2 - 5f;
                    // Max x position for content rect (First Element).
                    float maxPosition = _content.sizeDelta.x / 2 + 5f;

                    // User dragged past max x position.
                    if (_content.anchoredPosition.x >= maxPosition && delta.x > 0)
                    {
                        newPosition.x = maxPosition;
                        _content.anchoredPosition = newPosition;
                        return;
                    }
                    // User dragged past min y position.
                    else if (_content.anchoredPosition.x <= minPosition && delta.x < 0)
                    {
                        newPosition.x = minPosition;
                        _content.anchoredPosition = newPosition;
                        return;
                    }
                }
                else if (_scrollAxis == ScrollAxis.VERTICAL)
                {
                    // Min x position for content rect (First Element).
                    float minPosition = _content.sizeDelta.y / 2 + 5f;
                    // Max x position for content rect (Last Element).
                    float maxPosition = -_content.sizeDelta.y / 2 - 5f;

                    // User dragged past max y position.
                    if (_content.anchoredPosition.y <= maxPosition && delta.y < 0)
                    {
                        newPosition.y = maxPosition;
                        _content.anchoredPosition = newPosition;
                        return;
                    }
                    // User dragged past min y position.
                    else if (_content.anchoredPosition.y >= minPosition && delta.y > 0)
                    {
                        newPosition.y = minPosition;
                        _content.anchoredPosition = newPosition;
                        return;
                    }
                }
            }

            // Settting the new position based on the drag delta.
            newPosition.x =
                _scrollAxis == ScrollAxis.HORIZONTAL ?
                _content.anchoredPosition.x + delta.x :
                _content.anchoredPosition.x;
            newPosition.y =
                _scrollAxis == ScrollAxis.VERTICAL ?
                _content.anchoredPosition.y + delta.y :
                _content.anchoredPosition.y;

            _content.anchoredPosition = newPosition;
        }

        /// <summary>
        /// Go to the previous page.
        /// </summary>
        public void PreviousPage()
        {
            if (_selectedPageIndex > 0)
            {
                _selectedPageIndex--;
                PageChanged();
            }
        }

        /// <summary>
        /// Go to the next page.
        /// </summary>
        public void NextPage()
        {
            if (_paginatedElements.Count - 1 > _selectedPageIndex)
            {
                _selectedPageIndex++;
                PageChanged();
            }
        }

        /// <summary>
        /// Go to a specific page.
        /// </summary>
        /// <param name="pageIndex">The index of the page. Must be 0 and smaller than the amount of pages.</param>
        public void GotoPage(int pageIndex)
        {
            if (pageIndex >= 0 || pageIndex < _paginatedElements.Count)
            {
                _selectedPageIndex = pageIndex;
                PageChanged();
            }
        }

        /// <summary>
        /// Updates the pagedots and sets the <see cref="_snappedPageReachedDestination"/> when the desired page has changed.
        /// </summary>
        private void PageChanged()
        {
            _snappedPageReachedDestination = false;
            SelectedPageChanged();
        }

        /// <summary>
        /// Returns the <see cref="PaginatedElement"/> which is closest to the viewport rect.
        /// </summary>
        /// <returns></returns>
        private int GetClosestPaginatedElement()
        {
            float smallestDistance = float.MaxValue;
            int index = -1;
            float distance = float.MaxValue;

            for (int i = 0; i < _paginatedElements.Count; i++)
            {
                switch (_scrollAxis)
                {
                    case ScrollAxis.HORIZONTAL:
                    distance = Mathf.Abs(_paginatedElements[i].distanceToCenter.x);
                    break;
                    case ScrollAxis.VERTICAL:
                    distance = Mathf.Abs(_paginatedElements[i].distanceToCenter.y);
                    break;
                }

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Updates the page dots so that they are displaying which page is selected.
        /// </summary>
        private void SelectedPageChanged() => OnSelectedPageChanged?.Invoke(_selectedPageIndex);
        
        /// <summary>
        /// Gets all children of the content rect and add these to the <see cref="_paginatedElements"/> which is cleared first.
        /// </summary>
        private void GetChildren()
        {
            _paginatedElements.Clear();

            if (!_content)
                return;

            for (int i = 0; i < _content.childCount; i++)
            {
                RectTransform childTransform = (RectTransform)_content.GetChild(i);

                _paginatedElements.Add(new PaginatedElement(childTransform, Vector2.zero));
            }

            UpdateDistances();

            OnPageAmountChanged?.Invoke(_paginatedElements.Count);
        }

        /// <summary>
        /// Updates the distances of the <see cref="PaginatedElement.distanceToCenter"/>.
        /// </summary>
        private void UpdateDistances()
        {
            for (int i = 0; i < _paginatedElements.Count; i++)
            {
                Vector2 distance = transform.localPosition - transform.InverseTransformPoint(_paginatedElements[i].rectTransform.position);

                _paginatedElements[i] =
                    new PaginatedElement(
                        _paginatedElements[i].rectTransform,
                        distance);
            }
        }

        /// <summary>
        /// Checks if the given page is within the minimum snap distance.
        /// </summary>
        /// <param name="pageIndex">Which page needs to be checked. Same as the child index of the page.</param>
        /// <param name="scrollAxis">Which axis.</param>
        /// <returns>Weither the give page is within the minmum snap distance.</returns>
        private bool PaginatedElementWithinMinimumSnapDistance(int pageIndex, ScrollAxis scrollAxis)
        {
            switch (scrollAxis)
            {
                case ScrollAxis.HORIZONTAL: return Mathf.Abs(_paginatedElements[pageIndex].distanceToCenter.x) < _minimumSnapDistance;
                case ScrollAxis.VERTICAL: return Mathf.Abs(_paginatedElements[pageIndex].distanceToCenter.y) < _minimumSnapDistance;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Snaps the content rect to a specific page based on the configuration of this script.
        /// </summary>
        private void MoveToCenter()
        {
            // If the content rect didn't snap to the desired page yet.
            if (!_snappedPageReachedDestination)
            {
                if (!PaginatedElementWithinMinimumSnapDistance(_selectedPageIndex, _scrollAxis))
                {
                    Vector2 newPosition = new Vector2();

                    switch (_snapType)
                    {
                        case SnapType.ELASTIC:
                        {
                            newPosition =
                                Vector2.Lerp(
                                    _content.anchoredPosition,
                                    _content.anchoredPosition +
                                    new Vector2(
                                        _scrollAxis == ScrollAxis.HORIZONTAL ?
                                        GetPaginatedElementToSnapTowards().distanceToCenter.x :
                                        0f,
                                        _scrollAxis == ScrollAxis.VERTICAL ?
                                        GetPaginatedElementToSnapTowards().distanceToCenter.y :
                                        0f),
                                    Time.deltaTime * _snapSpeed);
                            break;
                        }
                        case SnapType.CLAMPED:
                        {
                            newPosition =
                                _content.anchoredPosition +
                                new Vector2(
                                    _scrollAxis == ScrollAxis.HORIZONTAL ?
                                    GetPaginatedElementToSnapTowards().distanceToCenter.x :
                                    0f,
                                    _scrollAxis == ScrollAxis.VERTICAL ?
                                    GetPaginatedElementToSnapTowards().distanceToCenter.y :
                                    0f);
                            break;
                        }
                    }

                    _content.anchoredPosition = newPosition;
                }
                else
                {
                    _snappedPageReachedDestination = true;
                    onPageArrivedAtSnappingPoint?.Invoke();
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="PaginatedElement"/> that the content rect needs to snap towards depending on the configuration of this script.
        /// </summary>
        /// <returns>The <see cref="PaginatedElement"/> to snap towards.</returns>
        private PaginatedElement GetPaginatedElementToSnapTowards()
        {
            switch (_snapTowards)
            {
                case SnapTowards.PREVIOUS:
                {
                    if (_paginatedElements[_selectedPageIndex].distanceToCenter.x < 0 && _selectedPageIndex > 0)
                        _selectedPageIndex--;
                    break;
                }
                case SnapTowards.NEXT:
                {
                    if (_paginatedElements[_selectedPageIndex].distanceToCenter.x > 0 && _selectedPageIndex < _paginatedElements.Count - 1)
                        _selectedPageIndex++;
                    break;
                }
            }

            return _paginatedElements[_selectedPageIndex];
        }
    }
}
