using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FellaBuildingEventHandler : MonoBehaviour
{
    public WorldSpawner worldSpawner;
    public Inventory inventory;
    bool canCollect = true;
    TimedEvent tevent;
    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        worldSpawner = FindObjectOfType<WorldSpawner>();
        //worldSpawner.SpawnEvent(300, this.gameObject.GetComponent<SavedObject>().x, this.gameObject.GetComponent<SavedObject>().y);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<SavedObject>().event_happened)
        {
            canCollect = true;
        }
    }

    public bool Collect()
    {
        if(canCollect)
        {
            worldSpawner.SpawnEvent(300, this.gameObject.GetComponent<SavedObject>().x, this.gameObject.GetComponent<SavedObject>().y);
            inventory.AddResource(Inventory.ResourceType.Fellas, 2);
            this.gameObject.GetComponent<SavedObject>().event_happened = false;
            canCollect = false;
            return true;
        }

        return false;
    }
}
