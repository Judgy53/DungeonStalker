using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitAimGatherer : MonoBehaviour, IAimGatherer
{
    [SerializeField]
    private Transform raycastOrigin = null;

    private Unit unit = null;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public Vector3 GetAimHitpoint(ref IDamageable dmg)
    {
        if (unit.Target != null)
        {
            RaycastHit hit;
            Vector3 dir;
            if (Vector3.Angle(transform.forward, unit.Target.position - (raycastOrigin == null ? transform.position : raycastOrigin.position)) < 10.0f)
                dir = unit.Target.position - (raycastOrigin == null ? transform.position : raycastOrigin.position);
            else
                dir = transform.forward;

            if (Physics.Raycast(new Ray((raycastOrigin == null ? transform.position : raycastOrigin.position), dir.normalized), out hit))
            {
                dmg = hit.collider.gameObject.GetComponentInParent<IDamageable>();
                return hit.point;
            }

            return (raycastOrigin == null ? transform.position : raycastOrigin.position) + (dir.normalized * 1000.0f);
        }

        return Vector3.zero;
    }
}
