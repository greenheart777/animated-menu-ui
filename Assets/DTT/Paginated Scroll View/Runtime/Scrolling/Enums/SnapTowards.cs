using UnityEngine;

namespace DTT.PaginatedScrollView
{
    /// <summary>
    /// To which page should paginated view be snapped when the user stops dragging.
    /// </summary>
    public enum SnapTowards
    {
        /// <summary>
        /// Snap to the closest page.
        /// </summary>
        [InspectorName("Closest")]
        CLOSEST = 0,

        /// <summary>
        /// Snap to the closest previous page.
        /// </summary>
        [InspectorName("Previous")]
        PREVIOUS = 1,

        /// <summary>
        /// Snap to the closest next page.
        /// </summary>
        [InspectorName("Next")]
        NEXT = 2
    }
}
