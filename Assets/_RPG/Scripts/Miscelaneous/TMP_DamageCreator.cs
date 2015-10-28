using UnityEngine;
using System.Collections;

public class TMP_DamageCreator : MonoBehaviour {

    [SerializeField]
    private HealthManager target = null;
	
	void Update () {
        if(target == null)
            return;

	    if(Input.GetKeyDown(KeyCode.Q))
            target.AddDamage(Random.Range(0f, 2f));
        else if (Input.GetKeyDown(KeyCode.W))
            target.Heal(Random.Range(0f, 2f));
	}
}
