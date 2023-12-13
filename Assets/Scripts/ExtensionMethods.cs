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

    public static bool Sees(this GameObject caller, GameObject target, float distance)
    {
        if (Vector2.Distance(caller.transform.position, target.transform.position) > distance) return false;

        RaycastHit2D hit;
        hit = Physics2D.Raycast(caller.transform.position, (target.transform.position - caller.transform.position).normalized, distance);

        if (hit.collider != null && hit.transform.gameObject == target)
        {
            if (target.gameObject.name == "Player")
            {
                return target.GetComponent<PlayerControl>().alive;
            }
            return true;
        }
        return false;
    }
}
