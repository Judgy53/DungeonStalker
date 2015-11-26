using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EffectManager : MonoBehaviour
{
    private List<IEffect> effects = new List<IEffect>();

    private List<IEffect> toRemove = new List<IEffect>();

    private void OnDestroy()
    {
        foreach (IEffect e in effects)
            e.OnDestroy();
    }

    public void AddEffect(IEffect effect)
    {
        effects.Add(effect);
        effect.OnApply(this);
    }

    public void RemoveEffect(IEffect effect)
    {
        effect.OnDestroy();
        toRemove.Add(effect);
    }

    public bool HasEffect(string effectName)
    {
        return effects.Find(x => x.Name == effectName) != null;
    }

    public void ClearEffects()
    {
        foreach (var e in effects)
            e.OnDestroy();
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

    public IEffect GetEffect(string name)
    {
        foreach (var e in effects)
        {
            if (e.Name == name)
                return e;
        }

        return null;
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

            effect.Update();
        }

        foreach (var e in toRemove)
            effects.Remove(e);

        toRemove.Clear();
    }   
}
