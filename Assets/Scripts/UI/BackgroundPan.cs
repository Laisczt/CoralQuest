using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPan : MonoBehaviour
{
    /*
        Esse script varia a posicao de um Rect Transform (transform de UI) lateralmente, sem alterar a posição do GameObject
        eh usado para fazer a tela de fundo do menu principal se mover 
    */
    public float range = 835;   // limite maximo de movimento para cada lado
    public float speed = 1;     // velocidade do pan 
    RectTransform _transform;

    // Start is called before the first frame update
    void Start()
    {

        _transform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        _transform.anchoredPosition += new Vector2(speed, 0) * Time.deltaTime;  // move o transform

        if(_transform.anchoredPosition.x >= range)   // ao chegar no limite aa direita...
        {
            _transform.anchoredPosition = new Vector2(range, 0); // limita a posicao
            speed *= -1;    // troca de direção
        }
        else if (_transform.anchoredPosition.x <= -range)   // '' esquerda
        {
            _transform.anchoredPosition = new Vector2(-range, 0);
            speed *= -1;
        }

    }
}
