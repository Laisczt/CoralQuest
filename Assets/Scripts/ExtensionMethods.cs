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

    static RaycastHit2D[] hits = new RaycastHit2D[1];
    public static bool Sees(this Transform caller, Transform target, float distance)
    {
        if (Vector2.Distance(caller.position, target.position) > distance) return false;
        
        var hitcount = Physics2D.RaycastNonAlloc(caller.position, (target.position - caller.position), hits, distance);
        //Debug.DrawRay(caller.position, target.position - caller.position);

        if (hitcount > 0 && hits[0].transform == target)
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
