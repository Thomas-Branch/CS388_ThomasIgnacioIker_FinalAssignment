using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public enum Type {Water, Grass, Sand, Size};

    public Vector3 position;
    Vector3 scale;

    public char tile_type = (char)Type.Water;
    float UV_scale;

    public string GetName()
    {
        switch((Type)tile_type)
        {
            case Type.Water:
                return "Water";
            case Type.Grass:
                return "Grass";
            case Type.Sand:
                return "Sand";
        }
        return "Unknown";
    }

    public float GetHeight()
    {
        return position.y;
    }

    public Cell(float noiseValue, float x, float y, float size)
    {
        UV_scale = (1.0f / (float)Type.Size);
        position = new Vector3(x, 0, y);
        scale = new Vector3(1, 2, 1);


        if (noiseValue < 0.4f)
        {
            tile_type = (char)Type.Water;
            position.y = 0.0f;
        }
        else if (noiseValue > 0.45f)
        {
            tile_type = (char)Type.Grass;
            position.y = noiseValue > 0.65f ? (noiseValue > 0.8f ? 5.0f : 3.0f) : 1.0f;
        }
        else
        {
            tile_type = (char)Type.Sand;
            position.y = 0.5f;
        }
    }

    public void Add_Square(Vector3[] vertices, Vector2[] UVList, List<Vector3> vert, List<Vector2> UVs, List<int> tri)
    {
        int v = vert.Count;
        int[] triangles = new int[] { v, v + 1, v + 2, v + 1, v + 3, v + 2 };
        for (int i = 0; i < 4; i++)
        {
            vert.Add(position + vertices[i]);
            UVs.Add(UVList[i]);
        }
        for (int i = 0; i < 6; i++)
            tri.Add(triangles[i]);
    }

    public void CreateMesh(List<Vector3> vert, List<Vector2> UVs, List<int> tri, int tblr)
    {
        float start = UV_scale * ((float)tile_type) + 0.01f;
        float end = start + UV_scale - 0.02f;

        Add_Square(new Vector3[] { new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f) }, 
                      new Vector2[] { new Vector2(start, 1), new Vector2(end, 1), new Vector2(start, 0.5f), new Vector2(end, 0.5f) }, vert, UVs, tri);

        if((tblr & 4) != 0)
            Add_Square(new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(-0.5f, -2, -0.5f), new Vector3(0.5f, -2, -0.5f) },
                          new Vector2[] { new Vector2(start, 0.5f), new Vector2(end, 0.5f), new Vector2(start, 0.0f), new Vector2(end, 0.0f) }, vert, UVs, tri);

        if((tblr & 8) != 0)
            Add_Square(new Vector3[] { new Vector3(0.5f, 0, 0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, -2, 0.5f), new Vector3(-0.5f, -2, 0.5f) },
                          new Vector2[] { new Vector2(start, 0.5f), new Vector2(end, 0.5f), new Vector2(start, 0.0f), new Vector2(end, 0.0f) }, vert, UVs, tri);

        if ((tblr & 1) != 0)
            Add_Square(new Vector3[] { new Vector3(0.5f, 0, -0.5f), new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, -2, -0.5f), new Vector3(0.5f, -2, 0.5f) },
                          new Vector2[] { new Vector2(start, 0.5f), new Vector2(end, 0.5f), new Vector2(start, 0.0f), new Vector2(end, 0.0f) }, vert, UVs, tri);

        if ((tblr & 2) != 0)
            Add_Square(new Vector3[] { new Vector3(-0.5f, 0, 0.5f), new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, -2, 0.5f), new Vector3(-0.5f, -2, -0.5f) },
                          new Vector2[] { new Vector2(start, 0.5f), new Vector2(end, 0.5f), new Vector2(start, 0.0f), new Vector2(end, 0.0f) }, vert, UVs, tri);
    }

    public void Render()
    {
        switch((Type)tile_type)
        {
            case Type.Water:
                Gizmos.color = Color.blue;
                break;
            case Type.Grass:
                Gizmos.color = Color.green;
                break;
            case Type.Sand:
                Gizmos.color = Color.yellow;
                break;
        }
        Gizmos.DrawCube(position, scale);
    }

}
