using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GridEnvironment : MonoBehaviour
{
    public int size = 250;
    public float noise_scale = 0.01f;
    public int seed = 0;
    public Cell[,] grid;
    public GameObject rendererObj;

    public GameObject[] Environment_Spawns;

    int[] Spawn_Location = { 0, 0 };
    SaveFile data;
    WorldSpawner Spawner;

    float GetNoise(float x, float y, int seed_value, float scale)
    {
        float noise = Mathf.PerlinNoise(x * scale + seed_value, y * scale + seed_value);
        return noise;
    }

    void AddLine(List<Vector3> vert, List<Vector2> UVs, List<int> tri)
    {
        GameObject current = Instantiate(rendererObj);
        Mesh mesh = new Mesh();
        mesh.vertices = vert.ToArray();
        vert.Clear();
        mesh.triangles = tri.ToArray();
        tri.Clear();
        mesh.uv = UVs.ToArray();
        UVs.Clear();
        mesh.RecalculateNormals();
        current.GetComponent<MeshFilter>().mesh = mesh;
        current.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    int GetSurroundings(int x, int y)
    {
        int tblr = 0;
        if (grid[x, y].GetHeight() > grid[x, Mathf.Min(size - 1, y + 1)].GetHeight())
            tblr += 8;
        if (grid[x, y].GetHeight() > grid[x, Mathf.Max(0, y - 1)].GetHeight())
            tblr += 4;
        if (grid[x, y].GetHeight() > grid[Mathf.Max(0, x - 1), y].GetHeight())
            tblr += 2;
        if (grid[x, y].GetHeight() > grid[Mathf.Min(size - 1, x + 1), y].GetHeight())
            tblr += 1;
        return tblr;
    }

    void Update_Start(int x, int y)
    {
        if(grid[x,y].tile_type == (char)Cell.Type.Grass && grid[x, y].position.y < 2.0f)
        {
            int mid = size / 2;
            if(Mathf.Sqrt((x-mid)*(x-mid)+(y-mid)*(y-mid)) < Mathf.Sqrt((Spawn_Location[0] - mid) * (Spawn_Location[0] - mid) + (Spawn_Location[1] - mid) * (Spawn_Location[1] - mid)))
            {
                Spawn_Location[0] = x;
                Spawn_Location[1] = y;
            }

        }
    }

    void GenerateNewLand()
    {
        List<Vector3> vert = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        List<int> tri = new List<int>();
        int segment = (100 * 50) / size;

        if(seed == 0)
            seed = Random.Range(0, 1000000);

        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                grid[x, y] = new Cell(GetNoise(x, y, seed, noise_scale), x, y, size);
                Update_Start(x, y);
            }
        }

        for (int y = 0; y < size; y++)
        {
            if (y % segment == segment - 1)
                AddLine(vert, UVs, tri);

            for (int x = 0; x < size; x++)
                grid[x, y].CreateMesh(vert, UVs, tri, GetSurroundings(x, y));
        }
        AddLine(vert, UVs, tri);
        Instantiate(Environment_Spawns[0], grid[Spawn_Location[0], Spawn_Location[1]].position, Quaternion.identity, transform);
    }

    void GenerateNewDecorations()
    {
        if (PlayerPrefs.GetInt("Load Existing Seed") > 0)
        {
            for (int i = 0; i < data.world_data.Length; i = i + 4)
                Spawner.SpawnStructure(data.world_data[i], data.world_data[i + 1], data.world_data[i + 2], data.world_data[i + 3], grid[data.world_data[i], data.world_data[i + 1]].position, true);
            for (int i = 0; i < data.event_positions.Length; i = i + 2)
                Spawner.SpawnEvent(data.event_times[i], data.event_times[i + 1], data.event_positions[i], data.event_positions[i + 1]);
        }
        else
        {
            int secondary_seed = seed * 2 + 1000;
            float secondary_scale = noise_scale * 10.0f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float noise1 = GetNoise(x, y, seed, noise_scale);
                    float noise2 = GetNoise(x, y, secondary_seed, secondary_scale);
                    if (noise1 > 0.45f && noise1 + noise2 > 1.2f && x % 3 == 0 && y % 3 == 0)
                        Spawner.SpawnStructure(x, y, "Tree", 0, grid[x, y].position, true);
                }
            }
            Spawner.SpawnEvent(1000, 0, 0);
        }

    }

    void SetUpValues()
    {
        if(PlayerPrefs.GetInt("Load Existing Seed") > 0)
        {
            int saveFile = PlayerPrefs.GetInt("Save Existing Seed");
            string path = Application.persistentDataPath + "/map" + saveFile + ".island";
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as SaveFile;
            size = data.size;
            seed = data.seed;
            stream.Close();
        }
        else
        {
            if (PlayerPrefs.GetInt("Map_Scale") == 0)
                PlayerPrefs.SetInt("Map_Scale", 250);
            size = PlayerPrefs.GetInt("Map_Scale");
        }
        Spawner.CreateOccupationGrid(size);
    }


    // Start is called before the first frame update
    void Awake()
    {
        Spawner = FindObjectOfType<WorldSpawner>();
        SetUpValues();

        noise_scale = 2.5f / size;
        GenerateNewLand();
        GenerateNewDecorations();
        PlayerPrefs.SetInt("Load Existing Seed", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
