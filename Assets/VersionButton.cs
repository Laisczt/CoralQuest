using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionButton : MonoBehaviour
{
    private int clickCount = 0;
    public GameStart gameStart;

    public void click(){
        clickCount++;
        if(clickCount >= 5) gameStart.DebugUnlockLevelSelect();
    }
}
