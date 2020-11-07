using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Dictionary<int, GameObject> Items;

    private static int index;

    public GameObject ItemPrefab;

    public int SpawnFrequency;

    void Start()
    {
        Items = new Dictionary<int, GameObject>();
        index = 0;
        StartCoroutine(CoroutineSpawnItem());
    }

    private IEnumerator CoroutineSpawnItem()
    {
        while (true)
        {
            SpawnItem();
            yield return new WaitForSeconds(SpawnFrequency);
        }
    }

    private void SpawnItem()
    {
        GameObject Instantiated = Instantiate(ItemPrefab, transform);
        Instantiated.GetComponent<Ball>().Init(index);
        Items.Add(index, Instantiated);
        ServerSend.SpawnObject(index, transform.position);
        index++;
    }
}
