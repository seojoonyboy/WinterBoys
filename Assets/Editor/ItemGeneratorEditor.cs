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
        dh_coolTime,
        dh_minTime,
        dh_maxTime,
        items,
        parent;

    private void OnEnable() {
        state_Prop = serializedObject.FindProperty("gameType");

        dh_playerController = serializedObject.FindProperty("dh_playerController");
        sj_playerController = serializedObject.FindProperty("sj_playerController");

        dh_coolTime = serializedObject.FindProperty("dh_coolTime");
        dh_minTime = serializedObject.FindProperty("dh_minTime");
        dh_maxTime = serializedObject.FindProperty("dh_maxTime");

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
                myTarget.dh_minTime = EditorGUILayout.IntField("dh_minTime", myTarget.dh_minTime);
                myTarget.dh_maxTime = EditorGUILayout.IntField("dh_maxTime", myTarget.dh_maxTime);
                myTarget.dh_coolTime = EditorGUILayout.IntField("dh_coolTime", myTarget.dh_coolTime);
                break;
            case SportType.SKELETON:

                break;
            case SportType.SKIJUMP:
                EditorGUILayout.PropertyField(sj_playerController, true, new GUILayoutOption[0]);
                
                myTarget.sj_minTime = EditorGUILayout.IntField("sj_minTime", myTarget.sj_minTime);
                myTarget.sj_maxTime = EditorGUILayout.IntField("sj_maxTime", myTarget.sj_maxTime);
                myTarget.sj_coolTime = EditorGUILayout.IntField("sj_coolTime", myTarget.sj_coolTime);
                break;
        }

        EditorGUILayout.PropertyField(items, new GUIContent("items"), true);
        EditorGUILayout.PropertyField(parent, new GUIContent("parent"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
