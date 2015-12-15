using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

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

    public static bool HasTagInParent(this GameObject go, string tag)
    {
        Transform current = go.transform;
        do
        {
            if (current.tag == tag)
                return true;
        } while ((current = current.transform.parent) != null);

        return false;
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

[System.Serializable]
public class MinMax<T>
{
    public T min;
    public T max;

    public MinMax(T min, T max)
    {
        this.min = min;
        this.max = max;
    }

    public static implicit operator string(MinMax<T> m)
    {
        return m.min + " - " + m.max;
    }
}

[System.Serializable]
public class IntMinMax : MinMax<int> 
{
    public IntMinMax(int min, int max) : base(min, max) { }
}

[System.Serializable]
public class FloatMinMax : MinMax<float>
{
    public FloatMinMax(float min, float max) : base(min, max) { }
}

public static class CameraExtensions
{
    public static bool GetWorldHitpoint(this Camera camera, Vector3 screenPos, out RaycastHit hit, out Vector3 v, float maxDistance = 10000.0f)
    {
        Ray ray = camera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            v = hit.point;
            return true;
        }

        v = camera.transform.position + ray.direction * maxDistance;
        return false;
    }
}


////////////////////
//Thoses methodes exist for Editor scripts being able to change private Object's members.
//They use reflection
////////////////////
public static class Utils
{
    /// <summary>
    /// Returns a _private_ Property Value from a given Object. Uses Reflection.
    /// Throws a ArgumentOutOfRangeException if the Property is not found.
    /// </summary>
    /// <typeparam name="T">Type of the Property</typeparam>
    /// <param name="obj">Object from where the Property Value is returned</param>
    /// <param name="propName">Propertyname as string.</param>
    /// <returns>PropertyValue</returns>
    public static T GetPrivatePropertyValue<T>(object obj, string propName)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (pi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
        return (T)pi.GetValue(obj, null);
    }

    /// <summary>
    /// Returns a private Property Value from a given Object. Uses Reflection.
    /// Throws a ArgumentOutOfRangeException if the Property is not found.
    /// </summary>
    /// <typeparam name="T">Type of the Property</typeparam>
    /// <param name="obj">Object from where the Property Value is returned</param>
    /// <param name="propName">Propertyname as string.</param>
    /// <returns>PropertyValue</returns>
    public static T GetPrivateFieldValue<T>(object obj, string propName)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        Type t = obj.GetType();
        FieldInfo fi = null;
        while (fi == null && t != null)
        {
            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            t = t.BaseType;
        }
        if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
        return (T)fi.GetValue(obj);
    }

    /// <summary>
    /// Sets a _private_ Property Value from a given Object. Uses Reflection.
    /// Throws a ArgumentOutOfRangeException if the Property is not found.
    /// </summary>
    /// <typeparam name="T">Type of the Property</typeparam>
    /// <param name="obj">Object from where the Property Value is set</param>
    /// <param name="propName">Propertyname as string.</param>
    /// <param name="val">Value to set.</param>
    /// <returns>PropertyValue</returns>
    public static void SetPrivatePropertyValue<T>(object obj, string propName, T val)
    {
        Type t = obj.GetType();
        if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
        t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
    }

    /// <summary>
    /// Set a private Property Value on a given Object. Uses Reflection.
    /// </summary>
    /// <typeparam name="T">Type of the Property</typeparam>
    /// <param name="obj">Object from where the Property Value is returned</param>
    /// <param name="propName">Propertyname as string.</param>
    /// <param name="val">the value to set</param>
    /// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
    public static void SetPrivateFieldValue<T>(object obj, string propName, T val)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        Type t = obj.GetType();
        FieldInfo fi = null;
        while (fi == null && t != null)
        {
            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            t = t.BaseType;
        }
        if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
        fi.SetValue(obj, val);
    }
}