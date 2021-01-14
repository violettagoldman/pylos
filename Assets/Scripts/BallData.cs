using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallData : MonoBehaviour
{

    public int x;
    public int y;
    public int stage;
    public int player;
    public bool moveable = false;
    public bool removeable = false;

    public BallData(int x, int y, int stage) {
        this.x = x;
        this.y = y;
        this.stage = stage;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
