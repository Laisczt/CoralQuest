using System.Collections;
using UnityEngine;

public static class ExtensionMethods 
{
    /*
        Metodos uteis a varios scripts
    */


    public static Vector2 Rotated(this Vector2 vec, float delta)        // Rotaciona um vector2 por um certo angulo
    {
        return new Vector2(
            vec.x * Mathf.Cos(delta) - vec.y * Mathf.Sin(delta),
            vec.x * Mathf.Sin(delta) + vec.y * Mathf.Cos(delta)
        );
    }

    public static bool Sees(this Transform caller, Transform target, float distance, LayerMask mask)    
    // Cria um raycast entre a posicao do transform que chamou o metodo e um alvo, e retorna true se não tiver uma parede entre os 2
    {
        if (Vector2.Distance(caller.position, target.position) > distance) return false;
        
        var hit = Physics2D.Raycast(caller.position, (target.position - caller.position), distance, mask);
        Debug.DrawRay(caller.position, target.position - caller.position);

        if (hit.transform != null && hit.transform == target)
        {
            if (target.gameObject.CompareTag("Player"))     // Se o alvo for o jogador, so retorna verdadeiro se o mesmo estiver vivo
            {
                return target.GetComponent<PlayerControl>().Alive;
            }
            return true;
        }
        return false;
    }
    public static bool Sees(this Transform caller, Vector3 offset, Transform target, float distance, LayerMask mask)
    // Mesmo que o anterior, mas com offset
    {
        if (Vector2.Distance(caller.position, target.position) > distance) return false;

        var hit = Physics2D.Raycast(caller.position + offset, (target.position - caller.position), distance, mask);
        Debug.DrawRay(caller.position + offset, target.position - caller.position);

        if (hit.transform != null && hit.transform == target)
        {
            if (target.gameObject.CompareTag("Player"))
            {
                return target.GetComponent<PlayerControl>().Alive;
            }
            return true;
        }
        return false;
    }

    // Alteração do método AudioSource.PlayClipAtPoint que toca o audio no modo 2d
    public static void PlayDetached(this AudioSource source)
    {
        GameObject gameObject = new GameObject("One shot audio");
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = source.clip;
        audioSource.spatialBlend = 0f;
        audioSource.volume = source.volume;
        audioSource.Play();
        Object.Destroy(gameObject, source.clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
}
