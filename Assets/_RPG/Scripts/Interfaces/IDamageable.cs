using UnityEngine;
using System.Collections;

public interface IDamageable {

    float Damage { get; }

    void AddDamage(float damage); // Add damage Source (weapon ?)

}
