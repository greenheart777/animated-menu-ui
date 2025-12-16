using UnityEditor;
using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;

namespace DTT.PaginatedScrollView.Editor
{
    /// <summary>
    /// The custom editor for the <see cref="PaginatedScrollRect"/> class.
    /// </summary>
    [CustomEditor(typeof(PaginatedScrollRect))]
    [DTTHeader("dtt.paginated-scroll-view", "Paginated Scroll Rect")]
    public class PaginatedScrollRectEditor : DTTInspector
    {
        /// <summary>
        /// Cache containing all the serialized properties.
        /// </summary>
        private PaginatedScrollRectSerializedPropertyCache _serializedPropertyCache;

        /// <summary>
        /// The toggle used for the inertia.
        /// </summary>
        private AnimatedToggleFoldout _inertiaToggleFoldout;

        /// <summary>
        /// Foldout for the onSnapChange event.
        /// </summary>
        private AnimatedFoldout _onSnapChange;

        /// <summary>
        /// Foldout for the onPageArrivedAtSnappingPoint event.
        /// </summary>
        private AnimatedFoldout _onPageArrivedAtSnappingPoint;

        /// <summary>
        /// Foldout for the hidden fields.
        /// </summary>
        private AnimatedFoldout _hiddenFields;

        /// <summary>
        /// Initializes all properties and sets up the component correctly for use.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _serializedPropertyCache =
                new PaginatedScrollRectSerializedPropertyCache(serializedObject);
            _inertiaToggleFoldout =
                new AnimatedToggleFoldout(this);
            _onSnapChange =
                new AnimatedFoldout(this);
            _onPageArrivedAtSnappingPoint =
                new AnimatedFoldout(this);
            _hiddenFields =
                new AnimatedFoldout(this);
        }

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            #region Snapping & Movement
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Snapping & Movement", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_serializedPropertyCache.ScrollAxis);
            EditorGUILayout.PropertyField(_serializedPropertyCache.EndOfScrollBehaviour);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_serializedPropertyCache.SnapType);

            SnapType snapType = (SnapType)_serializedPropertyCache.SnapType.enumValueIndex;

            if (snapType == SnapType.ELASTIC ||
                snapType == SnapType.CLAMPED)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_serializedPropertyCache.SnapTowards);
                EditorGUI.indentLevel--;
            }

            if (snapType == SnapType.ELASTIC)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(
                    _serializedPropertyCache.SnapSpeed);
                EditorGUILayout.PropertyField(
                    _serializedPropertyCache.MinimumSnapDistance);
                EditorGUI.indentLevel--;
            }
            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(
                    _serializedPropertyCache.LayoutElementSpacing);
            EditorGUILayout.Space();

            #region UI
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_serializedPropertyCache.HorizontalOrVerticalLayoutGroup);
            EditorGUILayout.PropertyField(_serializedPropertyCache.Content);
            EditorGUILayout.PropertyField(_serializedPropertyCache.Viewport);
            EditorGUILayout.PropertyField(_serializedPropertyCache.PreviousButton);
            EditorGUILayout.PropertyField(_serializedPropertyCache.NextButton);
            #endregion

            #region Events
            //EditorGUILayout.Space();
            //EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

            //_onSnapChange.OnGUI("OnSnapChange", () =>
            //{
            //    EditorGUILayout.PropertyField(_serializedPropertyCache.OnSnapChange);
            //});

            //_onPageArrivedAtSnappingPoint.OnGUI("OnPageArrivedAtSnappingPoint", () =>
            //{
            //    EditorGUILayout.PropertyField(_serializedPropertyCache.OnPageArrivedAtSnappingPoint);
            //});
            #endregion

            #region Hidden fields
            EditorGUILayout.Space();
            _hiddenFields.OnGUI("Debugging Information", () =>
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.PropertyField(
                    _serializedPropertyCache.Dragging);
                EditorGUILayout.PropertyField(
                    _serializedPropertyCache.SnappedPageReachedDestination);
                EditorGUILayout.PropertyField(
                    _serializedPropertyCache.PreviousContentPosition);
                EditorGUILayout.PropertyField(
                    _serializedPropertyCache.PaginatedElements);

                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            });
            #endregion

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
