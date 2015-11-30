using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IDamageable))]
[RequireComponent(typeof(LootTable))]
public class GenerateLootOnDeath : MonoBehaviour
{
    private LootTable table = null;

    public void Start()
    {
        IDamageable dmg = GetComponent<IDamageable>();
        table = GetComponent<LootTable>();
        dmg.OnDeath += dmg_OnDeath;
    }

    void dmg_OnDeath(object sender, System.EventArgs e)
    {
        table.GenerateLoot();
    }
}
