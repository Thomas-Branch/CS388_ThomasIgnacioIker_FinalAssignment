using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsMenu : MonoBehaviour
{
    bool isMenuUp;
    bool lerpMenu;
    public float upPos, downPos;
    float startPos, endPos, t;
    public RectTransform menuBackground;

    // Start is called before the first frame update
    void Start()
    {
        isMenuUp = false;
        lerpMenu = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(lerpMenu)
        {
            LerpMenu();
        }
    }

    public void ToggleBuildingMenu()
    {
        if(lerpMenu)
        {
            return;
        }

        if(!isMenuUp)
        {
            isMenuUp = true;
            startPos = downPos;
            endPos = upPos;
        }
        else
        {
            isMenuUp = false;
            startPos = upPos;
            endPos = downPos;
        }

        t = 0;
        lerpMenu = true;
    }

    public void LerpMenu()
    {
        t += Time.deltaTime*3;
        if (t >= 1)
        {
            lerpMenu = false;
            menuBackground.position = new Vector3(menuBackground.position.x, endPos, menuBackground.position.z);
            return;
        }
        menuBackground.position = new Vector3(menuBackground.position.x, Mathf.Lerp(startPos, endPos, t), menuBackground.position.z);
    }
}
