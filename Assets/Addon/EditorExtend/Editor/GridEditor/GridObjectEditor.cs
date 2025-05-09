using UnityEditor;
using UnityEngine;

namespace EditorExtend.GridEditor
{
    [CustomEditor(typeof(GridObject), true)]
    public class GridObjectEditor : AutoEditor
    {
        public GridObject GridObject => target as GridObject;
        [AutoProperty]
        public SerializedProperty cellPosition, groundHeight;

        private Vector3Int prev;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GridObject.Manager != null)
                prev = cellPosition.vector3IntValue = GridObject.Manager.ClosestCell(GridObject.transform.position);
            serializedObject.ApplyModifiedProperties();
        }

        protected override void MyOnInspectorGUI()
        {
            cellPosition.Vector3IntField("网格坐标");
            groundHeight.IntField("地面高度");
            if (cellPosition.vector3IntValue != prev)
            {
                prev = cellPosition.vector3IntValue;
                GridObject.CellPosition = cellPosition.vector3IntValue;
            }
            if (GUILayout.Button("Z不变对齐"))
            {
                cellPosition.vector3IntValue = GridObject.AlignXY();
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("XY不变对齐"))
            {
                cellPosition.vector3IntValue = GridObject.AlignZ();
                EditorUtility.SetDirty(target);
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("引用次数", GridObject.referenceCount);
            EditorGUI.EndDisabledGroup();
        }
    }
}