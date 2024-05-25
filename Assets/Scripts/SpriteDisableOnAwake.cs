using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpriteDisableOnAwake : MonoBehaviour
{
    // Desabilita o sprite do gameobject ao iniciar o jogo (útil para poder usar sprites no editor para objetos invisíveis em jogo)
    void Awake(){
        var renderer = GetComponent<SpriteRenderer>();
        if(renderer == null)
        {
            var trenderer = GetComponent<TilemapRenderer>();
            trenderer.enabled = false;
        }else
            renderer.enabled = false;
    }
}
