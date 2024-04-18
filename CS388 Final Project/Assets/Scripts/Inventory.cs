using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public enum ResourceType { Wood, Rock, Metal, Dirt, Sand, Seed, Length };

    string[] UIText = { "Wood", "Rock", "Metal", "Dirt", "Sand", "Seed" };

    int[] inventory = new int[(int)ResourceType.Length];


    public GameObject[] itemUI;

    public void LoadInventory()
    {
        // Load inventory from save file
    }

    public void SaveInventory()
    {
        // Save inventory to save file
    }

    public void AddResource(ResourceType type, int amount)
    {
        inventory[(int)type] += amount;
    }

    public int CheckResourceAmount(ResourceType type)
    {
        return inventory[(int)type];
    }

    public bool ConsumeResource(ResourceType[] types, int[] amount)
    {
        // Sanity check
        if (types.Length != amount.Length)
        {
            Debug.Log("ERROR: ResourceType / Amount mismatch!");
            return false;
        }

        // If not enough to pay, return false
        // and does not actually consume the resources
        for(int i = 0; i < types.Length; i++)
        {
            if(inventory[(int)types[i]] < amount[i])
            {
                return false;
            }
        }

        // There is enough to pay

        // Reduce the necessary amount of stock
        for (int i = 0; i < types.Length; i++)
        {
            inventory[(int)types[i]] -= amount[i];
        }

        // Success
        return true;
    }

    void updateInventory()
    {
        for(int i = 0; i < (int)ResourceType.Length; i++)
        {
            itemUI[i].GetComponent<TextMeshPro>().SetText(UIText[i] + ": " + inventory[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load inventory from the save file
        LoadInventory();
    }

    // Update is called once per frame
    void Update()
    {
        updateInventory();
    }
}
