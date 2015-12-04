﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerContainer : MonoBehaviour, IContainer, IStatsDependable, ISavable
{
    public event EventHandler<WeightChangeArgs> OnWeightChange;

    [SerializeField]
    private float maxWeight = 100.0f;
    public float MaxWeight { get { return maxWeight; } set { maxWeight = value; } }

    private float currentWeight = 0.0f;
    public float CurrentWeight { get { return currentWeight; }
        private set
        {
            if (OnWeightChange != null)
                OnWeightChange(this, new WeightChangeArgs(currentWeight, value));

            currentWeight = value;
        }
    }

    public float dropDistance = 0.1f;

    public IItem[] Items { 
        get 
        {
            List<IItem> results = new List<IItem>(GetComponentsInChildren<IItem>());
            //Somehow, GetComponentsInChildren returns destroyed components on the same frame ...
            results.RemoveAll(x => !(x as Behaviour).enabled);
            return results.ToArray();
        }
    }

    private EffectManager effectManager = null;
    private MovementEffect effect = null;

    private StatsManager statsManager = null;
    public StatsManager StatsManager { get { return statsManager; } }

    private void Start()
    {
        OnWeightChange += OnWeightChangeCallback;
        effectManager = GetComponentInParent<EffectManager>();

        statsManager = GetComponentInParent<StatsManager>();
        if (statsManager != null)
            statsManager.Stats.OnStatsChange += OnStatsChange;
        else
            Debug.LogWarning("No StatsManager found in " + this.name + " parents ...");
    }

    public bool AddItem(IItem item)
    {
        (item as Behaviour).transform.SetParent(gameObject.transform, false);
        CurrentWeight += item.Weigth;

        return true;
    }

    public bool RemoveItem(IItem item)
    {
        Behaviour itemBehaviour = (item as Behaviour);
        if (itemBehaviour.transform.IsChildOf(transform))
        {
            itemBehaviour.transform.SetParent(null, false);
            CurrentWeight -= item.Weigth;
            return true;
        }

        return false;
    }

    private void ClearInventory()
    {
        IItem[] items = Items;

        foreach (IItem item in items)   
        {
            RemoveItem(item);
            Destroy((item as Behaviour).gameObject);
        }
    }

    public void DropItem(IItem item)
    {
        Behaviour itemBehaviour = (item as Behaviour);
        if (itemBehaviour.transform.IsChildOf(transform))
        {
            if (item.CanDrop)
            {
                CurrentWeight -= item.Weigth;

                if (item.DropPrefab != null)
                {
                    GameObject pickableGo = GameObject.Instantiate(item.DropPrefab, transform.position + transform.forward * dropDistance, Quaternion.identity) as GameObject;
                    pickableGo.GetComponent<Rigidbody>().AddForce(transform.forward * 2.0f);
                }

                GameObject.Destroy(itemBehaviour.gameObject);
            }
        }
    }

    private void OnWeightChangeCallback(object sender, WeightChangeArgs args)
    {
        if (effectManager == null)
            return;

        if (args.newWeight > maxWeight)
        {
            if (effect == null)
            {
                effect = new MovementEffect("Inventory overload", 0.2f, EffectStyle.Debuff, EffectType.Physical);
                effect.IsInfinite = true;
                effect.ShouldBeSaved = false;
                effectManager.AddEffect(effect);
            }
        }
        else
        {
            if (effect != null)
            {
                effectManager.RemoveEffect(effect);
                effect = null;
            }
        }
    }

    public void OnStatsChange(object sender, EventArgs args)
    {
        maxWeight = 90u + statsManager.Stats.Strength * 10u;
    }

    public void Save(SaveData data)
    {
        IItem[] items = Items;

        int count = items.Length;

        data.Add("count", count);

        for (int i = 0; i < count; i++)
        {
            IItem item = items[i];

            data.Add("Item_" + i, ResourcesPathHelper.GetItemPath(item));
        }
    }

    public void Load(SaveData data)
    {
        ClearInventory();

        int count = int.Parse(data.Get("count"));

        for (int i = 0; i < count; i++)
        {
            string path = data.Get("Item_" + i);
            if (path == null)
                break;

            GameObject prefab = Resources.Load(path) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning("Loading Inventory : Failed to load \"" + path + "\"");
                continue;
            }

            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

            AddItem(instance.GetComponent<IItem>());
        }
    }
}

public class WeightChangeArgs : EventArgs
{
    public float oldWeight = 0.0f;
    public float newWeight = 0.0f;

    public WeightChangeArgs(float old, float newWeight)
    {
        oldWeight = old;
        this.newWeight = newWeight;
    }
}
