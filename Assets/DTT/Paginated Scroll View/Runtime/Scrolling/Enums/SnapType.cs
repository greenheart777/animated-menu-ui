using UnityEngine;

namespace DTT.PaginatedScrollView
{
    /// <summary>
    /// How the paginated scroll view should snap towards the closest page.
    /// </summary>
    public enum SnapType
    {
        /// <summary>
        /// Snap to the selected page with a smooth animation.
        /// </summary>
        [InspectorName("Elastic")]
        ELASTIC = 0,

        /// <summary>
        /// Teleport to the selected page.
        /// </summary>
        [InspectorName("Clamped")]
        CLAMPED = 1
    }
}
