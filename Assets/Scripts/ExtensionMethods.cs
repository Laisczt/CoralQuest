using System.Collections;
using UnityEngine;

public static class ExtensionMethods 
{
    public static Vector2 Rotate(this Vector2 vec, float delta)
    {
        return new Vector2(
            vec.x * Mathf.Cos(delta) - vec.y * Mathf.Sin(delta),
            vec.x * Mathf.Sin(delta) + vec.y * Mathf.Cos(delta)
        );
    }

    public static bool Sees(this Transform caller, Transform target, float distance, LayerMask mask)
    {
        if (Vector2.Distance(caller.position, target.position) > distance) return false;
        
        var hit = Physics2D.Raycast(caller.position, (target.position - caller.position), distance, mask);
        //Debug.DrawRay(caller.position, target.position - caller.position);

        if (hit.transform != null && hit.transform == target)
        {
            if (target.gameObject.CompareTag("Player"))
            {
                return target.GetComponent<PlayerControl>().alive;
            }
            return true;
        }
        return false;
    }
}
