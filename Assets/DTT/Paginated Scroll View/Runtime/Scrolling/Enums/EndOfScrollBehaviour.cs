using UnityEngine;

namespace DTT.PaginatedScrollView
{
    /// <summary>
    /// Determines what should happen when the user reaches the beginning or end of the scrollview.
    /// </summary>
    public enum EndOfScrollBehaviour
    {
        /// <summary>
        /// The user cannot drag furher than the first or last page.
        /// </summary>
        [InspectorName("Stop")]
        STOP = 0,

        /// <summary>
        /// The user can drag furher than the first or last page.
        /// </summary>
        [InspectorName("Elastic")]
        ELASTIC = 1
    }
}
