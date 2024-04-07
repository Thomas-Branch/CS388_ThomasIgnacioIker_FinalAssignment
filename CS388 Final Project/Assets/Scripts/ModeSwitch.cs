using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ModeSwitch : MonoBehaviour
{
    public TextMeshProUGUI text;
    public enum Mode { rotate, pan, end, zoom, select };
    public Mode Current_Mode = Mode.rotate;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "rotate";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle()
    {
        Current_Mode = (Mode)((int)(Current_Mode + 1) % (int)Mode.end);
        switch(Current_Mode)
        {
            case Mode.rotate:
                text.text = "rotate";
                break;
            case Mode.pan:
                text.text = "pan";
                break;
            case Mode.zoom:
                text.text = "zoom";
                break;
            case Mode.select:
                text.text = "select";
                break;
        }
    }
}
