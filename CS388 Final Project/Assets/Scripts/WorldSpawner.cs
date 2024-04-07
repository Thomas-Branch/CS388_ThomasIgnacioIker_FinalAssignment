using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    public GameObject[] structures;
    public bool[,] Occupation;
    GridEnvironment environment = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateOccupationGrid(int size)
    {
        environment = FindObjectOfType<GridEnvironment>();
        Occupation = new bool[size, size];
    }

    SavedObject FindOccupier(int x, int y)
    {
        SavedObject[] Objects = GameObject.FindObjectsOfType<SavedObject>();
        for (int i = 0; i < Objects.Length; i++)
        {
            if (Objects[i].x == x && Objects[i].y == y)
                return Objects[i];
        }
        return null;
    }

    public SavedObject CheckValidOccupation(int x, int y, SavedObject.Shape shape)
    {
        switch(shape)
        {
            case SavedObject.Shape.Single:
                if (Occupation[x, y] == false)
                    return null;
                else
                    return FindOccupier(x, y);
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
            if (Occupying == null)
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
