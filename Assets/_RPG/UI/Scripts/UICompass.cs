using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICompass : MonoBehaviour {

    [SerializeField]
    private GameObject target = null;

	// Use this for initialization
	private void Start () {
        if (target == null)
            Debug.LogWarning("Compass' target not set.");
	}
	
	// Update is called once per frame
	private void Update () {
	    if(target != null)
        {
            float angle = target.transform.eulerAngles.y;

            float angleRatio = angle / 360f;

            RawImage image = GetComponent<RawImage>();

            Rect rect = image.uvRect;
            rect.x = angleRatio;
            image.uvRect = rect;
        }
	}
}
