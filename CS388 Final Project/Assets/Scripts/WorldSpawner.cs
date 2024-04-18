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
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
        for(int i = 0; i < structures.Length; i++)
        {
            if (structures[i].GetComponent<SavedObject>().objName == objName)
                return SpawnStructure(x, y, i, extra, position, loading_saved);
        }
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
            FillOccupation(data);
            return null;
        }
    }

}
