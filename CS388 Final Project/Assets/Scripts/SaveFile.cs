using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public int seed;
    public char size;
    public char[] world_data;

    public char[] event_positions;
    public long[] event_times;

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

        TimedEvent[] events = GameObject.FindObjectsOfType<TimedEvent>();
        event_positions = new char[2 * events.Length];
        event_times = new long[2 * events.Length];
        for(int i = 0; i < events.Length; i++)
        {
            event_positions[i * 2] = (char)events[i].x;
            event_positions[i * 2 + 1] = (char)events[i].y;
            event_times[i * 2] = events[i].start_time;
            event_times[i * 2 + 1] = events[i].finish_time;
        }
    }
}
