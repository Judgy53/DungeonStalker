using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IDamageable))]
public class OnDeathDurabilityManager : MonoBehaviour
{
    [System.Serializable]
    private class DurableScript
    {
        [Tooltip("This script will have its 'enable' variable set to 'value' on the object's death.")]
        public Object script = null;

        public bool value = false;

        public DurableScript() 
        {
        }

        public DurableScript(Object s, bool v)
        {
            script = s;
            value = v;
        }
    }

    [SerializeField]
    private DurableScript[] onDeath = new DurableScript[0];

    public void Awake()
    {
        IDamageable damageable = GetComponent<IDamageable>();
        damageable.OnDeath += OnDeath;
    }

    void OnDeath(object sender, System.EventArgs args)
    {
        foreach (DurableScript s in onDeath)
        {
            Behaviour script = s.script as Behaviour;
            if (script != null)
            {
                script.enabled = s.value;
                continue;
            }

            Collider col = s.script as Collider;
            if (col != null)
            {
                col.enabled = s.value;
                continue;
            }

            GameObject go = s.script as GameObject;
            if (go != null)
            {
                go.SetActive(s.value);
                continue;
            }

            Debug.LogWarning("Invalid object given in " + this.name + ". Supported objects are : Behaviours, Colliders, and GameObjects.");
        }
    }
}
