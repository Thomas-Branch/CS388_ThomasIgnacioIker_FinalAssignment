using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimedEvent : MonoBehaviour
{
    public long start_time = 0;
    public long finish_time = 0;
    float timer = 0;
    public int x = 0;
    public int y = 0;

    public TMPro.TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        WorldSpawner spawner = FindObjectOfType<WorldSpawner>();
        SavedObject obj = spawner.FindOccupier(x, y);
        if (obj)
            obj.event_happening = this;

    }
    const long SECOND_VALUE = 10000000;
    // Update is called once per frame

    long UpdateTime()
    {
        long value = (finish_time - System.DateTime.UtcNow.ToBinary()) / SECOND_VALUE + 1;
        long seconds = value % 60;
        long minutes = (value / 60) % 60;
        long hours = (value / 60) / 60;
        string stringText;
        if (hours > 0)
            stringText = "" + hours + " h " + minutes + " m " + seconds + " s";
        else if (minutes > 0)
            stringText = "" + minutes + " m " + seconds + " s";
        else
            stringText = "" + seconds + " s";
        text.text = stringText;
        return value;
    }

    void CheckFinished(long value)
    {
        if (value < 0)
        {
            WorldSpawner spawner = FindObjectOfType<WorldSpawner>();
            SavedObject obj = spawner.FindOccupier(x, y);
            if (obj)
            {
                obj.event_happened = true;
                obj.event_happening = null;
            }
            else
            {
                GridEnvironment environment = FindObjectOfType<GridEnvironment>();
                Inventory inv = FindObjectOfType<Inventory>();
                switch ((Cell.Type)environment.grid[x,y].tile_type)
                {
                    case Cell.Type.Grass:
                        inv.AddResource(Inventory.ResourceType.Dirt, Random.Range(3, 6));
                        break;
                    case Cell.Type.Sand:
                        inv.AddResource(Inventory.ResourceType.Sand, Random.Range(3, 6));
                        break;
                }
                inv.AddResource(Inventory.ResourceType.Fellas, 2);
            }
            Destroy(gameObject);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1.0f)
        {
            CheckFinished(UpdateTime());
            timer -= 1.0f;
        }

    }

    public void SetFinishTimeFromNow(long seconds, int x_pos, int y_pos)
    {
        long start = System.DateTime.UtcNow.ToBinary();
        long finish = start + seconds * SECOND_VALUE;
        SetFinishTime(start, finish, x_pos, y_pos);
    }

    public void SetFinishTime(long start, long finish, int x_pos, int y_pos)
    {
        x = x_pos;
        y = y_pos;
        start_time = start;
        finish_time = finish;
        CheckFinished(UpdateTime());
    }

    public long GetFinishTime()
    {
        return finish_time;
    }

    public long GetTimePercentLeft()
    {
        long finish = (finish_time - start_time) / SECOND_VALUE;
        long current = (System.DateTime.UtcNow.ToBinary() - start_time) / SECOND_VALUE;

        return current * ((long)100)/finish;

    }
}
