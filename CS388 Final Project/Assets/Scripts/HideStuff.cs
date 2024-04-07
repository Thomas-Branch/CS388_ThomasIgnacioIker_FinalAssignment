using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HideStuff : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject[] active_objects;
    public string active_text;
    public string inactive_text;
    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateText()
    {
        if (text)
        {
            if (active_objects[0].activeSelf)
                text.text = active_text;
            else
                text.text = inactive_text;
        }
    }

    public void toggleActivity()
    {
        for (int i = 0; i < active_objects.Length; i++)
            active_objects[i].SetActive(!active_objects[i].activeSelf);
        UpdateText();
    }

    public void LoadScene(int scene_num)
    {
        SceneManager.LoadScene(scene_num);
    }

    public void NowLoadScene(int size)
    {
        PlayerPrefs.SetInt("Map_Scale", size);
        LoadScene(2);
    }
}
