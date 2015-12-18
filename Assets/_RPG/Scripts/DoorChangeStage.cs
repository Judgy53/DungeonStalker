using UnityEngine;
using System.Collections;

public class DoorChangeStage : MazeCellEdge, ISavable
{
    [SerializeField]
    private uint delta = 1;
    public uint Delta { get { return delta; } set { delta = value; } }

    public Transform[] toColor = new Transform[0];

    private UsableButton usable = null;

    public string lockedDescription = "It's locked !";
    public string unlockedDescription = "It's open now, but I won't be able to go back if I go through this door !";

    [SerializeField]
    private bool locked = true;
    public bool Locked
    {
        get { return locked; }
        set
        {
            locked = value;
            UpdateUsableDescription();
        }
    }

    public override void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        base.Initialize(cell, otherCell, direction);
        foreach (Transform t in toColor)
            t.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;

        usable = GetComponentInChildren<UsableButton>();
        UpdateUsableDescription();
    }

    public void Use(InteractManager interactManager)
    {
        if (locked)
        {
            IContainer container = interactManager.GetComponentInChildren<IContainer>();
            if (container != null)
            {
                IItem[] items = container.Items;
                foreach (IItem item in items)
                {
                    ItemStageKey key = item as ItemStageKey;
                    if (key != null && key.StageValue == GameManager.Stage)
                    {
                        container.RemoveItem(key);
                        GameObject.Destroy(key);
                        Locked = false;
                        return;
                    }
                }
            }

            return;
        }

        GameManager.LoadStage(GameManager.Stage + delta);
    }

    private void UpdateUsableDescription()
    {
        if (usable != null)
        {
            if (locked)
                usable.actionDescription = lockedDescription;
            else
                usable.actionDescription = unlockedDescription;
        }
    }

    public void Save(SaveData data)
    {
        data.Add("locked", locked);
        data.Add("delta", delta);
    }

    public void Load(SaveData data)
    {
        Locked = bool.Parse(data.Get("locked"));
        Delta = uint.Parse(data.Get("delta"));
    }
}
