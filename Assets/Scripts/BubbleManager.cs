using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;

public class BubbleManager : MonoBehaviour
{
    /*
        O GameObject Bubble Manager deve estar presente em todos os níveis
        Ele é encarregado de criar bolhas pela tela, em lugares específicos
        Esses lugares são definidos por um tilemap especial, utilizando a pallete Bubble Spawners
        O tipo de bolha a ser gerado é definido pelo nome da tile

    */
    [SerializeField] List<GameObject> bubbles = new List<GameObject>();   // Prefabs das bolhas, deve estar em ordem: Cluster, Small, Medium, Large
    [SerializeField] Tilemap spawners;    // A Tilemap com os spawners
    public float BubbleChance = 0.025f;  // Chance que cada tile spawne uma bolha
    [SerializeField, HideInInspector] Transform target;   // O Player (setado pelo game controller)
    Vector3 lastPlayerPos;  // Última posição do jogador
    private Vector3Int size;    // A área ao redor do player onde bolhas podem ser geradas
    private int count;  // contador de fixedUpdates
    private TileBase[] tiles = new TileBase[735]; // Vetor que guarda as tiles

    
    // Start is called before the first frame update
    void Start()
    {
        size = new Vector3Int(35, 21, 0);
        target = PlayerControl.Instance.transform;
        if(target == null){
            Debug.LogError("Player not found - Bubble Manager");
        }
    }

    
    void FixedUpdate()
    {
        if(count % 12 == 0) // Bolhas são geradas a cada 12 frames
        {
            if(count % 5 == 0)  // O Vetor de tiles é atualizado a cada 60 frames (mmc(5, 12))
            {
                lastPlayerPos = (Vector2) target.position;
                spawners.GetTilesBlockNonAlloc(new BoundsInt(Vector3Int.FloorToInt(lastPlayerPos) - size/2, size + Vector3Int.forward), tiles); //Versão não alocante dessas função é usada para ajudar performance
            }

            for(int i = 0; i < tiles.Length; i++){
                if(tiles[i] == null) continue;
                
                if(Random.Range(0f,1f) <= BubbleChance)
                {
                    SpawnBubble(i, tiles[i].name[0]);
                }
            }
        }

        count++;
    }

    private void SpawnBubble(int pos, char type)    // Instancia uma bolha e agenda sua destruição
    {
        Vector3 worldpos = new Vector3(Mathf.Floor(lastPlayerPos.x - size.x/2) + (pos % size.x),
                                       Mathf.Floor(lastPlayerPos.y - size.y/2) + (pos / size.x),
                                       spawners.transform.position.z);
        worldpos += new Vector3(0.5f, 0.5f);

        int bubindex = -1;
        switch(type)    // Define o tipo de bolha a instanciar a partir do nome da tile
        {
            case 'C':
                bubindex = 0;
                break;
            case 'S':
                bubindex = 1;
                break;
            case 'M':
                bubindex = 2;
                break;
            case 'L':
                bubindex = 3;
                break;
            case 'P':   // P instancia cluster ou small
                bubindex = Random.Range(0, 2);
                break;
            case 'O':   // O instancia small, medium ou large
                bubindex = Random.Range(1, 4);
                break;
            case 'A':   // A instancia qualquer uma
                bubindex = Random.Range(0, 4);
                break;
        }
        if (bubindex == -1) {
            Debug.LogWarning("Couldn't resolve bubble type to generate in manager");
            return;
        }

        var _bubble = Instantiate(bubbles[bubindex], worldpos, Quaternion.identity);

        _bubble.GetComponent<SpriteRenderer>().flipX = Random.Range(0f,1f) <= 0.5f;

        StartCoroutine(BubbleDeathStall(_bubble));
    }

    IEnumerator BubbleDeathStall(GameObject bub)
    {
        var i = 72;
        while(i > 0){
            i--;
            yield return new WaitForFixedUpdate();
        }    
        Destroy(bub);
    }
}
