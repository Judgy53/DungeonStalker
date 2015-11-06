using UnityEngine;

public class ResourcesPathHelper
{
    public static string GetWeaponPath(WeaponType type, string name)
    {
        string path = "Weapons/";
        path += type.ToString() + "/";
        path += CleanName(name);
        
        return path;
    }

    public static string GetItemPath(string name)
    {
        string path = "Items/";
        //path += type.ToString() + "/"; // Item type would be great
        path += CleanName(name);

        return path;
    }

    public static string GetPickablePath(string name)
    {
        string path = "Pickables/";
        //path += type.ToString() + "/"; // pickable type would be great
        path += CleanName(name);

        return path;
    }

    private static string CleanName(string name)
    {
        string weaponName = name;

        int index = weaponName.LastIndexOf('(');

        if (index > 0)
            weaponName = weaponName.Remove(index);

        weaponName = weaponName.TrimEnd(); // copy in editor adds a space (Instantiate does not)

        return weaponName;
    }
}
