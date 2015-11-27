using UnityEngine;
using System.Collections;

public abstract class AnimationDriverBase : MonoBehaviour
{
    [SerializeField]
    protected Animator animator = null;
    public Animator Animator { get { return animator; } }

    private void Awake()
    {
        if (animator == null)
        {
            enabled = false;
            throw new System.InvalidOperationException("No animator defined on " + this.name);
        }

        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart() { }
    protected virtual void OnAwake() { }

    public virtual void MainHandPrimary() { }
    public virtual void MainHandEndPrimary() { }
    public virtual void MainHandSecondary() { }
    public virtual void MainHandEndSecondary() { }

    public virtual void OffHandPrimary() { }
    public virtual void OffHandEndPrimary() { }
    public virtual void OffHandSecondary() { }
    public virtual void OffHandEndSecondary() { }

    public virtual void SetHandsRestrictions(IWeapon weapon) { }
    public virtual void SetWeaponType(IWeapon weapon, WeaponRestriction hand) { }

    public virtual void SetSpeed(IWeapon weapon, WeaponRestriction hand) { }

    public virtual void OnHit(object sender, System.EventArgs args) { }
    public virtual void OnDeath(object sender, System.EventArgs args) { }

    public virtual void SetMovementVelocity(float forwardVelocity, float lateralVelocity, float airVelocity, bool grounded) { }
}
