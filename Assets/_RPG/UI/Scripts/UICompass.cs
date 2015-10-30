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

            RawImage image = GetComponent<RawImage>();
            Rect rect = image.uvRect;

            angle += 360f * ((1 - rect.width) / 2f);

            float angleRatio = angle / 360f;

            rect.x = angleRatio;
            image.uvRect = rect;
        }
	}
}
