using System;
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// Defines extension methods for <see cref="ScrollRect"/> class.
    /// </summary>
    public static class PaginatedScrollRectExtensions
    {
        /// <summary>
        /// Converts a scroll rectangle to a paginated scroll rectangle.
        /// </summary>
        /// <param name="scrollRect">The scroll rectangle to convert.</param>
        /// <returns>The created paginated scroll rectangle.</returns>
        public static PaginatedScrollRect ToPaginatedScrollRect(this ScrollRect scrollRect)
        {
            if (scrollRect == null) throw new ArgumentNullException(nameof(scrollRect));

            // Save reference to attached GameObject.
            GameObject gameObject = scrollRect.gameObject;

            // Save settings of the scroll rect, since we can't access these when the component is destroyed.
            var scrollRectSettings = new
            {
                scrollRect.content,
                scrollRect.viewport
            };

            // Destroy objects.
            scrollRect.viewport.sizeDelta = Vector2.zero;
            if (scrollRect.horizontalScrollbar != null)
                Object.DestroyImmediate(scrollRect.horizontalScrollbar.gameObject);
            if (scrollRect.verticalScrollbar != null)
                Object.DestroyImmediate(scrollRect.verticalScrollbar.gameObject);

            Object.DestroyImmediate(scrollRect);

            // Add the new scroll rect.
            PaginatedScrollRect paginatedScrollRect = gameObject.AddComponent<PaginatedScrollRect>();

            // Apply settings from previous scroll rect.
            paginatedScrollRect.Content = scrollRectSettings.content;
            paginatedScrollRect.ViewPort = scrollRect.viewport;

            return paginatedScrollRect;
        }
    }
}
