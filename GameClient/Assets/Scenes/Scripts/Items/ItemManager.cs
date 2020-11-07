using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject Item;

    private Dictionary<int, GameObject> Items;

    private List<int> removedIds;

    private void Awake()
    {
        Items = new Dictionary<int, GameObject>();
        removedIds = new List<int>();
    }
    //TODO add client side prediction to this also, now there is a delay
    public void Spawn(int id, Vector3 position)
    {
        Items.Add(id, Instantiate(Item, position, Quaternion.identity));
    }

    public void UpdatePosition(int id, Vector3 position)
    {
        if (Items.TryGetValue(id, out GameObject obj))
        {
            obj.transform.position = position;
        }
        else if(!removedIds.Contains(id))
        {
            Spawn(id, position);
        }
    }

    public void DestroyItem(int id)
    {
        if(Items.TryGetValue(id, out GameObject obj))
        {
            Items.Remove(id);
            removedIds.Add(id);
            Destroy(obj.gameObject);
        }
    }
}
