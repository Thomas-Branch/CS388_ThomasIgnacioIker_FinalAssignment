using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ConstructLogic : MonoBehaviour
{
    public SavedObject obj;
    public int fella_count = 0;

    public GameObject phase0;
    public GameObject[] phases;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < phases.Length; i++)
        {
            phases[i].SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        TimedEvent e;
        if (e = obj.event_happening)
        {
            //if (e.GetTimePercentLeft() > (long)100)
            //{
            //    for (int i = 0; i < phases.Length; i++)
            //    {
            //        phases[i].SetActive(true);
            //    }
            //    phase0.SetActive(false);
            //    return;
            //}
            Debug.Log(e.GetTimePercentLeft());
            for (int i = 0; i < phases.Length; i++)
            {
                if (e.GetTimePercentLeft() > ((long)100) / phases.Length)
                {
                    phases[i].SetActive(true);
                }

            }
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
