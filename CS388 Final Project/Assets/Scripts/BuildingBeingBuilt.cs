using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingBeingBuilt : MonoBehaviour
{
    public GameObject phase0, phase1, phase2;
    public GameObject text;
    public float timeToBuild;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        t = 0;
        phase1.SetActive(false);
        phase2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if(t > timeToBuild / 2)
        {
            phase1.SetActive(true);
        }
        if(t > timeToBuild)
        {
            phase2.SetActive(true);
            phase0.SetActive(false);
            text.SetActive(false);
            return;
        }


        text.GetComponent<TextMeshPro>().SetText(((int)(timeToBuild - t)).ToString());
    }
}
