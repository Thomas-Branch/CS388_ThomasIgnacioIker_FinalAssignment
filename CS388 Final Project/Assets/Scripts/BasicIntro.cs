using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
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
    public GameObject MineButton;
    public GameObject DigButton;
    public GameObject CollectButton;
    SavedObject selected_object;
    Vector3Int selected_position;

    // For Buildings
    bool build = false;
    public enum buildingType {Null, Building1, Building2, Building3, Building4, Building5, Building6}
    buildingType buildThis;

    // Start is called before the first frame update
    void Start()
    {
        swipeSize = new Vector2(Screen.width, Screen.height);

        if (init)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            LoadScene(1);
        }
        else
        {
            if (selection_text)
            {
                selection_text.text = "";
                PlantButton.SetActive(false);
                DigButton.SetActive(false);
                ChopButton.SetActive(false);
                MineButton.SetActive(false);
                CollectButton.SetActive(false);
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
                angle = new Vector3(startAngle.x, Mathf.Max(2,startAngle.y - distance.y * 100.0f), startAngle.z);
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
                distance = Mathf.Max(0.1f,StartDist - 0.5f * difference);
                break;
        }
        switch(touch1.phase)
        {
            case TouchPhase.Moved:
                Vector2 new_touch_dist = new Vector2((touch1.position.x - touch2.position.x) / swipeSize.x, (touch1.position.y - touch2.position.y) / swipeSize.y);
                float difference = new_touch_dist.magnitude - StartTouchDist.magnitude;
                distance = Mathf.Max(0.1f, StartDist - 0.5f * difference);
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
                        DigButton.SetActive(false);
                        ChopButton.SetActive(false);
                        MineButton.SetActive(false);
                        CollectButton.SetActive(false);
                        selected_position = Vector3Int.RoundToInt(raycastHit.point);
                        selected_object = Spawner.CheckValidOccupation(selected_position.x, selected_position.z, SavedObject.Shape.Single);
                        if (selected_object != null)
                        {
                            selection_text.text = selected_object.objName + " (" + selected_position.x + "," + selected_position.z + ")";
                            if (selected_object.objName == "Fella Generator")
                            {
                                CollectButton.SetActive(true);
                            }
                            else if(selected_object.objName == "Tree")
                            {
                                ChopButton.SetActive(true);
                            }
                            else if(selected_object.objName == "Rock")
                            {
                                MineButton.SetActive(true);
                            }
                            
                        }
                        else
                        {
                            string name = environment.grid[selected_position.x, selected_position.z].GetName();
                            selection_text.text = name + " (" + selected_position.x + "," + selected_position.z + ")";
                            if (name == "Grass")
                            {
                                PlantButton.SetActive(true);
                                DigButton.SetActive(true);
                            }
                            else if(name == "Beach")
                            {
                                DigButton.SetActive(true);
                            }
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
            if (Input.touchCount == 1 && (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) || selection_text == null))
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
        if (inventory.CheckResourceAmount(Inventory.ResourceType.Fellas) >= 2 && Spawner.CheckEventOccupation(selected_position.x, selected_position.z) == null)
        {
            inventory.AddResource(Inventory.ResourceType.Fellas, -2);
            Spawner.SpawnEvent(30, selected_position.x, selected_position.z);
            selected_object = null;
            selected_position = new Vector3Int(-1, -1, -1);
            selection_text.text = "";
            ChopButton.SetActive(false);
        }
    }

    public void Mine()
    {
        if (inventory.CheckResourceAmount(Inventory.ResourceType.Fellas) >= 3 && Spawner.CheckEventOccupation(selected_position.x, selected_position.z) == null)
        {
            inventory.AddResource(Inventory.ResourceType.Fellas, -3);
            Spawner.SpawnEvent(120, selected_position.x, selected_position.z);
            selected_object = null;
            selected_position = new Vector3Int(-1, -1, -1);
            selection_text.text = "";
            MineButton.SetActive(false);
        }
    }

    public void Dig()
    {
        if (inventory.CheckResourceAmount(Inventory.ResourceType.Fellas) >= 2 && Spawner.CheckEventOccupation(selected_position.x, selected_position.z) == null)
        {
            inventory.AddResource(Inventory.ResourceType.Fellas, -2);
            Spawner.SpawnEvent(180, selected_position.x, selected_position.z);
            selected_object = null;
            selected_position = new Vector3Int(-1, -1, -1);
            selection_text.text = "";
            DigButton.SetActive(false);
        }
    }

    public void Plant()
    {
        if (Spawner.Construct(selected_position.x, selected_position.z, "Tree", 300, new Inventory.ResourceType[] { Inventory.ResourceType.Fellas, Inventory.ResourceType.Seeds }, new int[] { 1, 1 }))
        {
            selected_object = null;
            selected_position = new Vector3Int(-1, -1, -1);
            selection_text.text = "";
            PlantButton.SetActive(false);
        }
    }

    public void Collect()
    {
        if(selected_object.GetComponent<FellaBuildingLogic>().Collect())
        {
            CollectButton.SetActive(false);
        }
    }

    public void Building(int BuildingID)
    {
        buildingType type = buildingType.Null;

        if(BuildingID == 1)
            type = buildingType.Building1;
        if(BuildingID == 2)
            type = buildingType.Building2;
        if(BuildingID == 3)
            type = buildingType.Building3;
        if(BuildingID == 4)
            type = buildingType.Building4;
        if(BuildingID == 5)
            type = buildingType.Building5;
        if(BuildingID == 6)
            type = buildingType.Building6;

        if (build == true)// && buildThis == type)
        {
            build = false;
            return;
        }

        if(type == buildingType.Building1)
        {
            build = true;
            buildThis = buildingType.Building1;
        }
        if(type == buildingType.Building2)
        {
            build = true;
            buildThis = buildingType.Building2;
        }
        if(type == buildingType.Building3)
        {
            build = true;
            buildThis = buildingType.Building3;
        }
        if(type == buildingType.Building4)
        {
            build = true;
            buildThis = buildingType.Building4;
        }
        if(type == buildingType.Building5)
        {
            build = true;
            buildThis = buildingType.Building5;
        }
        if(type == buildingType.Building6)
        {
            build = true;
            buildThis = buildingType.Building6;
        }
    }

    public void BuildABuilding()
    {
        if (buildThis == buildingType.Building1)
        {
            if (Spawner.Construct(selected_position.x, selected_position.z, "Fella Generator", 120, new Inventory.ResourceType[] { Inventory.ResourceType.Fellas, Inventory.ResourceType.Rock, Inventory.ResourceType.Metal }, new int[] { 6, 25, 5 }))
            {
                selected_object = null;
                selected_position = new Vector3Int(-1, -1, -1);
                selection_text.text = "";
                PlantButton.SetActive(false);
            }
        }

        if (buildThis == buildingType.Building2)
        {
            if (Spawner.Construct(selected_position.x, selected_position.z, "Apartments", 300, new Inventory.ResourceType[] {Inventory.ResourceType.Fellas, Inventory.ResourceType.Wood, Inventory.ResourceType.Rock }, new int[] { 8, 15, 15 }))
            {
                selected_object = null;
                selected_position = new Vector3Int(-1, -1, -1);
                selection_text.text = "";
                PlantButton.SetActive(false);
            }
        }

        if (buildThis == buildingType.Building3)
        {
            if (Spawner.Construct(selected_position.x, selected_position.z, "Fancy House", 420, new Inventory.ResourceType[] {Inventory.ResourceType.Fellas, Inventory.ResourceType.Sand, Inventory.ResourceType.Metal}, new int[] { 10, 10, 10 }))
            {
                selected_object = null;
                selected_position = new Vector3Int(-1, -1, -1);
                selection_text.text = "";
                PlantButton.SetActive(false);
            }
        }

        if (buildThis == buildingType.Building4)
        {
            if (Spawner.Construct(selected_position.x, selected_position.z, "Skyscraper", 600, new Inventory.ResourceType[] {Inventory.ResourceType.Fellas, Inventory.ResourceType.Rock, Inventory.ResourceType.Sand, Inventory.ResourceType.Metal }, new int[] { 20, 30, 30, 10 }))
            {
                selected_object = null;
                selected_position = new Vector3Int(-1, -1, -1);
                selection_text.text = "";
                PlantButton.SetActive(false);
            }
        }

        if (buildThis == buildingType.Building5)
        {
            if (Spawner.Construct(selected_position.x, selected_position.z, "Big Pool", 300, new Inventory.ResourceType[] {Inventory.ResourceType.Fellas, Inventory.ResourceType.Dirt, Inventory.ResourceType.Rock, Inventory.ResourceType.Metal }, new int[] { 12, 10, 15, 5 }))
            {
                selected_object = null;
                selected_position = new Vector3Int(-1, -1, -1);
                selection_text.text = "";
                PlantButton.SetActive(false);
            }
        }

        if (buildThis == buildingType.Building6)
        {
            if (Spawner.Construct(selected_position.x, selected_position.z, "Small House", 60, new Inventory.ResourceType[] {Inventory.ResourceType.Fellas, Inventory.ResourceType.Wood }, new int[] { 4, 20 }))
            {
                selected_object = null;
                selected_position = new Vector3Int(-1, -1, -1);
                selection_text.text = "";
                PlantButton.SetActive(false);
            }
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

    private void OnApplicationQuit()
    {
        if(selection_text)
            Save(false);
    }

    private void OnApplicationFocus()
    {
        if (selection_text)
            Save(false);
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
