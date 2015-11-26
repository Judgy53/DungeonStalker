using UnityEngine;
using System.Collections;

public class OverlayManager : MonoBehaviour
{
    public Overlay[] overlays = new Overlay[Overlays.Count];

    private int poisonOverlayCount = 0;
    public bool IsPoisonOverlayBusy { get { return poisonOverlayCount != 0; } }

    public void Update()
    {
        if (overlays[(int)OverlayEnum.Poison] != null)
            overlays[(int)OverlayEnum.Poison].enabled = IsPoisonOverlayBusy;
    }

    public void RegisterPoisonOverlay()
    {
        poisonOverlayCount++;
        Debug.Log("Registering poison overlay ... Count : " + poisonOverlayCount);
    }

    public void UnregisterPoisonOverlay()
    {
        if (poisonOverlayCount > 0)
            poisonOverlayCount--;
        Debug.Log("Unregistering poison overlay ... Count : " + poisonOverlayCount);
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