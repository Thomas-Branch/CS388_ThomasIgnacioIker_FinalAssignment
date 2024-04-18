using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public enum ResourceType { Wood, Rock, Metal, Dirt, Sand, Fellas, Length };

    string[] UIText = { "Wood", "Rock", "Metal", "Dirt", "Sand", "Fellas" };

    public int[] inventory = new int[(int)ResourceType.Length];

    public TMPro.TextMeshProUGUI[] itemUI;

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
        updateInventory();
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
        updateInventory();
        return true;
    }

    void updateInventory()
    {
        for(int i = 0; i < itemUI.Length; i++)
        {
            string str = UIText[i] + ": " + inventory[i];
            Debug.Log(str);
            itemUI[i].SetText(str);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load inventory from the save file
        updateInventory();
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
