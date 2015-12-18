using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAutoSave : MonoBehaviour {

    [SerializeField]
    private GameObject textObject;

    [SerializeField]
    private float textFullOpacity = 1.0f;

    [SerializeField]
    private float outlineFullOpacity = 0.5f;

    [SerializeField]
    private float fadeDuration = 1.0f;

    [SerializeField]
    private float fullOpacityDuration = 3.0f;

	private void Start () {
        AutoSaver.OnAutoSave += AutoSaver_OnAutoSave;

        SetTextOpacity(0f);
	}

    private void OnDestroy()
    {
        AutoSaver.OnAutoSave -= AutoSaver_OnAutoSave;
    }

    private void AutoSaver_OnAutoSave(object sender, System.EventArgs e)
    {
        StartCoroutine("AutoSaveToolTip");
    }

    private IEnumerator AutoSaveToolTip()
    {
        float loopMax = fadeDuration * 20f;

        for(float i = 1f; i <= loopMax; i++)
        {
            SetTextOpacity(i / loopMax);
            yield return new WaitForSeconds(fadeDuration / loopMax);
        }

        yield return new WaitForSeconds(fullOpacityDuration);

        for (float j = 1f; j <= loopMax; j++)
        {
            SetTextOpacity(1f - (j / loopMax));
            yield return new WaitForSeconds(fadeDuration / loopMax);
        }
    }

    private void SetTextOpacity(float opacity)
    {
        Text textComp = textObject.GetComponent<Text>();

        Color textColor = textComp.color;
        textColor.a = opacity * textFullOpacity;

        textComp.color = textColor;


        Outline outlineComp = textObject.GetComponent<Outline>();

        Color outlineColor = outlineComp.effectColor;
        textColor.a = opacity * outlineFullOpacity;

        outlineComp.effectColor = outlineColor;
    }
}
