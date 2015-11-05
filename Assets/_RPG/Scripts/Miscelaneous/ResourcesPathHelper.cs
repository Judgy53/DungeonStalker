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

    private static string CleanName(string name)
    {
        string weaponName = name;

        if (weaponName.EndsWith("(Clone)"))
            weaponName = weaponName.Remove(weaponName.LastIndexOf('('));

        weaponName.Trim();

        return weaponName;
    }
}
