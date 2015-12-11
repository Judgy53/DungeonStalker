using UnityEngine;
using System.Collections;

public class UIInventoryButton : MonoBehaviour {

    private IItem item;
    public IItem Item { get { return item; } set { item = value; } }
    
}
