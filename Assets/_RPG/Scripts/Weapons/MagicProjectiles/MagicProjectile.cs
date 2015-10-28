using UnityEngine;
using System.Collections;

public abstract class MagicProjectile : MonoBehaviour
{
    protected ManaManager launcher = null;
    public ManaManager Launcher { get { return launcher; } set { launcher = value; } }

    protected float power = 0f;
    public float Power { get { return power; } set { power = value; } }

    protected float damage = 0f;
    public float Damage { get { return damage; } set { damage = value; } }
}
