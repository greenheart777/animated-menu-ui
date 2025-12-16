using UnityEditor;
using UnityEngine.UI;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// Contains the menu options for <see cref="ScrollRect"/>.
    /// </summary>
    internal static class ScrollRectUpgradeMenu
    {
        /// <summary>
        /// Adds the context menu option to <see cref="ScrollRect"/> to upgrade it to a <see cref="PaginatedScrollRect"/>.
        /// </summary>
        /// <param name="command">The command options from the context event.</param>
        [MenuItem("CONTEXT/ScrollRect/Upgrade to " + PaginatedScrollRectMenu.ITEM_NAME)]
        private static void ScrollRectToPaginatedScrollRect(MenuCommand command)
        {
            ScrollRect scrollRect = command.context as ScrollRect;
            if (scrollRect == null)
                return;
            scrollRect.ToPaginatedScrollRect();
        }
    }
}
