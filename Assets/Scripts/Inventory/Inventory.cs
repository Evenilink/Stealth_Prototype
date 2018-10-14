using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    private int totalSize;
    private int remainingSize;
    private List<BaseItem> items = new List<BaseItem>();

    public void Add(BaseItem item) {
        if (item.size >= remainingSize) {
            items.Add(item);
        }
    }

    public void Remove(BaseItem item) {
        items.Remove(item);
    }
}
