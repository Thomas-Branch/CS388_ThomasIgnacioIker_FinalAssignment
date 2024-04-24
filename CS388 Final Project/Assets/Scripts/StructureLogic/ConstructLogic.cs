using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructLogic : MonoBehaviour
{
    public SavedObject obj;
    public int fella_count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (obj.event_happened)
        {
            WorldSpawner spawner = FindObjectOfType<WorldSpawner>();
            spawner.DestroyStructure(obj);
            spawner.SpawnStructure(obj.x, obj.y, obj.extra, 0, obj.transform.position, true);
            Inventory inv = FindObjectOfType<Inventory>();
            if (inv)
                inv.AddResource(Inventory.ResourceType.Fellas, spawner.FindOccupier(obj.x, obj.y).used_fellas);
        }
    }
}
