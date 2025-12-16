using UnityEngine;
using UnityEngine.UI;

namespace DTT.PaginatedScrollView.AssistingUI
{
    /// <summary>
    /// Shows the current selected page.
    /// </summary>
    public class PageNumber : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="PaginatedScrollRect"/> that the current selected page is shown for.
        /// </summary>
        public PaginatedScrollRect PaginatedScrollRect
        {
            get => _paginatedScrollRect;
            set
            {
                if (_paginatedScrollRect)
                    _paginatedScrollRect.OnSelectedPageChanged -= UpdateText;
                _paginatedScrollRect = value;
                _paginatedScrollRect.OnSelectedPageChanged += UpdateText;
            }
        }

        /// <summary>
        /// Offset for the page number.
        /// </summary>
        [Header("Display Options")]
        [SerializeField]
        private int _indexOffset = 1;
        
        /// <summary>
        /// Page number display option.
        /// </summary>
        [SerializeField]
        private PageNumberOptions _displayOptions;

        /// <summary>
        /// String that is placed between the page number and number of pages.
        /// </summary>
        [SerializeField]
        private string _inBetweenText = "/";

        /// <summary>
        /// The text that holds the currently shown page number.
        /// </summary>
        [Header("UI")]
        [SerializeField]
        private Text _text;

        /// <summary>
        /// The <see cref="PaginatedScrollRect"/> that the current selected page is shown for.
        /// </summary>
        [SerializeField]
        private PaginatedScrollRect _paginatedScrollRect;

        /// <summary>
        /// Subscribes the <see cref="UpdateText(int)"/> method to the <see cref="PaginatedScrollRect.onSnapChange"/> event.
        /// </summary>
        private void OnEnable()
        {
            if (_paginatedScrollRect)
                _paginatedScrollRect.OnSelectedPageChanged += UpdateText;
        }

        /// <summary>
        /// Unsubscribes the <see cref="UpdateText(int)"/> method from the <see cref="PaginatedScrollRect.onSnapChange"/> event.
        /// </summary>
        private void OnDisable()
        {
            if (_paginatedScrollRect)
                _paginatedScrollRect.OnSelectedPageChanged -= UpdateText;
        }

        /// <summary>
        /// Sets the text of the text component if added.
        /// </summary>
        private void Start() => UpdateText(_paginatedScrollRect.IndexToSnapTowards);

        /// <summary>
        /// When the user makes changes to this component it will update the text component if added.
        /// </summary>
        private void OnValidate() => UpdateText(_paginatedScrollRect.IndexToSnapTowards);

        /// <summary>
        /// Sets the text of the text component if added.
        /// </summary>
        /// <param name="index">The page number.</param>
        private void UpdateText(int index)
        {
            if (!_paginatedScrollRect || !_text || !_text.gameObject.activeInHierarchy)
                return;

            int pageAmount = _paginatedScrollRect.PaginatedElementsCount;

            switch (_displayOptions)
            {
                case PageNumberOptions.PAGE_NUMBER_AND_NUMBER_OF_PAGES:
                {
                    _text.text = string.Format("{0}{1}{2}", (index + _indexOffset), _inBetweenText, pageAmount);
                    break;
                }
                case PageNumberOptions.PAGE_NUMBER_ONLY:
                {
                    _text.text = string.Format("{0}", (index + _indexOffset));
                    break;
                }
                case PageNumberOptions.NUMBER_OF_PAGES_ONLY:
                {
                    _text.text = string.Format("{0}", pageAmount);
                    break;
                }
                default:
                {
                    _text.text = "";
                    break;
                }
            }
        }
    }
}
