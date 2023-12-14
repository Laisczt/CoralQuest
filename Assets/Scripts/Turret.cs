using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float radius; // Detection distance for the turret
    public int cooldown; // Cooldown between shots
    public GameObject shot; // The projectile shot


    private GameObject target; // What the turret aims at
    private int curr_Cooldown = 0;  // keeps track of cooldown passing
    private float y_Offset = 0.48f; // Vertical offset for the position where the projectile will spawn
    private float shotSpawnDistance = 0.4f; // Offset in the direction the projectile is shot
    private Vector2 headPos;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player"); // Finds the target to be tracked
        headPos = transform.position + new Vector3(0, y_Offset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(curr_Cooldown > 0)curr_Cooldown--;

        Vector2 direction;

        if (curr_Cooldown == 0 && this.gameObject.Sees(target, radius))
        {
            direction = (target.transform.position + (Vector3) target.GetComponent<Collider2D>().offset) - transform.position;
            direction.Normalize();

            var projec = Instantiate(shot, headPos + (direction * shotSpawnDistance), Quaternion.identity);
            projec.GetComponent<Projectile_Straight>().direction = direction;

            curr_Cooldown = cooldown;
        }
        
    }
}
