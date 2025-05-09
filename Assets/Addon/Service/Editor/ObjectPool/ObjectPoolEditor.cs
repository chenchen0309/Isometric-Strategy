using EditorExtend;
using UnityEditor;
using UnityEngine;

namespace Services.ObjectPools
{
    [CustomEditor(typeof(ObjectPool))]
    public class ObjectPoolEditor : AutoEditor
    {
        private ObjectPool ObjectPool => target as ObjectPool;
        [AutoProperty]
        public SerializedProperty prefab;

        protected override void MyOnInspectorGUI()
        {

            if (!Application.isPlaying)
            {
                base.OnInspectorGUI();
                return;
            }
            EditorGUI.BeginDisabledGroup(true);
            prefab.PropertyField("预制体");
            EditorGUILayout.IntField("当前可用物体数", ObjectPool.Count);
            EditorGUI.EndDisabledGroup();
        }
    }
}