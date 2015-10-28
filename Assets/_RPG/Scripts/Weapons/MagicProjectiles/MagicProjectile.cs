using UnityEngine;
using System.Collections;

public abstract class MagicProjectile : MonoBehaviour
{
    protected GameObject launcher = null;
    public GameObject Launcher { get { return launcher; } set { launcher = value; } }

    protected float power = 0f;
    public float Power { get { return power; } set { power = value; } }
}
