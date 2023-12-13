using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Game
{
    public static void updateScore()
    {
        var playerscore = GameObject.Find("Player").GetComponent<PlayerControl>().score ;
        var scoretext = GameObject.Find("Score").GetComponent<Text>();

        scoretext.text = playerscore.ToString();
    }
}
