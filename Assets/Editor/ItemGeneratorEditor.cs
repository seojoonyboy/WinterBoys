using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemGenerator))]
public class ItemGeneratorEditor : Editor {
    //dh : downhill
    //sj : skijump
    //st : skeleton
    public SerializedProperty
        state_Prop,
        dh_playerController,
        sj_playerController,
        dh_standardChangeMeters,
        dh_intervalMeters,
        dh_itemAreas,
        dh_numPerGenerates,
        sj_standardChangeMeter,
        sj_intervalMeter,
        sj_numPerGenerate,
        items,
        parent;

    private void OnEnable() {
        state_Prop = serializedObject.FindProperty("gameType");

        dh_playerController = serializedObject.FindProperty("dh_playerController");
        sj_playerController = serializedObject.FindProperty("sj_playerController");

        dh_standardChangeMeters = serializedObject.FindProperty("dh_standardChangeMeter");
        dh_intervalMeters = serializedObject.FindProperty("dh_intervalMeter");
        dh_itemAreas = serializedObject.FindProperty("dh_itemArea");
        dh_numPerGenerates = serializedObject.FindProperty("dh_numPerGenerate");

        sj_standardChangeMeter = serializedObject.FindProperty("sj_standardChangeMeter");
        sj_intervalMeter = serializedObject.FindProperty("sj_intervalMeter");
        sj_numPerGenerate = serializedObject.FindProperty("sj_numPerGenerate");

        items = serializedObject.FindProperty("items");
        parent = serializedObject.FindProperty("parent");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        ItemGenerator myTarget = (ItemGenerator)target;

        EditorGUILayout.PropertyField(state_Prop);
        SportType st = (SportType)state_Prop.enumValueIndex;
        
        switch (st) {
            case SportType.DOWNHILL:
                EditorGUILayout.PropertyField(dh_playerController, true, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(dh_standardChangeMeters, new GUIContent("dh_standardChangeMeters"), true);
                EditorGUILayout.PropertyField(dh_intervalMeters, new GUIContent("dh_intervalMeters"), true);
                EditorGUILayout.PropertyField(dh_itemAreas, new GUIContent("dh_itemAreas"), true);
                EditorGUILayout.PropertyField(dh_numPerGenerates, new GUIContent("dh_numPerGenerates"), true);
                break;
            case SportType.SKELETON:

                break;
            case SportType.SKIJUMP:
                EditorGUILayout.PropertyField(sj_playerController, true, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(sj_standardChangeMeter, new GUIContent("sj_standardChangeMeter"), true);
                EditorGUILayout.PropertyField(sj_intervalMeter, new GUIContent("sj_intervalMeter"), true);
                EditorGUILayout.PropertyField(sj_numPerGenerate, new GUIContent("sj_numPerGenerate"), true);
                break;
        }

        EditorGUILayout.PropertyField(items, new GUIContent("items"), true);
        EditorGUILayout.PropertyField(parent, new GUIContent("parent"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
