using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockLogic : MonoBehaviour
{
    public SavedObject obj;
    // Start is called before the first frame update
    void Start()
    {
        GridEnvironment environment = FindObjectOfType<GridEnvironment>();
        environment.RockCount++;
    }

    // Update is called once per frame
    void Update()
    {
        if (obj.event_happened)
        {
            WorldSpawner spawner = FindObjectOfType<WorldSpawner>();
            Inventory inv = FindObjectOfType<Inventory>();
            if (inv)
            {
                int total = Random.Range(6, 10);
                int rocks = Random.Range(total - 2, total);
                inv.AddResource(Inventory.ResourceType.Rock, rocks);
                inv.AddResource(Inventory.ResourceType.Metal, total - rocks);
                inv.AddResource(Inventory.ResourceType.Fellas, 3);
            }
            GridEnvironment environment = FindObjectOfType<GridEnvironment>();
            environment.RockCount--;
            spawner.DestroyStructure(obj);
        }
    }
}
