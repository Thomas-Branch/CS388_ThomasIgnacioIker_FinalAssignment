using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildingBeingBuilt : MonoBehaviour
{
    public GameObject phase0;
    public GameObject[] phases;
    public GameObject text;
    public float timeToBuild;
    float t;
    bool built = false;

    // Start is called before the first frame update
    void Start()
    {
        t = 0;
        for(int i = 0; i < phases.Length;i++)
        {
            phases[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!built)
        {
            Build();
        }
    }

    void Build()
    {
        t += Time.deltaTime;
        if (t > timeToBuild)
        {
            for (int i = 0; i < phases.Length; i++)
            {
                phases[i].SetActive(true);
            }
            phase0.SetActive(false);
            text.SetActive(false);
            built = true;
            return;
        }

        for (int i = 0; i < phases.Length; i++)
        {
            if (t > timeToBuild * (i + 1) / phases.Length)
            {
                phases[i].SetActive(true);
            }

        }


        text.GetComponent<TextMeshPro>().SetText(((int)(timeToBuild - t)).ToString());
    }
}
