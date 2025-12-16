using UnityEditor;
using UnityEngine.UI;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// Contains the menu options for <see cref="PaginatedScrollRect"/>.
    /// </summary>
    internal static class PaginatedScrollRectMenu
    {
        /// <summary>
        /// The item name used for the paginated scroll rect as part of the
        /// menu items.
        /// </summary>
        public const string ITEM_NAME = "Paginated Scroll Rect";
        
        /// <summary>
        /// Creates the Paginated Scroll Rect from the create menu.
        /// </summary>
        [MenuItem("GameObject/UI/Paginated/" + ITEM_NAME)]
        public static void CreatePaginatedScrollView()
        {
            EditorApplication.ExecuteMenuItem("GameObject/UI/Scroll View");
            ScrollRect scrollRect = Selection.activeGameObject.GetComponent<ScrollRect>();
            if (scrollRect == null)
                return;
            PaginatedScrollRect infiniteScroll = scrollRect.ToPaginatedScrollRect();
            infiniteScroll.gameObject.name = "Paginated Scroll Rect Horizontal";
        }
    }
}