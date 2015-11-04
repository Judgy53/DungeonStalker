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

[System.Serializable]
public class Vector2i
{
    public int x, z;

    public Vector2i(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static Vector2i operator +(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x + b.x, a.z + b.z);
    }
}
