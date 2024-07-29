using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BasicProjectile))]
public class Shatter : MonoBehaviour
{
    /*
        Projeteis que spawnam outros projeteis ao serem destruidos
    */
    [SerializeField] Projectile_Straight Fragment;  // O projetil secundario
    public int ProjectileCount = 16;

    private void OnDestroy()
    {
        ShatterProjectile();
    }

    public void ShatterProjectile()     // Instancia projeteis secundarios em um cone de 120º abaixo de si
    {
        for(int i = 0; i < ProjectileCount; i++)
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
