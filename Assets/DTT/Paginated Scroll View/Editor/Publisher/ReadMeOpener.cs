#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// Class that handles opening the editor window for the paginated-scroll-view package.
    /// </summary>
    internal static class ReadMeOpener
    {
        /// <summary>
        /// Opens the readme for this package.
        /// </summary>
        [MenuItem("Tools/DTT/PaginatedScrollView/ReadMe")]
        private static void OpenReadMe() => DTTEditorConfig.OpenReadMe("dtt.paginated-scroll-view");
    }
}
#endif
