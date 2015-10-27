using UnityEngine;
using System.Collections;

public static class GameObjectExtension
{
    public static void SetLayerRecursively(this GameObject go, int newLayer)
    {
        go.layer = newLayer;
        foreach (Transform t in go.transform)
            t.gameObject.SetLayerRecursively(newLayer);
    }
}
