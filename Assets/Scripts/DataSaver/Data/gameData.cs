using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class gameData
{
    /*
        Dados a serem salvos em arquivo
    */
    public Vector3 Level1SavePos, Level2SavePos;
    public bool ReachedLevel2, ShowTimer;
    public List<string> KilledTentacles;
    public string bestTimeLevel1, bestTimeLevel2; // usando string pois TimeSpan não pode ser serializado

    public gameData()
    {
        Level1SavePos = new Vector3(0,0, -100);
        KilledTentacles = new List<string>();
        Level2SavePos = new Vector3(0,0, -100);
        bestTimeLevel1 = new TimeSpan().ToString("mm':'ss'.'fff");
        bestTimeLevel2 = new TimeSpan().ToString("mm':'ss'.'fff");
    }
}
