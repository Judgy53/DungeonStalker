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

    //Can't extend static method in C# ...
    public static T GetComponentWithTag<T>(this GameObject go, string tag)
    {
        GameObject f = GameObject.FindGameObjectWithTag(tag);
        if (f != null)
            return f.GetComponent<T>();
        return default(T);
    }

    public static T GetComponentInChildrenWithTag<T>(this GameObject go, string tag)
    {
        GameObject f = GameObject.FindGameObjectWithTag(tag);
        if (f != null)
            return f.GetComponentInChildren<T>();
        return default(T);
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

    public override bool Equals(object other)
    {
        return this == (Vector2i)other;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Vector2i a, Vector2i b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator != (Vector2i a, Vector2i b)
    {
        return a.x != b.x || a.z != b.z;
    }

    public override string ToString()
    {
        return "[" + x + ", " + z + "]";
    }
}

public static class Vector3Extension
{
    public static void ToSaveData(this Vector3 vector, SaveData data, string name)
    {
        data.Add(name + "X", vector.x);
        data.Add(name + "Y", vector.y);
        data.Add(name + "Z", vector.z);
    }

    public static Vector3 FromSaveData(this Vector3 vector, SaveData data, string name)
    {
        vector.x = float.Parse(data.Get(name + "X"));
        vector.y = float.Parse(data.Get(name + "Y"));
        vector.z = float.Parse(data.Get(name + "Z"));

        return vector;
    }

    public static Vector3 RotateAround(this Vector3 vector, Vector3 pivot, Quaternion angle)
    {
        return angle * (vector - pivot) + pivot;
    }
}

public static class LayerMaskExtension
{
    public static bool IsInLayer(this LayerMask mask, int layer)
    {
        return ((mask.value & (1 << layer)) > 0);
    }
}
