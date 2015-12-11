using UnityEngine;
using UnityEditor;
using System.Collections;

public class ArmorCreator : EditorWindow
{
    //Armor fields
    private string armorName = "";
    private string armorDescription = "";
    private int armorValue = 0;
    private ArmorType armorType = ArmorType.Cloth;
    private ArmorSlot.ArmorSlotHelper armorSlot = ArmorSlot.ArmorSlotHelper.Head;
    private int strength = 0;
    private int stamina = 0;
    private int defense = 0;
    private int energy = 0;

    //Item fields
    private ItemQuality quality = ItemQuality.Common;
    private Sprite image = null;
    private int weight = 5;
    private bool canDrop = true;

    //Pickable fields
    private string pickableDescription = "";
    private GameObject pickableModel = null;

    private bool showArmorFields = true;
    private bool showArmorStats = true;

    private bool showItemFields = true;

    private bool showPickableFields = true;

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

        showItemFields = EditorGUILayout.Foldout(showItemFields, "Items fields");
        if (showItemFields)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Quality");
                quality = (ItemQuality)EditorGUILayout.EnumPopup(quality);
                EditorGUILayout.PrefixLabel("Image");
                image = (Sprite)EditorGUILayout.ObjectField(image, typeof(Sprite), false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Weight");
                weight = EditorGUILayout.IntField(weight);
                EditorGUILayout.PrefixLabel("CanDrop");
                EditorGUILayout.Toggle(canDrop);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        showPickableFields = EditorGUILayout.Foldout(showPickableFields, "Pickable fields");
        if (showPickableFields)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Pickup description");
                pickableDescription = EditorGUILayout.TextArea(pickableDescription);
                EditorGUILayout.PrefixLabel("Pickable model");
                pickableModel = (GameObject)EditorGUILayout.ObjectField(pickableModel, typeof(GameObject), false);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("The model MUST contain a collider !");

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Files will be placed in Assets/Resources/(Armors|Items/Armors|Pickables/Armors).");

        if (GUILayout.Button("Create Armor"))
        {
            //Creating Armor ScriptableObject
            Armor newArmor = ScriptableObject.CreateInstance<Armor>();

            Utils.SetPrivateFieldValue<int>(newArmor, "armor", armorValue);
            Utils.SetPrivateFieldValue<string>(newArmor, "armorName", armorName);
            Utils.SetPrivateFieldValue<ArmorSlot>(newArmor, "slot", (ArmorSlot)((int)armorSlot));
            Utils.SetPrivateFieldValue<ArmorType>(newArmor, "type", armorType);
            Utils.SetPrivateFieldValue<CharStats>(newArmor, "stats", new CharStats((uint)strength, (uint)defense, (uint)stamina, (uint)energy));

            //Creating Armor Item
            GameObject newItem = new GameObject("Item" + armorName);
            ItemArmor item = newItem.AddComponent<ItemArmor>();
            Utils.SetPrivateFieldValue<Sprite>(item, "image", image);
            Utils.SetPrivateFieldValue<string>(item, "itemName", armorName);
            Utils.SetPrivateFieldValue<string>(item, "actionName", "Equip");
            Utils.SetPrivateFieldValue<string>(item, "useDescription", "Equip Armor.");
            Utils.SetPrivateFieldValue<string>(item, "description", armorDescription);
            Utils.SetPrivateFieldValue<uint>(item, "weight", (uint)weight);
            Utils.SetPrivateFieldValue<ItemType>(item, "type", ItemType.Armor);
            Utils.SetPrivateFieldValue<ItemQuality>(item, "quality", quality);
            Utils.SetPrivateFieldValue<bool>(item, "canDrop", canDrop);

            //Creating Pickable
            GameObject newPickable = new GameObject("Pickable" + armorName, typeof(Rigidbody), typeof(Pickable));
            Rigidbody rb = newPickable.GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Pickable pickable = newPickable.GetComponent<Pickable>();
            pickable.actionName = "Pickup - " + armorName;
            pickable.description = pickableDescription;

            //Attaching model to pickable
            if (pickableModel != null)
            {
                GameObject model = GameObject.Instantiate(pickableModel);
                model.transform.SetParent(newPickable.transform, false);
            }

            //Saving prefabs
            GameObject itemPrefab = PrefabUtility.CreatePrefab("Assets/Resources/Items/Armors/" + newItem.name + ".prefab", newItem);
            GameObject pickablePrefab = PrefabUtility.CreatePrefab("Assets/Resources/Pickables/Armors/" + newPickable.name + ".prefab", newPickable);

            //Linkage
            Utils.SetPrivateFieldValue<GameObject>(newArmor, "itemPrefab", itemPrefab);
            Utils.SetPrivateFieldValue<ScriptableObject>(itemPrefab.GetComponent<ItemArmor>(), "armorPrefab", newArmor);
            Utils.SetPrivateFieldValue<GameObject>(itemPrefab.GetComponent<ItemArmor>(), "dropPrefab", pickablePrefab);
            Utils.SetPrivateFieldValue<GameObject>(pickablePrefab.GetComponent<Pickable>(), "pickedItemPrefab", itemPrefab);

            //Saving ScriptableObject
            AssetDatabase.CreateAsset(newArmor, "Assets/Resources/Armors/Armor" + armorName + ".asset");
            
            //Cleaning temp gameObjects
            GameObject.DestroyImmediate(newItem);
            GameObject.DestroyImmediate(newPickable);
        }
    }
}
