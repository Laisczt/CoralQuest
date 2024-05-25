using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    [SerializeField] Projectile_Straight Fragment;

    private void OnDestroy()
    {
        for(int i = 0; i < 12; i++)
        {
            var direction = Vector2.down;
            var angle = Random.Range(-60,60);
            direction = direction.Rotated(angle * Mathf.Deg2Rad);

            var proj = Instantiate(Fragment, transform.position +  (Vector3)direction, Quaternion.identity);
            proj.SetDirection(direction);
            proj.GetComponent<Rigidbody2D>().angularVelocity = angle * 15f;
        }
        
    }
}
