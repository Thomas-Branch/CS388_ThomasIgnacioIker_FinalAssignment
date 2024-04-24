using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLogic : MonoBehaviour
{
    public SavedObject obj;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(obj.event_happened)
        {
            WorldSpawner spawner = FindObjectOfType<WorldSpawner>();
            spawner.DestroyStructure(obj);
            Inventory inv = FindObjectOfType<Inventory>();
            if (inv)
            {
                inv.AddResource(Inventory.ResourceType.Wood, 5);
                inv.AddResource(Inventory.ResourceType.Fellas, 2);
            }
        }
    }
}
