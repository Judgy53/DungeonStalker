﻿using UnityEngine;
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
}
