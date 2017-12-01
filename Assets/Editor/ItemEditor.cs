using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor {
    public SerializedProperty
        state_Prop,
        item_dh,
        item_sj,
        item_st;

    private void OnEnable() {
        state_Prop = serializedObject.FindProperty("gameType");

        item_dh = serializedObject.FindProperty("item_dh");
        item_sj = serializedObject.FindProperty("item_sj");
        item_st = serializedObject.FindProperty("item_st");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        Item myTarget = (Item)target;

        EditorGUILayout.PropertyField(state_Prop);
        SportType st = (SportType)state_Prop.enumValueIndex;

        switch (st) {
            case SportType.DOWNHILL:
                EditorGUILayout.PropertyField(item_dh, true, new GUILayoutOption[0]);
                break;
            case SportType.SKELETON:
                EditorGUILayout.PropertyField(item_st, true, new GUILayoutOption[0]);
                break;
            case SportType.SKIJUMP:
                EditorGUILayout.PropertyField(item_sj, true, new GUILayoutOption[0]);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
