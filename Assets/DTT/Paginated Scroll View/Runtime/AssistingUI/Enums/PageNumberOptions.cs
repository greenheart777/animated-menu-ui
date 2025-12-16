using UnityEngine;

namespace DTT.PaginatedScrollView.AssistingUI
{
    /// <summary>
    /// Display options for the <see cref="PageNumber"/> class.
    /// </summary>
    public enum PageNumberOptions
    {
        /// <summary>
        /// Show the current page number and number of pages.
        /// </summary>
        [InspectorName("CurrentPageAndPageAmount")]
        PAGE_NUMBER_AND_NUMBER_OF_PAGES = 0,

        /// <summary>
        /// Only show the current page number.
        /// </summary>
        [InspectorName("CurrentPageNumberOnly")]
        PAGE_NUMBER_ONLY = 1,

        /// <summary>
        /// Only show the number of pages.
        /// </summary>
        [InspectorName("NumberOfPagesOnly")]
        NUMBER_OF_PAGES_ONLY = 2
    }
}
