using UnityEngine;
using UnityEditor;
using System.Collections;

public class ArmorCreator : EditorWindow
{
    private string armorName = "";
    private string armorDescription = "";
    private int armorValue = 0;
    private ArmorType armorType = ArmorType.Cloth;
    private ArmorSlot.ArmorSlotHelper armorSlot = ArmorSlot.ArmorSlotHelper.Head;
    private int strength = 0;
    private int stamina = 0;
    private int defense = 0;
    private int energy = 0;

    private ItemQuality quality = ItemQuality.Common;

    private bool showArmorFields = true;
    private bool showArmorStats = true;

    private bool showItemFields = true;

    [MenuItem("RPG/ArmorCreator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ArmorCreator));
    }

    private void OnGUI()
    {
        GUILayout.Label("RPG Armor Creator.", EditorStyles.boldLabel);
        GUILayout.Label("This tool has been created to build Armor Prefabs required for the RPG Dungeon Stalker.\n");

        //EditorGUILayout.LabelField("Armor field : ");

        showArmorFields = EditorGUILayout.Foldout(showArmorFields, "Armor fields");
        if (showArmorFields)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Name");
                armorName = EditorGUILayout.TextField("", armorName);
                EditorGUILayout.PrefixLabel("Armor value");
                armorValue = EditorGUILayout.IntField(armorValue);

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Description");
                armorDescription = EditorGUILayout.TextField("", armorDescription);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Armor type");
                armorType = (ArmorType)EditorGUILayout.EnumPopup(armorType);
                EditorGUILayout.PrefixLabel("Armor slot");
                armorSlot = (ArmorSlot.ArmorSlotHelper)EditorGUILayout.EnumPopup(armorSlot);
            }
            EditorGUILayout.EndHorizontal();

            showArmorStats = EditorGUILayout.Foldout(showArmorStats, "Stats");
            if (showArmorStats)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Strength");
                    strength = EditorGUILayout.IntField(strength);
                    EditorGUILayout.PrefixLabel("Stamina");
                    stamina = EditorGUILayout.IntField(stamina);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Defense");
                    defense = EditorGUILayout.IntField(defense);
                    EditorGUILayout.PrefixLabel("Energy");
                    energy = EditorGUILayout.IntField(energy);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();

        //EditorGUILayout.LabelField("Item field : ");
        showItemFields = EditorGUILayout.Foldout(showItemFields, "Items fields");
        if (showItemFields)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Quality");
                quality = (ItemQuality)EditorGUILayout.EnumPopup(quality);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
    }
}
