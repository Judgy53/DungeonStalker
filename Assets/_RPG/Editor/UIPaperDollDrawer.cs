using UnityEditor;
using System.Collections;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIPaperDoll))]
public class UIPaperDollDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        SerializedProperty p = serializedObject.GetIterator();
        p.Next(true);
        do
        {
            if (p.name.Contains("m_"))
                continue;

            if (p.name == "armorSlots" && p.isExpanded)
                DrawArmorArrayEditorProperty(p);
            else
                EditorGUILayout.PropertyField(p);
        }
        while (p.Next(false));
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }


    public static void DrawArmorArrayEditorProperty(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list);

        SerializedProperty size = list.FindPropertyRelative("Array.size");
        EditorGUILayout.PropertyField(size);

        EditorGUI.indentLevel++;

        for (int i = 0; i < list.arraySize; i++)
        {
            string name = "Unknown";
            if (i < ArmorSlot.Count)
                name = ((ArmorSlot.ArmorSlotHelper)i).ToString();

            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(name));
        }

        EditorGUI.indentLevel--;
    }
}