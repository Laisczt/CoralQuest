using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class gameData
{

    public int hp;

    public Vector3 position;

    public gameData()
    {
        position = Vector3.zero;
        hp = 5;
    }
}
