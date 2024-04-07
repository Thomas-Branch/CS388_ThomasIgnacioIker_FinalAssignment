using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public int seed;
    public char size;
    public char[] world_data;

    public SaveFile(GridEnvironment environment)
    {
        size = (char)environment.size;
        seed = environment.seed;

        SavedObject[] Objects = GameObject.FindObjectsOfType<SavedObject>();
        world_data = new char[4 * Objects.Length];
        for(int i = 0; i < Objects.Length; i++)
        {
            world_data[i * 4] = (char)Objects[i].x;
            world_data[i * 4 + 1] = (char)Objects[i].y;
            world_data[i * 4 + 2] = (char)Objects[i].ID;
            world_data[i * 4 + 3] = (char)Objects[i].extra;
        }
    }
}
