using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EffectManager : MonoBehaviour
{
    private List<IEffect> effects = new List<IEffect>();

    private List<IEffect> toRemove = new List<IEffect>();

    public void AddEffect(IEffect effect)
    {
        effects.Add(effect);
    }

    public void RemoveEffect(IEffect effect)
    {
        toRemove.Add(effect);
    }

    public bool HasEffect(string effectName)
    {
        return effects.Find(x => x.Name == effectName) != null;
    }

    public void ClearEffects()
    {
        effects.Clear();
    }

    public T GetEffect<T>() where T : IEffect
    {
        foreach (var effect in effects)
        {
            if (effect is T)
                return (T)effect;
        }

        return default(T);
    }

    public T[] GetEffects<T>() where T : IEffect
    {
        List<T> list = new List<T>();
        foreach (var effect in effects)
        {
            if (effect is T)
                list.Add((T)effect);
        }

        return list.ToArray();
    }

    private void Update()
    {
        foreach (var effect in effects)
        {
            if (!effect.IsInfinite)
                effect.RemainingTime -= Time.deltaTime;

            if (effect.RemainingTime <= 0.0f)
                RemoveEffect(effect);
        }

        foreach (var e in toRemove)
            effects.Remove(e);
    }   
}
