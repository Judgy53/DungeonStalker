using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(ArmorManager))]
public class AmorManagerCustomEditor : Editor
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

            if (p.name == "debugStartArmor" && p.isExpanded)
                UIPaperDollDrawer.DrawArmorArrayEditorProperty(p);
            else
                EditorGUILayout.PropertyField(p);
        }
        while (p.Next(false));
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}