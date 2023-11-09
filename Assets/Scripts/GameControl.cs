using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public GameObject Player;
    public GameObject PlayerSpawn;
    public GameObject HealthBar;
    public GameObject UI;
    public GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        var player = Instantiate(Player, PlayerSpawn.transform.position, Quaternion.identity);
        player.name = "Player";

        var healthBar = Instantiate(HealthBar, UI.transform);
        healthBar.name = "Health Bar";
        healthBar.GetComponent<HealthBarControl>().AsignTarget(player);

        Camera.GetComponent<CameraMovement>().AsignTarget(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
