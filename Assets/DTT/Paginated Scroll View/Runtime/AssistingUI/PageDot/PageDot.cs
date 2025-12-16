using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.PaginatedScrollView.AssistingUI
{
    /// <summary>
    /// Page dot that can toggle between two game objects.
    /// </summary>
    public class PageDot : MonoBehaviour
    {
        [Header("Navigation")]
        /// <summary>
        /// Button to go to the page that this <see cref="PageDot"/> represents.
        /// </summary>
        [SerializeField]
        private Button _button;

        /// <summary>
        /// The page number text this dot button represents. 
        /// </summary>
        [SerializeField]
        private Text _pageNumber;

        [Header("Selected")]
        /// <summary>
        /// The color of the button if <see cref="PageDot.IsSelected"/> is true. 
        /// </summary>
        [SerializeField, Tooltip("The color of the button if the dot is selected.")]
        private Color _selectedColor = Color.blue;

        /// <summary>
        /// The color of the text if <see cref="PageDot.IsSelected"/> is true. 
        /// </summary>
        [SerializeField, Tooltip("The color of the Text if the dot is selected.")]
        private Color _selectedTextColor = Color.white;

        [Header("Not Selected")]

        /// <summary>
        /// The color of the button if <see cref="PageDot.IsSelected"/> is false. 
        /// </summary>
        [SerializeField, Tooltip("The color of the button if the dot is not selected.")]
        private Color _defaultColor = Color.white;

        /// <summary>
        /// The color of the text if <see cref="PageDot.IsSelected"/> is false. 
        /// </summary>
        [SerializeField, Tooltip("The color of the Text if the dot is not selected.")]
        private Color _defaultTextColor = Color.black;

        /// <summary>
        /// Fires when the user clicks on the <see cref="Button"/>.
        /// </summary>
        public event Action<int> OnClick;

        /// <summary>
        /// Whether the dot is on or off.
        /// </summary>
        public bool IsSelected => _isSelected;

        /// <summary>
        /// Button to go to the page that this <see cref="PageDot"/> represents.
        /// </summary>
        public Button Button
        {
            get => _button;
            set
            {
                if (_button)
                    _button.onClick.RemoveListener(Clicked);
                _button = value;
                _button.onClick.AddListener(Clicked);
            }
        }

        /// <summary>
        /// The page index this dot represents.
        /// </summary>
        private int _index;

        /// <summary>
        /// Whether the dot is on or off.
        /// </summary>
        private bool _isSelected;

        /// <summary>
        /// Subscribe the <see cref="Clicked"/> method from the button's onClick event.
        /// </summary>
        private void Start()
        {
            if (_button != null)
                _button.onClick.AddListener(Clicked);
        }

        /// <summary>
        /// Subscribe the <see cref="Clicked"/> method from the button's onClick event.
        /// </summary>
        public virtual void OnEnable()
        {            
            if (_button != null) 
                _button.onClick.AddListener(Clicked);
        }

        /// <summary>
        /// Unsubscribe the <see cref="Clicked"/> method from the button's onClick event.
        /// </summary>
        private void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Clicked);
        }

        /// <summary>
        /// Unsubscribe the <see cref="Clicked"/> method from the button's onClick event.
        /// </summary>
        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Clicked);
        }

        /// <summary>
        /// Invokes the onClick action of this page dot.
        /// </summary>
        public void Clicked() => OnClick?.Invoke(_index);

        /// <summary>
        /// Set the index of this page dot.
        /// </summary>
        /// <param name="index">Index to set to.</param>
        /// <param name="indexOffset">Index value to be displayed</param>
        public void SetIndex(int index, int displayIndex)
        {
            _index = index;
            transform.name = String.Format("PageDot {0}", index + displayIndex);

            if (_pageNumber != null)
                _pageNumber.text = String.Format("{0}", index + displayIndex);
        }

        /// <summary>
        /// Toggle this page dot between on and off.
        /// </summary>
        /// <param name="index">Which page.</param>
        public virtual void Toggle(int index)
        {
            _isSelected = index == _index;

            if (_isSelected)
                Selected();
            else
                Deselected();
        }

        /// <summary>
        /// Sets the color of the <see cref="PageDot.Button"/> to the <see cref="_selectedColor"/> color.
        /// </summary>
        public void Selected()
        {
            _button.image.color = _selectedColor;
            _pageNumber.color = _selectedTextColor;
        }

        /// <summary>
        /// Sets the color of the <see cref="PageDot.Button"/> to the <see cref="_defaultColor"/> color.
        /// </summary>
        public void Deselected()
        {
            _button.image.color = _defaultColor;
            _pageNumber.color = _defaultTextColor;
        }
    }
}