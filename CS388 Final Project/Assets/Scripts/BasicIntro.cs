using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class BasicIntro : MonoBehaviour
{
    public bool init = false;
    public bool playing = false;
    public Vector3 angle;
    public Camera Cam;
    float distance = 1.0f;
    GridEnvironment environment = null;
    WorldSpawner Spawner = null;
    public Inventory inventory;
    Vector2 swipeSize;
    public ModeSwitch mode;

    public TextMeshProUGUI selection_text;
    public GameObject PlantButton;
    public GameObject ChopButton;
    SavedObject selected_object;
    Vector3Int selected_position;

    // For Buildings
    bool build = false;
    enum buildingType { House }
    buildingType buildThis;

    // Start is called before the first frame update
    void Start()
    {
        swipeSize = new Vector2(Screen.width, Screen.height);

        if (init)
            LoadScene(1);
        else
        {
            if (selection_text)
            {
                selection_text.text = "";
                PlantButton.SetActive(false);
                ChopButton.SetActive(false);
            }
            environment = FindObjectOfType<GridEnvironment>();
            Spawner = FindObjectOfType<WorldSpawner>();
            if (environment)
            {
                transform.position = new Vector3(environment.size / 2, 0, environment.size / 2);
                distance = environment.size / 250.0f;
            }
        }
    }

    Vector2 StartPos;
    Vector3 startRot;
    Vector3 startAngle;
    void RotateMode()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartPos = touch.position;
                startRot = transform.eulerAngles;
                startAngle = angle;
                break;
            case TouchPhase.Moved:
                Vector2 distance = new Vector2((touch.position.x - StartPos.x) / swipeSize.x, (touch.position.y - StartPos.y) / swipeSize.y);
                transform.eulerAngles = new Vector3(startRot.x, startRot.y + distance.x * 180.0f, startRot.z);
                angle = new Vector3(startAngle.x, startAngle.y - distance.y * 100.0f, startAngle.z);
                break;
        }
    }

    Vector3 StartLoc;
    void PanMode()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartPos = touch.position;
                StartLoc = transform.position;
                break;
            case TouchPhase.Moved:
                Vector2 distance = new Vector2((touch.position.x - StartPos.x) / swipeSize.x, (touch.position.y - StartPos.y) / swipeSize.y);
                transform.position = StartLoc - new Vector3(Cam.transform.forward.x,0, Cam.transform.forward.z) * 25.0f * distance.y - Cam.transform.right * 25.0f * distance.x;
                break;
        }
    }

    float StartDist;
    Vector2 StartTouchDist;
    void ZoomMode()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        switch (touch2.phase)
        {
            case TouchPhase.Began:
                StartTouchDist = new Vector2((touch1.position.x - touch2.position.x) / swipeSize.x, (touch1.position.y - touch2.position.y) / swipeSize.y);
                StartDist = distance;
                break;
            case TouchPhase.Moved:
                Vector2 new_touch_dist = new Vector2((touch1.position.x - touch2.position.x) / swipeSize.x, (touch1.position.y - touch2.position.y) / swipeSize.y);
                float difference = new_touch_dist.magnitude - StartTouchDist.magnitude;
                distance = StartDist - 0.5f * difference;
                break;
        }
        switch(touch1.phase)
        {
            case TouchPhase.Moved:
                Vector2 new_touch_dist = new Vector2((touch1.position.x - touch2.position.x) / swipeSize.x, (touch1.position.y - touch2.position.y) / swipeSize.y);
                float difference = new_touch_dist.magnitude - StartTouchDist.magnitude;
                distance = StartDist - 0.5f * difference;
                break;
        }
    }

    void SelectMode()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartPos = touch.position;
                break;
            case TouchPhase.Ended:
                Vector2 distance_2 = new Vector2((touch.position.x - StartPos.x) / swipeSize.x, (touch.position.y - StartPos.y) / swipeSize.y);
                if (distance_2.magnitude < 0.1f)
                {
                    Ray ray = Cam.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
                    {
                        PlantButton.SetActive(false);
                        ChopButton.SetActive(false);
                        selected_position = Vector3Int.RoundToInt(raycastHit.point);
                        selected_object = Spawner.CheckValidOccupation(selected_position.x, selected_position.z, SavedObject.Shape.Single);
                        if (selected_object != null)
                        {
                            selection_text.text = selected_object.objName + " (" + selected_position.x + "," + selected_position.z + ")";
                            ChopButton.SetActive(true);
                        }
                        else
                        {
                            selection_text.text = environment.grid[selected_position.x, selected_position.z].GetName() + " (" + selected_position.x + "," + selected_position.z + ")";
                            if (environment.grid[selected_position.x, selected_position.z].GetName() == "Grass")
                                PlantButton.SetActive(true);
                        }

                        // For buildings
                        if(build)
                        {
                            BuildABuilding();
                        }
                    }
                    else
                    {
                        selected_object = null;
                        selected_position = new Vector3Int(-1, -1, -1);
                        selection_text.text = "";
                    }
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                switch (mode.Current_Mode)
                {
                    case ModeSwitch.Mode.rotate:
                        RotateMode();
                        break;
                    case ModeSwitch.Mode.pan:
                        PanMode();
                        break;
                }
                if(playing)
                    SelectMode();
            }
            else if(Input.touchCount == 2)
                ZoomMode();
            Cam.transform.localPosition = angle * distance;
            Cam.transform.LookAt(transform);
        }
    }

    public void Chop()
    {
        Spawner.DestroyStructure(selected_object);
        selected_object = null;
        selected_position = new Vector3Int(-1, -1, -1);
        selection_text.text = "";
        ChopButton.SetActive(false);

        // Add wood to inventory
        inventory.AddResource(Inventory.ResourceType.Wood, 5);

    }
    public void Plant()
    {
        Spawner.SpawnStructure(selected_position.x, selected_position.z, "Tree", 0, environment.grid[selected_position.x, selected_position.z].position, true);
        selected_object = null;
        selected_position = new Vector3Int(-1, -1, -1);
        selection_text.text = "";
        PlantButton.SetActive(false);
    }

    public void Building1()
    {
        // Check if enough resources
        if(inventory.ConsumeResource(new Inventory.ResourceType[]{ Inventory.ResourceType.Wood}, new int[] { 1 }) == false)
        {
            return;
        }

        if(build == true && buildThis == buildingType.House)
        {
            build = false;
        }

        build = true;
        buildThis = buildingType.House;
    }

    public void BuildABuilding()
    {
        if(buildThis == buildingType.House)
        {
            Spawner.SpawnStructure(selected_position.x, selected_position.z, "Building1", 0, environment.grid[selected_position.x, selected_position.z].position, false);
            selected_object = null;
            selected_position = new Vector3Int(-1, -1, -1);
            selection_text.text = "";
            PlantButton.SetActive(false);
        }
        build = false;
    }

    public void LoadScene(int scene_num)
    {
        SceneManager.LoadScene(scene_num);
    }

    public void Reduce()
    {
        distance += 0.1f;
    }

    public void Increase()
    {
        distance -= 0.1f;
    }

    public void RegenerateIsland(int value = 0)
    {
        if(value != 0)
            PlayerPrefs.SetInt("Map_Scale", value);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveThenMenu()
    {
        Save(false);
        LoadScene(1);
    }

    public void Save(bool and_load)
    {
        if (environment)
        {
            int saveFile = PlayerPrefs.GetInt("Save Existing Seed");
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/map" + saveFile + ".island";
            FileStream stream = new FileStream(path, FileMode.Create);

            SaveFile data = new SaveFile(environment);
            formatter.Serialize(stream, data);
            stream.Close();
            if (and_load)
                Load();
        }
    }

    public void Load()
    {
        int saveFile = PlayerPrefs.GetInt("Save Existing Seed");
        string path = Application.persistentDataPath + "/map" + saveFile + ".island";
        if (File.Exists(path))
        {
            PlayerPrefs.SetInt("Load Existing Seed", saveFile);
            SceneManager.LoadScene(3);
        }
    }
}
