using UnityEngine;
using System.Collections;

public class RandomPitcher : MonoBehaviour {

    [SerializeField]
    private float pitchRandomness = 0.02f;

	void Start () {
        GetComponent<AudioSource>().pitch += Random.Range(-pitchRandomness, pitchRandomness);
	}
}
