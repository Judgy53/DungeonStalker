using UnityEngine;
using System.Collections;

public class Headbob : MonoBehaviour
{
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;

    public float midpoint = 0.0f;
    private float timer = 0.0f;

    /*private void Start()
    {
        midpoint = transform.localPosition.y;
    }*/

    private void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float sprintMultiplier = Mathf.Lerp(1.0f, 2.0f, Input.GetAxis("Sprint"));
        float bobbingSpeed = this.bobbingSpeed * sprintMultiplier;

        Vector3 cSharpConversion = transform.localPosition;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            timer = 0.0f;
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed * Time.deltaTime;
            if (timer > Mathf.PI * 2)
                timer = timer - (Mathf.PI * 2);
        }
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cSharpConversion.y = midpoint + translateChange;
        }
        else
        {
            cSharpConversion.y = midpoint;
        }

        transform.localPosition = cSharpConversion;
    }
}
