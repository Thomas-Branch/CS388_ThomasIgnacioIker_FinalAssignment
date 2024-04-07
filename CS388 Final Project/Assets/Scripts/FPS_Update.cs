using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS_Update : MonoBehaviour
{
    public TextMeshProUGUI text;
    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            text.text = "" + (int)(1.0f / Time.deltaTime) + " FPS";
            timer += 0.25f;
        }
    }
}
