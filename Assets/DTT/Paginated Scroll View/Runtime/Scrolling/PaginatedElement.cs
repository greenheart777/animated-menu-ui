using System;
using UnityEngine;

namespace DTT.PaginatedScrollView
{
    /// <summary>
    /// Struct that holds the rect transform and the distance to the center of a page.
    /// </summary>
    [Serializable]
    public readonly struct PaginatedElement
    {
        /// <summary>
        /// The rect transform of the page.
        /// </summary>
        public readonly RectTransform rectTransform;

        /// <summary>
        /// The distance to the center of the page.
        /// </summary>
        public readonly Vector2 distanceToCenter;

        /// <summary>
        /// Creates a new PaginatedElement.
        /// </summary>
        /// <param name="rectTransform">The recttransform of the page.</param>
        /// <param name="distanceToCenter">The distance to the center of the page.</param>
        public PaginatedElement(RectTransform rectTransform, Vector2 distanceToCenter)
        {
            this.rectTransform = rectTransform;
            this.distanceToCenter = distanceToCenter;
        }
    }
}
