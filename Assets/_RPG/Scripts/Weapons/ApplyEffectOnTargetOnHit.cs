using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IWeapon))]
public class ApplyEffectOnTargetOnHit : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float probablity = 1.0f;

    public bool canStack = false;

    public ScriptableObject effect = null;

    private void Start()
    {
        if (effect == null)
        {
            Debug.LogError("No effect assigned to " + this.name);
            enabled = false;
        }

        if (effect is IEffect)
        {
            IWeapon weapon = GetComponent<IWeapon>();
            if (weapon != null)
                weapon.OnHit += OnHitCallback;
        }
        else
        {
            Debug.LogError(effect.name + "is not an effect");
            this.enabled = false;
        }
    }

    private void OnHitCallback(object sender, OnHitArgs args)
    {
        float rand = Random.Range(0.0f, 1.0f);
        if (rand <= probablity)
        {
            IEffect newEffect = (effect as IEffect).Clone();
            EffectManager m = (args.target as Behaviour).GetComponent<EffectManager>();
            if (m != null && !m.HasEffect(newEffect.Name))
                m.AddEffect(newEffect);
        }
    }
}
