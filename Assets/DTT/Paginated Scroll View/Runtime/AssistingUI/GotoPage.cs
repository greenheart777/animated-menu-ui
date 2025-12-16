using UnityEngine;
using UnityEngine.UI;

namespace DTT.PaginatedScrollView
{
    /// <summary>
    /// Behaviour for go to page button accompanied by an input field.
    /// </summary>
    public class GotoPage : MonoBehaviour
    {
        /// <summary>
        /// The input field where the user enters the desired page.
        /// </summary>
        [SerializeField]
        private InputField _input;

        /// <summary>
        /// The paginated scroll rect to talk to.
        /// </summary>
        [SerializeField]
        private PaginatedScrollRect _paginatedScrollRect;

        /// <summary>
        /// Sets a desired page.
        /// </summary>
        public void SetPage()
        {
            // Check if the input field and paginated scrollview references are set.
            if (_input == null || _paginatedScrollRect == null)
                return;

            // Check if the input field has a value.
            if (string.IsNullOrEmpty(_input.text))
                return;

            // Set the default index value.
            int index = 0;

            // Parse the index value from the input field to an integer.
            if (int.TryParse(_input.text, out int parsedIndex))
                index = parsedIndex;

            // Verify the int is within a valid range.
            index = Mathf.Clamp(index - 1, 0, _paginatedScrollRect.PaginatedElementsCount - 1);

            // Set the input field's text to the verified index.
            _input.text = $"{index + 1}";

            // Move the paginated scrollview to the given index.
            _paginatedScrollRect.GotoPage(index);
        }
    }
}