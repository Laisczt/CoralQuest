using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]

public class timerData
{
    /*
        Guarda timers parciais para cada nível
    */
    public string elapsedTimeLevel1, elapsedTimeLevel2; // usando string pois TimeSpan não pode ser serializado

    public timerData()
    {
        elapsedTimeLevel1 = new TimeSpan().ToString("mm':'ss'.'fff");
        elapsedTimeLevel2 = new TimeSpan().ToString("mm':'ss'.'fff");
    }
}
