using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ConstructLogic : MonoBehaviour
{
    public SavedObject obj;
    public int fella_count = 0;

    public GameObject[] animations;
    bool firstFrame = true;

    int currentAnim;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < animations.Length; i++)
        {
            animations[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(firstFrame)
        {
            firstFrame = false;

            if (obj.extra == 5) // Building1 (FellaGenerator)
            {
                currentAnim = 0;
            }
            if (obj.extra == 7) // Building2
            {
                currentAnim = 1;
            }
            if (obj.extra == 8) // Building3
            {
                currentAnim = 2;
            }
            animations[currentAnim].SetActive(true);
        }

        TimedEvent e;
        if(e = obj.event_happening)
        {
            animations[currentAnim].GetComponent<BuildingBeingBuilt>().UpdateTime(e.GetTimePercentLeft());
        }

        if (obj.event_happened)
        {
            WorldSpawner spawner = FindObjectOfType<WorldSpawner>();
            Inventory inv = FindObjectOfType<Inventory>();
            spawner.DestroyStructure(obj);
            spawner.SpawnStructure(obj.x, obj.y, obj.extra, 0, obj.transform.position, true);
            if (inv)
                inv.AddResource(Inventory.ResourceType.Fellas, spawner.FindOccupier(obj.x, obj.y).used_fellas);
        }
    }
}
