using UnityEditor;
using DTT.Utils.EditorUtilities;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// Property cache for the <see cref="PaginatedScrollRect"/>
    /// </summary>
    public class PaginatedScrollRectSerializedPropertyCache : SerializedPropertyCache
    {
        /// <summary>
        /// How the snapping should behave.
        /// </summary>
        public SerializedProperty SnapType => base["_snapType"];

        /// <summary>
        /// To which closest page should be snapped.
        /// </summary>
        public SerializedProperty SnapTowards => base["_snapTowards"];

        /// <summary>
        /// The behaviour of the scroll rect when one of the ends is reached.
        /// </summary>
        public SerializedProperty EndOfScrollBehaviour => base["_endOfScrollBehaviour"];

        /// <summary>
        /// Which scroll axis should be used.
        /// </summary>
        public SerializedProperty ScrollAxis => base["_scrollAxis"];

        /// <summary>
        /// How fast a page should be snapped towards.
        /// </summary>
        public SerializedProperty SnapSpeed => base["_snapSpeed"];

        /// <summary>
        /// What the minimum distance between the snapping point and the snapped page should be.
        /// </summary>
        public SerializedProperty MinimumSnapDistance => base["_minimumSnapDistance"];

        /// <summary>
        /// True is the user is currenty dragging.
        /// </summary>
        public SerializedProperty Dragging => base["_dragging"];

        /// <summary>
        /// True if the desired page has reached the snapping point.
        /// </summary>
        public SerializedProperty SnappedPageReachedDestination => base["_snappedPageReachedDestination"];

        /// <summary>
        /// The previous position of the content rect used to determine the velocity.
        /// </summary>
        public SerializedProperty PreviousContentPosition => base["_previousContentPosition"];

        /// <summary>
        /// Spacing between elements in the layout group component in the content game object.
        /// </summary>
        public SerializedProperty LayoutElementSpacing => base["_layoutElementSpacing"];

        /// <summary>
        /// Reference towards the layoutgroup component of the content.
        /// </summary>
        public SerializedProperty HorizontalOrVerticalLayoutGroup => base["_horizontalOrVerticalLayoutGroup"];

        /// <summary>
        /// Reference towards the content.
        /// </summary>
        public SerializedProperty Content => base["_content"];

        /// <summary>
        /// Reference towards the viewport.
        /// </summary>
        public SerializedProperty Viewport => base["_viewport"];

        /// <summary>
        /// The button to go to the previous page.
        /// </summary>
        public SerializedProperty PreviousButton => base["_previousButton"];

        /// <summary>
        /// The button to go to the next page.
        /// </summary>
        public SerializedProperty NextButton => base["_nextButton"];

        public SerializedProperty PaginatedElements => base["_paginatedElements"];

        /// <summary>
        /// When the desired page to snap towards changes.
        /// </summary>
        public SerializedProperty OnSnapChange => base["_onSnapChange"];

        /// <summary>
        /// When the desired page arrives at the snapping point.
        /// </summary>
        public SerializedProperty OnPageArrivedAtSnappingPoint => base["_onPageArrivedAtSnappingPoint"];

        /// <summary>
        /// Explicit constructor to call base.
        /// </summary>
        /// <param name="serializedObject">The object to gain the serialized properties from.</param>
        public PaginatedScrollRectSerializedPropertyCache(SerializedObject serializedObject) : base(serializedObject) { }
    }
}
