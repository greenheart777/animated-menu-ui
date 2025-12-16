using UnityEditor;
using DTT.PaginatedScrollView.AssistingUI;
using DTT.Utils.EditorUtilities;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// Property cache for the <see cref="PageDotManager"/>
    /// </summary>
    public class PageDotManagerSerializedPropertyCache : SerializedPropertyCache
    {
        /// <summary>
        /// The paginated scroll rectangle property.
        /// </summary>
        public SerializedProperty paginatedScrollRect => base[nameof(paginatedScrollRect)];

        /// <summary>
        /// The page dot prefab property.
        /// </summary>
        public SerializedProperty pageDotPrefab => base[nameof(pageDotPrefab)];

        /// <summary>
        /// The page dots property.
        /// </summary>
        public SerializedProperty pageDots => base[nameof(pageDots)];

        /// <summary>
        /// The page dots property.
        /// </summary>
        public SerializedProperty indexOffset => base[nameof(indexOffset)];

        /// <summary>
        /// Explicit constructor to call base.
        /// </summary>
        /// <param name="serializedObject">The object to gain the serialized properties from.</param>
        public PageDotManagerSerializedPropertyCache(SerializedObject serializedObject) : base(serializedObject) { }
    }
}
