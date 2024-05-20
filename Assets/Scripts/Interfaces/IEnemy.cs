using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void Damage(int amount);
    void Knockback();
    void Kill();
}
