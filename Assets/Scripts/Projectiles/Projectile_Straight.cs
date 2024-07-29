using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicProjectile))]
public class Projectile_Straight : MonoBehaviour
{
    /*
        Projeteis que voam em linha reta
    */
    public void SetDirection(Vector2 direction){    // Setar a direcao do projetil (feito pelo script que o spawnou)
        GetComponent<Rigidbody2D>().velocity = GetComponent<BasicProjectile>().speed * direction;
    }

}
