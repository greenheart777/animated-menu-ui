using UnityEngine;
using UnityEditor;
using DTT.PublishingTools;
using DTT.PaginatedScrollView.AssistingUI;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// Custom editor for the <see cref="PageDotManager"/> class.
    /// </summary>
    [CustomEditor(typeof(PageDotManager))]
    [DTTHeader("dtt.paginated-scroll-view", "Page Dot Manager")]
    public class PageDotManagerEditor : DTTInspector
    {
        /// <summary>
        /// Reference to the target.
        /// </summary>
        private PageDotManager _pageDotManager;

        /// <summary>
        /// Cache containing all the serialized properties.
        /// </summary>
        private PageDotManagerSerializedPropertyCache _serializedPropertyCache;

        /// <summary>
        /// Initializes all properties and sets up the component correctly for use.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _pageDotManager =
                (PageDotManager)target;

            _serializedPropertyCache =
                new PageDotManagerSerializedPropertyCache(serializedObject);
        }

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_serializedPropertyCache.paginatedScrollRect);
            EditorGUILayout.PropertyField(_serializedPropertyCache.indexOffset);
            EditorGUILayout.PropertyField(_serializedPropertyCache.pageDotPrefab);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh Dots"))
                _pageDotManager.Initialize();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_serializedPropertyCache.pageDots);
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}