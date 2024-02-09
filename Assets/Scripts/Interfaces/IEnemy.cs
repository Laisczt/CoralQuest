using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void Damage();
    void Knockback();
    void Kill();
}
