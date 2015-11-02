using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerContainer : MonoBehaviour, IContainer, IStatsDependable
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

    public void RemoveItem(IItem item)
    {
        Behaviour itemBehaviour = (item as Behaviour);
        if (itemBehaviour.transform.IsChildOf(transform))
        {
            itemBehaviour.transform.SetParent(null, false);
            CurrentWeight -= item.Weigth;
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
                    GameObject pickableGo = GameObject.Instantiate(item.DropPrefab, transform.position + transform.forward * 2.0f, Quaternion.identity) as GameObject;
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
