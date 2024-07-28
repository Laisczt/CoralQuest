using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    /*
        Interface para scripts de IA de inimigos
    */
    void Damage(int amount);
    void Knockback();
    void Kill();
}
