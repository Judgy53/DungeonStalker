using UnityEngine;
using System.Collections;

public class OverlayManager : MonoBehaviour
{
    public Overlay[] overlays = new Overlay[Overlays.Count];

    private int poisonOverlayCount = 0;
    public bool IsPoisonOverlayBusy { get { return poisonOverlayCount != 0; } }

    private void Start()
    {
        if (Camera.main != null)
        {
            OnRenderImageCallback cb = Camera.main.GetComponent<OnRenderImageCallback>();
            if (cb != null)
                cb.manager = this;
        }
    }

    public void Update()
    {
        if (overlays[(int)OverlayEnum.Poison] != null)
            overlays[(int)OverlayEnum.Poison].enabled = IsPoisonOverlayBusy;
    }

    public void RegisterPoisonOverlay()
    {
        poisonOverlayCount++;
    }

    public void UnregisterPoisonOverlay()
    {
        if (poisonOverlayCount > 0)
            poisonOverlayCount--;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        foreach (var o in overlays)
        {
            if (o.enabled)
                o.OnRenderImage(source, destination);
        }
    }
}

public enum OverlayEnum : int
{
    Poison = 0
}

public class Overlays
{
    public static readonly int Count = 1;
}