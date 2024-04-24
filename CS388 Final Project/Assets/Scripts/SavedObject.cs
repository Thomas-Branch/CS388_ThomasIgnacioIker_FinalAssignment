using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedObject : MonoBehaviour
{
    public enum Shape { Single, Plus, Square };
    public Shape shape = Shape.Single;
    public string objName = "Structure";
    //KEEP DATA TO BETWEEN 0-255
    public int x = 0;
    public int y = 0;
    public int ID = 0;
    public int extra = 0;
    public bool event_happened = false;
    public TimedEvent event_happening = null;
    public int used_fellas = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
