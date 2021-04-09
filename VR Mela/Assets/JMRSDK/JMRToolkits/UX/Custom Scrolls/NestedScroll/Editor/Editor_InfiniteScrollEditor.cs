// ----------------------------------------------------------------------------
// The MIT License
// InfiniteScroll https://github.com/mopsicus/infinite-scroll-unity
// Copyright (c) 2018-2019 Mopsicus <mail@mopsicus.ru>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit
{

    [CustomEditor(typeof(JMRNestedInfiniteScroll))]
    public class Editor_InfiniteScrollEditor : Editor
    {

        /// <summary>
        /// Scroller target
        /// </summary>
        private JMRNestedInfiniteScroll j_Target;

        /// <summary>
        /// Serialized target object
        /// </summary>
        private SerializedObject j_Object;

        /// <summary>
        /// Item list prefab
        /// </summary>
        private SerializedProperty j_Prefab;

        /// <summary>
        /// Fill Count
        /// </summary>
        private SerializedProperty j_FillCount;

        /// <summary>
        /// Fill Count
        /// </summary>
        private SerializedProperty j_PoolPrefabs;

        /// <summary>
        /// Top padding
        /// </summary>
        private SerializedProperty j_TopPadding;

        /// <summary>
        /// Bottom padding
        /// </summary>
        private SerializedProperty j_BottomPadding;

        /// <summary>
        /// Spacing between items
        /// </summary>
        private SerializedProperty j_ItemSpacing;

        /// <summary>
        /// Label font asset
        /// </summary>
        private SerializedProperty j_LabelsFont;

        /// <summary>
        /// Pull top text label
        /// </summary>
        private SerializedProperty j_TopPullLabel;

        /// <summary>
        /// Release top text label
        /// </summary>
        private SerializedProperty j_TopReleaseLabel;

        /// <summary>
        /// Pull bottom text label
        /// </summary>
        private SerializedProperty j_BottomPullLabel;

        /// <summary>
        /// Release bottom text label
        /// </summary>
        private SerializedProperty j_BottomReleaseLabel;

        /// <summary>
        /// Can we pull from top
        /// </summary>
        private SerializedProperty j_IsPullTop;

        /// <summary>
        /// Can we pull from bottom
        /// </summary>
        private SerializedProperty j_IsPullBottom;

        /// <summary>
        /// Left padding
        /// </summary>
        private SerializedProperty j_LeftPadding;

        /// <summary>
        /// Right padding
        /// </summary>
        private SerializedProperty _rightPadding;

        /// <summary>
        /// Pull left text label
        /// </summary>
        private SerializedProperty j_LeftPullLabel;

        /// <summary>
        /// Release left text label
        /// </summary>
        private SerializedProperty j_LeftReleaseLabel;

        /// <summary>
        /// Pull right text label
        /// </summary>
        private SerializedProperty j_RightPullLabel;

        /// <summary>
        /// Release right text label
        /// </summary>
        private SerializedProperty j_RightReleaseLabel;

        /// <summary>
        /// Can we pull from left
        /// </summary>
        private SerializedProperty j_IsPullLeft;

        /// <summary>
        /// Can we pull from right
        /// </summary>
        private SerializedProperty j_IsPullRight;

        /// <summary>
        /// Coefficient when labels should action
        /// </summary>
        private SerializedProperty j_PullValue;

        /// <summary>
        /// Label position offset
        /// </summary>
        private SerializedProperty j_LabelOffset;

        /// <summary>
        /// Init data
        /// </summary>
        private void OnEnable()
        {
            j_Target = (JMRNestedInfiniteScroll)target;
            j_Object = new SerializedObject(target);
            j_Prefab = j_Object.FindProperty("Prefab");
            j_PoolPrefabs = j_Object.FindProperty("poolPrefabs");
            j_FillCount = j_Object.FindProperty("fillCount");
            j_TopPadding = j_Object.FindProperty("TopPadding");
            j_BottomPadding = j_Object.FindProperty("BottomPadding");
            j_ItemSpacing = j_Object.FindProperty("ItemSpacing");
            j_LabelsFont = j_Object.FindProperty("LabelsFont");
            j_TopPullLabel = j_Object.FindProperty("TopPullLabel");
            j_TopReleaseLabel = j_Object.FindProperty("TopReleaseLabel");
            j_BottomPullLabel = j_Object.FindProperty("BottomPullLabel");
            j_BottomReleaseLabel = j_Object.FindProperty("BottomReleaseLabel");
            //j_IsPullTop = j_Object.FindProperty("IsPullTop");
            j_IsPullBottom = j_Object.FindProperty("IsPullBottom");
            j_LeftPadding = j_Object.FindProperty("LeftPadding");
            _rightPadding = j_Object.FindProperty("RightPadding");
            j_LeftPullLabel = j_Object.FindProperty("LeftPullLabel");
            j_LeftReleaseLabel = j_Object.FindProperty("LeftReleaseLabel");
            j_RightPullLabel = j_Object.FindProperty("RightPullLabel");
            j_RightReleaseLabel = j_Object.FindProperty("RightReleaseLabel");
            j_IsPullLeft = j_Object.FindProperty("IsPullLeft");
            //j_IsPullRight = j_Object.FindProperty("IsPullRight");
            j_PullValue = j_Object.FindProperty("PullValue");
            j_LabelOffset = j_Object.FindProperty("LabelOffset");
        }

        /// <summary>
        /// Draw inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            j_Object.Update();
            EditorGUI.BeginChangeCheck();
            j_Target.Type = GUILayout.Toolbar(j_Target.Type, new string[] { "Vertical", "Horizontal" });
            switch (j_Target.Type)
            {
                case 0:
                    EditorGUILayout.PropertyField(j_Prefab);
                    EditorGUILayout.PropertyField(j_PoolPrefabs);
                    EditorGUILayout.PropertyField(j_FillCount);
                    EditorGUILayout.PropertyField(j_TopPadding);
                    EditorGUILayout.PropertyField(j_BottomPadding);
                    EditorGUILayout.PropertyField(j_ItemSpacing);
                    EditorGUILayout.PropertyField(j_LabelsFont);
                    EditorGUILayout.PropertyField(j_TopPullLabel);
                    EditorGUILayout.PropertyField(j_TopReleaseLabel);
                    EditorGUILayout.PropertyField(j_BottomPullLabel);
                    EditorGUILayout.PropertyField(j_BottomReleaseLabel);
                    //EditorGUILayout.PropertyField(j_IsPullTop);
                    EditorGUILayout.PropertyField(j_IsPullBottom);
                    EditorGUILayout.PropertyField(j_PullValue);
                    EditorGUILayout.PropertyField(j_LabelOffset);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(j_Prefab);
                    EditorGUILayout.PropertyField(j_PoolPrefabs);
                    EditorGUILayout.PropertyField(j_FillCount);
                    EditorGUILayout.PropertyField(j_LeftPadding);
                    EditorGUILayout.PropertyField(_rightPadding);
                    EditorGUILayout.PropertyField(j_ItemSpacing);
                    EditorGUILayout.PropertyField(j_LabelsFont);
                    EditorGUILayout.PropertyField(j_LeftPullLabel);
                    EditorGUILayout.PropertyField(j_LeftReleaseLabel);
                    EditorGUILayout.PropertyField(j_RightPullLabel);
                    EditorGUILayout.PropertyField(j_RightReleaseLabel);
                    EditorGUILayout.PropertyField(j_IsPullLeft);
                    //EditorGUILayout.PropertyField(j_IsPullRight);
                    EditorGUILayout.PropertyField(j_PullValue);
                    EditorGUILayout.PropertyField(j_LabelOffset);
                    break;
                default:
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {
                j_Object.ApplyModifiedProperties();
            }
        }

    }

}