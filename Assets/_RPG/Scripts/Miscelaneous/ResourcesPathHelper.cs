using UnityEngine;

public class ResourcesPathHelper
{
    public static string GetWeaponPath(WeaponType type, string name)
    {
        string weapType = type == WeaponType.None ? "Special" : type.ToString();

        string path = "Weapons/";
        path += weapType + "/";
        path += CleanName(name);
        
        return path;
    }

    public static string GetItemPath(IItem item)
    {
        string path = "Items/";
        path += item.Type.ToString() + "/";
        path += CleanName((item as Behaviour).name);

        return path;
    }

    public static string GetPickablePath(Pickable pick)
    {
        string path = null;

        IItem item = pick.PickedItemPrefab.GetComponent<IItem>();

        if (item != null)
        {
            path = "Pickables/";
            path += item.Type.ToString() + "/";
            path += CleanName(pick.name);
        }

        return path;
    }

    public static string GetEnemyPath(string name)
    {
        string path = "Enemies/";
        //path += type.ToString() + "/"; // enemies type ?
        path += CleanName(name);

        return path;
    }

    public static string GetContainerPath(Container cont)
    {
        string path = "Containers/";
        //path += cont.Type.ToString() + "/"; // container type ?
        path += CleanName(cont.name);

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
