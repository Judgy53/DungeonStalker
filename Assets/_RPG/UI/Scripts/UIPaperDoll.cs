using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

public class UIPaperDoll : MonoBehaviour
{
    public Image[] armorSlots = new Image[ArmorSlot.Count];
    public Image mainHandSlot = null;
    public Image offHandSlot = null;
    public Image ammoSlot = null;

    private UICharSheet manager = null;
    private ArmorManager armorTarget = null;
    private WeaponManager weapTarget = null;

    private Sprite[] defaultSprites = null;
    private Sprite defaultMainHandSprite = null;
    private Sprite defaultOffHandSprite = null;
    private Sprite defaultAmmoSprite = null;

    private void Start()
    {
        GameManager.OnPlayerCreation += GameManager_OnPlayerCreation;

        manager = GetComponentInParent<UICharSheet>();
        if (manager == null)
        {
            Debug.LogError("No manager found for " + this.name);
            enabled = false;
            return;
        }

        manager.OnMenuStateChange += manager_OnMenuStateChange;

        defaultSprites = new Sprite[armorSlots.Length];
        for (int i = 0; i < armorSlots.Length; i++)
            defaultSprites[i] = armorSlots[i].sprite;

        defaultMainHandSprite = mainHandSlot.sprite;
        defaultOffHandSprite = offHandSlot.sprite;
        defaultAmmoSprite = ammoSlot.sprite;
    }

    void manager_OnMenuStateChange(object sender, UIMenuStateChangeArgs e)
    {
        if (e.newState == UIMenuState.Shown)
            ActualizeSlots();
        UITooltip.HideOnce();
    }

    void GameManager_OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        armorTarget = e.player.GetComponent<ArmorManager>();
        weapTarget = e.player.GetComponent<WeaponManager>();
    }

    private void ActualizeSlots()
    {
        if (armorTarget != null)
        {
            for (int i = 0; i < ArmorSlot.Count; i++)
            {
                if (i < armorSlots.Length && i < armorTarget.Armor.Length)
                {
                    if (armorTarget.Armor[i] != null && armorTarget.Armor[i].ItemPrefab != null)
                    {
                        IItem item = armorTarget.Armor[i].ItemPrefab.GetComponent<IItem>();
                        if (item != null && item.Image != null)
                            armorSlots[i].sprite = item.Image;
                        else
                            armorSlots[i].sprite = defaultSprites[i];
                    }
                    else
                        armorSlots[i].sprite = defaultSprites[i];
                }
            }
        }

        if (weapTarget != null)
        {
            CheckWeaponSlot(mainHandSlot, weapTarget.MainHandWeapon, defaultMainHandSprite);
            CheckWeaponSlot(offHandSlot, weapTarget.OffHandWeapon, defaultOffHandSprite);

            if (weapTarget.CurrentAmmos != null && weapTarget.CurrentAmmos.ItemPrefab != null)
            {
                IItem item = weapTarget.CurrentAmmos.ItemPrefab;
                if (item != null && item.Image != null)
                    ammoSlot.sprite = item.Image;
                else
                    ammoSlot.sprite = defaultAmmoSprite;
            }
            else
                ammoSlot.sprite = defaultAmmoSprite;
        }
    }

    private void CheckWeaponSlot(Image slot, IWeapon weapon, Sprite defaultSprite)
    {
        if (weapon != null && weapon.InventoryItemPrefab != null)
        {
            IItem item = weapon.InventoryItemPrefab.GetComponent<IItem>();
            if (item != null && item.Image != null)
                slot.sprite = item.Image;
            else
                slot.sprite = defaultSprite;
        }
        else
            slot.sprite = defaultSprite;
    }

    public void RemoveArmorCallback(UIMouseEvents sender)
    {
        int senderIndex = System.Array.FindIndex(armorSlots, x => x.gameObject == sender.gameObject);
        if (senderIndex != -1 && armorTarget != null && armorTarget.Armor[senderIndex] != null)
        {
            armorTarget.Armor[senderIndex].TransferToContainer(armorTarget.GetComponentInChildren<IContainer>());
            armorTarget.Armor[senderIndex] = null;
            armorTarget.RecomputeGearStats();
        }
        else if (senderIndex == -1 && weapTarget != null)
        {
            if (sender == mainHandSlot.GetComponent<UIMouseEvents>())
                weapTarget.MainHandWeapon = null;
            else if (sender == offHandSlot.GetComponent<UIMouseEvents>())
                weapTarget.OffHandWeapon = null;
            else if (sender == ammoSlot.GetComponent<UIMouseEvents>())
                weapTarget.CurrentAmmos = null;
        }

        ActualizeSlots();
        RebuildTooltipDescription(sender);
    }

    public void RebuildTooltipDescription(UIMouseEvents sender)
    {
        UITooltip.Text = "";
        UITooltip.DisableForceHide();

        int senderIndex = System.Array.FindIndex(armorSlots, x => x.gameObject == sender.gameObject);
        if (senderIndex != -1 && armorTarget != null)
        {
            if (armorTarget.Armor[senderIndex] != null && armorTarget.Armor[senderIndex].ItemPrefab != null)
            {
                IItem item = armorTarget.Armor[senderIndex].ItemPrefab.GetComponent<IItem>();
                if (item != null)
                {
                    Armor a = armorTarget.Armor[senderIndex];

                    UITooltip.Text += "<size=12>";
                    UITooltip.Text += "<color=#" + item.Quality.ToColor().ToHexStringRGBA() + ">" + item.Name;
                    UITooltip.Text += "</color>";
                    UITooltip.Text += "</size>\n";
                    UITooltip.Text += "<color=yellow><size=7><i>";
                    UITooltip.Text += item.Description;
                    UITooltip.Text += "</i></size></color>\n\n";

                    UITooltip.Text += a.Type.ToString() + " armor\n";
                    UITooltip.Text += "Armor value : " + a.ArmorValue + "\n";

                    if (a.Stats != 0)
                    {
                        UITooltip.Text += "<color=green>";
                        if (a.Stats.Strength != 0)
                            UITooltip.Text += "Strength +" + a.Stats.Strength + "\n";
                        if (a.Stats.Stamina != 0)
                            UITooltip.Text += "Stamina +" + a.Stats.Stamina + "\n";
                        if (a.Stats.Defense != 0)
                            UITooltip.Text += "Defense +" + a.Stats.Defense + "\n";
                        if (a.Stats.Energy != 0)
                            UITooltip.Text += "Energy +" + a.Stats.Energy + "\n";
                        UITooltip.Text += "</color>";
                    }

                    UITooltip.Text += "<color=green><i>Right click to Unequip</i></color>";
                }
            }
            else
                UITooltip.ForceHide();
        }
        else if (senderIndex == -1 && weapTarget != null)
        {
            /*if (sender == mainHandSlot.GetComponent<UIMouseEvents>())
                weapTarget.MainHandWeapon = null;
            else if (sender == offHandSlot.GetComponent<UIMouseEvents>())
                weapTarget.OffHandWeapon = null;
            else if (sender == ammoSlot.GetComponent<UIMouseEvents>())
                weapTarget.CurrentAmmos = null;*/
        }
    }
}

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
                ArmorSlot.DrawArmorArrayEditorProperty(p);
            else
                EditorGUILayout.PropertyField(p);
        }
        while (p.Next(false));
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
