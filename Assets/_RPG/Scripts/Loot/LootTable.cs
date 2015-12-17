using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootTable : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float lootChance = 0.3f;

    [SerializeField]
    private Behaviour container = null;
    public IContainer Container { get { return (IContainer)container; } }

    [SerializeField]
    private Loot[] lootTable = new Loot[0];
    public Loot[] Table { get { return lootTable; } }

    public uint maxItemNumber = 3;

    public void Awake()
    {
        //Sorted rarest to most common
        System.Array.Sort(lootTable, delegate(Loot x, Loot y)
        {
            if (x.dropChance < y.dropChance)
                return -1;
            else if (x.dropChance == y.dropChance)
                return 0;
            return 1;
        });
    }

    public void GenerateLoot()
    {
        if (container == null)
        {
            Debug.LogError("No container set on " + this.name);
            return;
        }

        if (Container.Items.Length > 0) // if is already filled, don't generate loot
            return;

        float rand = Random.Range(0.0f, 1.0f);
        if (rand > lootChance)
            return;

        uint addedItemCount = 0;
        foreach (var loot in lootTable)
        {
            if (addedItemCount >= maxItemNumber)
                break;

            rand = Random.Range(0.0f, 1.0f);
            if (rand <= loot.dropChance)
            {
                if (loot.itemGo == null)
                    Debug.Log("Loot gameobject is null : " + loot.ToString());

                GameObject itemgo = GameObject.Instantiate(loot.itemGo, Vector3.zero, Quaternion.identity) as GameObject;
                IItem item = itemgo.GetComponent<IItem>();

                Container.AddItem(item);
                addedItemCount++;
            }
        }
    }
}

[System.Serializable]
public class Loot
{
    public GameObject itemGo = null;
    public IItem Item { get { return itemGo.GetComponent<IItem>(); } }

    [Range(0.0f, 1.0f)]
    public float dropChance = 0.5f;
}
