using UnityEngine;
using System;
using System.Collections;

public class UITransfer : MonoBehaviour
{
    [SerializeField]
    private GameObject content = null;

    private UITransferState state = UITransferState.Free;
    public UITransferState State { get { return state; } 
        private set 
        {
            state = value;

            if (OnStateChange != null)
                OnStateChange(this, new EventArgs());
        } 
    }

    public event EventHandler OnStateChange;

    [SerializeField]
    private UIInventoryToggleLister listerA = null;
    [SerializeField]
    private UIInventoryToggleLister listerB = null;

    public void OpenUI(IContainer a, IContainer b)
    {
        if (UIStateManager.State == UIState.Free && state == UITransferState.Free)
        {
            UIStateManager.RegisterUI();
            listerA.Clear();
            listerA.Populate(a);
            listerB.Clear();
            listerB.Populate(b);
            content.SetActive(true);
            Time.timeScale = 0.0f;

            State = UITransferState.Busy;
        }
    }

    public void CloseUI()
    {
        if (state == UITransferState.Busy)
        {
            UIStateManager.UnregisterUI();
            listerA.Clear();
            listerB.Clear();
            content.SetActive(false);
            Time.timeScale = 1.0f;

            State = UITransferState.Free;
        }
    }

    private void Start()
    {
        content.SetActive(false);
        state = UITransferState.Free;
    }

    public void AtoBButtonCallback()
    {
        if (!TransferItemAtoB(listerA.SelectedItem))
            listerB.PlayWeightAnim();
    }

    public void BtoAButtonCallback()
    {
        if (!TransferItemBtoA(listerB.SelectedItem))
            listerA.PlayWeightAnim();
    }

    public void TransferAllBtoA()
    {
        IItem[] items = listerB.Target.Items;
        foreach (IItem i in items)
            TransferItemBtoA(i);

        CloseUI();
    }

    public bool TransferItemAtoB(IItem item)
    {
        bool ret = TransferItem(listerA.Target, listerB.Target, item);
        listerA.Populate(listerA.Target);
        listerB.Populate(listerB.Target);
        return ret;
    }

    public bool TransferItemBtoA(IItem item)
    {
        bool ret = TransferItem(listerB.Target, listerA.Target, item);
        listerA.Populate(listerA.Target);
        listerB.Populate(listerB.Target);

        return ret;
    }

    private static bool TransferItem(IContainer source, IContainer destination, IItem item)
    {
        if (item == null)
            return false;

        if (!source.RemoveItem(item))
        {
            Debug.LogWarning("Item not found in source !");
            return false;
        }

        if (!destination.AddItem(item))
        {
            Debug.Log("Destination is full !");
            source.AddItem(item);
            return false;
        }

        return true;
    }
}

public enum UITransferState
{
    Free,
    Busy
}
