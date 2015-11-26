using UnityEngine;
using System.Collections;

public class OnRenderImageCallback : MonoBehaviour
{
    public OverlayManager manager = null;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (manager != null)
            manager.OnRenderImage(source, destination);
    }
}
