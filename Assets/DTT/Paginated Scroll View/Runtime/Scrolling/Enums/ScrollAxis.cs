using UnityEngine;

namespace DTT.PaginatedScrollView
{
    /// <summary>
    /// On which axis the user can scroll.
    /// </summary>
    public enum ScrollAxis
    {
        /// <summary>
        /// Horizontal axis.
        /// </summary>
        [InspectorName("Horizontal")]
        HORIZONTAL = 0,

        /// <summary>
        /// Vertical axis.
        /// </summary>
        [InspectorName("Vertical")]
        VERTICAL = 1
    }
}
