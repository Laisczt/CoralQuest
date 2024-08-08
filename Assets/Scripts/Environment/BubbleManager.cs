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

        Também pode ser chamado por outros scripts para criar bolhas por meio de SpawnBubble()

    */
    [SerializeField] List<GameObject> bubbles = new List<GameObject>();   // Prefabs das bolhas, deve estar em ordem: Cluster, Small, Medium, Large
    [SerializeField] Tilemap spawners;    // A Tilemap com os spawners
    public float BubbleChance = 0.025f;  // Chance que cada tile spawne uma bolha
    [SerializeField, HideInInspector] Transform target;   // O Player
    Vector3 lastPlayerPos;  // Última posição do jogador
    private Vector3Int size;    // A área ao redor do player onde bolhas podem ser geradas
    private int count;  // contador de fixedUpdates
    private TileBase[] tiles = new TileBase[735]; // Vetor que guarda as tiles

    public static BubbleManager Instance;

    // Awake é chamado ainda antes de Start
    void Awake()
    {
        Instance = this;
    }

    
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
                spawners.GetTilesBlockNonAlloc(new BoundsInt(Vector3Int.FloorToInt(lastPlayerPos) - size/2, size + Vector3Int.forward), tiles); // Versão não alocante dessa função é usada para ajudar performance
            }

            for(int i = 0; i < tiles.Length; i++){  // Percorre o tilemap procurando por tiles não vazias
                if(tiles[i] == null) continue;
                
                if(Random.Range(0f,1f) <= BubbleChance)
                {
                    SpawnBubbleFromTilemap(i, (BubbleType)tiles[i].name[0]); // Passa a posição do tilemap e o nome da tile 
                }
            }
        }

        count++;
    }

    private void SpawnBubbleFromTilemap(int index, BubbleType type)    // Calcula a posição da bolha no mundo e chama spawnbubble
    {
        Vector3 worldpos = new Vector3(Mathf.Floor(lastPlayerPos.x - size.x/2) + (index % size.x),
                                       Mathf.Floor(lastPlayerPos.y - size.y/2) + (index / size.x),
                                       spawners.transform.position.z);
        worldpos += new Vector3(0.5f, 0.5f);

        SpawnBubble(worldpos, type);
    }

    public void SpawnBubble(Vector3 position, BubbleType type, int lifespan = 72)    // Instancia uma bolha e agenda sua destruição
    {
        int bubindex = -1;

        if(type == BubbleType.None){
            return;
        }

        switch(type)    // Pega o índice da bolha a ser instanciada
        {
            case BubbleType.Cluster:
                bubindex = 0;
                break;
            case BubbleType.Small:   
                bubindex = 1;
                break;
            case BubbleType.Medium:   
                bubindex = 2;
                break;
            case BubbleType.Large:   
                bubindex = 3;
                break;
            case BubbleType.Small_And_Cluster:   
                bubindex = Random.Range(0, 2);
                break;
            case BubbleType.Single:   
                bubindex = Random.Range(1, 4);
                break;
            case BubbleType.Any:   
                bubindex = Random.Range(0, 4);
                break;
            case BubbleType.Red:  
                bubindex = Random.Range(4,6);
                break;
            case BubbleType.Red_Cluster:
                bubindex = 4;
                break;
            case BubbleType.Red_Medium:
                bubindex = 5;
                break;
            case BubbleType.Any_Inc_Red:
                bubindex = Random.Range(0,6);
                break;
        }

        if (bubindex == -1 || bubindex >= bubbles.Count) {
            Debug.LogWarning("Couldn't resolve bubble type to generate in manager");
            return;
        }


        var _bubble = Instantiate(bubbles[bubindex], position, Quaternion.identity);    // Cria a bolha

        _bubble.GetComponent<SpriteRenderer>().flipX = Random.Range(0f,1f) <= 0.5f;     // Espelha o sprite da bolha 50% das vezes

        StartCoroutine(BubbleDeathStall(_bubble, lifespan));  // Agenda a destruição da bolha
    }

    IEnumerator BubbleDeathStall(GameObject bubble, int lifespan) // Destroi a bolha depois da duração especificada
    {
        while(lifespan > 0){
            lifespan--;
            yield return new WaitForFixedUpdate();
        }    
        Destroy(bubble);
    }
}
