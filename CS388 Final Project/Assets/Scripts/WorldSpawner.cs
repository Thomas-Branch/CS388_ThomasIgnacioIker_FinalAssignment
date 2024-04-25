using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SavedObject;

public class WorldSpawner : MonoBehaviour
{
    public GameObject eventPrefab;
    public GameObject[] structures;
    public bool[,] Occupation;
    GridEnvironment environment = null;
    public bool occupied = false;
    Inventory inventory;
    // Start is called before the first frame update

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool Construct(int x, int y, string building, long seconds, Inventory.ResourceType[] types, int[] amount)
    {
        int StructID = getID(building);
        int ConstructID = -1;
        if (StructID >= 0)
        {
            SavedObject.Shape shape = structures[StructID].GetComponent<SavedObject>().shape;
            if (CheckValidOccupation(x, y, shape) == null)
            {
                if (building == "Tree")
                    ConstructID = getID("Sapling");
                else
                {
                    switch (shape)
                    {
                        case SavedObject.Shape.Single:
                            ConstructID = getID("construct1");
                            break;
                        case SavedObject.Shape.Plus:
                            ConstructID = getID("construct2");
                            break;
                        case SavedObject.Shape.Square:
                            ConstructID = getID("construct3");
                            break;
                    }
                }
            }

            int start = inventory.CheckResourceAmount(Inventory.ResourceType.Fellas);
            if (ConstructID >= 0 && inventory.ConsumeResource(types, amount))
            {
                int fellas_consumed = start - inventory.CheckResourceAmount(Inventory.ResourceType.Fellas);
                SavedObject obj = SpawnStructure(x, y, ConstructID, StructID, environment.grid[x, y].position, true);
                SpawnEvent(seconds, x, y);
                return true;
            }
        }
        return false;
    }

    public int getID(string name)
    {
        for (int i = 0; i < structures.Length; i++)
        {
            if (structures[i].GetComponent<SavedObject>().objName == name)
                return i;
        }
        return -1;
    }


    const long SECOND_VALUE = 10000000;
    public void SpawnEvent(long seconds, int x_pos, int y_pos)
    {
        long start = System.DateTime.UtcNow.ToBinary();
        long finish = start + seconds * SECOND_VALUE;
        SpawnEvent(start, finish, x_pos, y_pos);
    }

    public void SpawnEvent(long start, long finish, int x_pos, int y_pos)
    {
        Vector3 position = environment.grid[x_pos, y_pos].position;
        GameObject obj = Instantiate(eventPrefab, position, Quaternion.identity, transform);
        TimedEvent Tevent = obj.GetComponent<TimedEvent>();
        Tevent.SetFinishTime(start, finish, x_pos, y_pos);
    }

    public void CreateOccupationGrid(int size)
    {
        environment = FindObjectOfType<GridEnvironment>();
        Occupation = new bool[size, size];
    }

    public TimedEvent CheckEventOccupation(int x, int y)
    {
        TimedEvent[] events = GameObject.FindObjectsOfType<TimedEvent>();
        for(int i = 0; i < events.Length; i++)
        {
            if (events[i].x == x && events[i].y == y)
            {
                Debug.Log("Occupied");
                return events[i];
            }
        }
        return null;
    }

    public SavedObject FindOccupier(int x, int y)
    {
        occupied = true;
        SavedObject[] Objects = GameObject.FindObjectsOfType<SavedObject>();
        for (int i = 0; i < Objects.Length; i++)
        {
            switch (Objects[i].shape)
            {
                case SavedObject.Shape.Single:
                    if (Objects[i].x == x && Objects[i].y == y)
                        return Objects[i];
                    break;
                case SavedObject.Shape.Square:
                    for (int k = x - 1; k <= x + 1; k++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            if(Objects[i].x == k && Objects[i].y == j)
                                return Objects[i];
                        }
                    }
                    break;
            }
        }
        occupied = false;
        return null;
    }

    public SavedObject CheckValidOccupation(int x, int y, SavedObject.Shape shape)
    {
        occupied = false;

        switch(shape)
        {
            case SavedObject.Shape.Single:
                if (Occupation[x, y] == false)
                    return null;
                else
                    return FindOccupier(x, y);
            case SavedObject.Shape.Square:
                float height= environment.grid[x,y].GetHeight();
                
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for(int j = y - 1; j <= y + 1; j++)
                    {
                        if (i < 0 || j < 0 || i >= environment.size || j >= environment.size || environment.grid[i, j].GetHeight() != height)
                        {
                            occupied = true;
                            return null;
                        }
                        if (Occupation[i, j] == true)
                            return FindOccupier(i, j);
                    }
                }
                return null;
        }
        return null;
    }

    public void DestroyStructure(SavedObject obj)
    {
        switch (obj.shape)
        {
            case SavedObject.Shape.Single:
                Occupation[obj.x, obj.y] = false;
                break;
            case SavedObject.Shape.Square:

                for (int i = obj.x - 1; i <= obj.x + 1; i++)
                {
                    for (int j = obj.y - 1; j <= obj.y + 1; j++)
                    {
                        Occupation[i, j] = false;
                    }
                }
                break;
        }
        Destroy(obj.gameObject);
    }

    void FillOccupation(SavedObject data)
    {
        switch (data.shape)
        {
            case SavedObject.Shape.Single:
                Occupation[data.x, data.y] = true;
                break;
            case SavedObject.Shape.Square:

                for (int i = data.x - 1; i <= data.x + 1; i++)
                {
                    for (int j = data.y - 1; j <= data.y + 1; j++)
                    {
                        Occupation[i, j] = true;
                    }
                }
                break;
        }
    }

    public SavedObject SpawnStructure(int x, int y, string objName, int extra, Vector3 position, bool loading_saved = false)
    {
        int ID = getID(objName);
        if(ID >= 0)
            return SpawnStructure(x, y, ID, extra, position, loading_saved);
        else
            return null;
    }

    public SavedObject SpawnStructure(int x, int y, int ID, int extra, Vector3 position, bool loading_save = false)
    {
        SavedObject.Shape shape = structures[ID].GetComponent<SavedObject>().shape;
        if (!loading_save)
        {
            SavedObject Occupying = CheckValidOccupation(x, y, shape);
            if (occupied == false)
            {
                GameObject obj = Instantiate(structures[ID], position, Quaternion.identity, transform);
                SavedObject data = obj.GetComponent<SavedObject>();
                data.x = x;
                data.y = y;
                data.extra = extra;
                FillOccupation(data);
            }
            return Occupying;
        }
        else
        {
            GameObject obj = Instantiate(structures[ID], position, Quaternion.identity, transform);
            SavedObject data = obj.GetComponent<SavedObject>();
            data.x = x;
            data.y = y;
            data.extra = extra;
            FillOccupation(data);
            return null;
        }
    }

}
