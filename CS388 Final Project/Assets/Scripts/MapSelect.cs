using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class MapSelect : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject x;
    public int MapNum = 1;
    bool Has_Save = false;
    public HideStuff size_selection;
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/map" + MapNum + ".island";
        if (File.Exists(path))
        {
            text.text = "Open Map " + MapNum;
            Has_Save = true;
        }
        else
            x.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(int scene_num)
    {
        SceneManager.LoadScene(scene_num);
    }

    public void Open()
    {
        PlayerPrefs.SetInt("Save Existing Seed", MapNum);
        if (Has_Save)
        {
            PlayerPrefs.SetInt("Load Existing Seed", MapNum);
            LoadScene(3);
        }
        else
        {
            size_selection.toggleActivity();
        }
    }

    public void delete_save()
    {
        File.Delete(Application.persistentDataPath + "/map" + MapNum + ".island");
        LoadScene(1);
    }
}
