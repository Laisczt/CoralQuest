using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawn))]
public class RandomScript_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (EnemySpawn)target;

        var spawnContinProperty = serializedObject.FindProperty("SpawnContinuously");
        var maxEnemyProperty = serializedObject.FindProperty("MaxEnemyCount");
        var cooldownProperty = serializedObject.FindProperty("SpawnCooldown");

        EditorGUILayout.PropertyField(spawnContinProperty);

        if (script.SpawnContinuously)
        {
            EditorGUILayout.PropertyField(maxEnemyProperty);
            EditorGUILayout.PropertyField(cooldownProperty);

        }
        serializedObject.ApplyModifiedProperties();


        /*DrawDefaultInspector(); // for other non-HideInInspector fields

        var script = (EnemySpawn)target;

        script.SpawnContinuously = EditorGUILayout.Toggle("Spawn Continuously", script.SpawnContinuously);

        if (script.SpawnContinuously) // if bool is true, show other fields
        {
            script.MaxEnemyCount = EditorGUILayout.IntField("Max Enemy Count", script.MaxEnemyCount);
            script.SpawnCooldown = EditorGUILayout.IntField("Spawn Cooldown", script.SpawnCooldown);
        }
        */
    }

    
}

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] BasicEnemy monster;
    private BasicEnemy[] enemyPool;
    private int enemyCount = 0;
    [HideInInspector] public int poolAvailableCount = 0;

    private Transform player;


    [HideInInspector] public bool SpawnContinuously;

    [HideInInspector] public int MaxEnemyCount;

    [HideInInspector] public int SpawnCooldown;
    private int rSpawnCooldown;

    private void Awake()
    {
        enemyPool = new BasicEnemy[MaxEnemyCount];
        poolAvailableCount = MaxEnemyCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        rSpawnCooldown = SpawnCooldown;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (SpawnContinuously)
        {
            if (poolAvailableCount > 0)
            {
                if (rSpawnCooldown > 0) rSpawnCooldown--;
            }
            else rSpawnCooldown = SpawnCooldown;

            if (rSpawnCooldown == 0)
            {
                if (enemyCount < MaxEnemyCount)
                {
                    spawn();
                }
                else
                {
                    foreach(var element in enemyPool)
                    {
                        if (!element.Alive)
                        {
                            respawn(element);
                            break;
                        }
                    }
                }
                rSpawnCooldown = SpawnCooldown;
            }
        }
    }

    [ContextMenu("Spawn Enemy")]
    public void ForceSpawn()
    {
        var spawnedMonster = Instantiate(monster, transform.position, Quaternion.identity);
        spawnedMonster.Target = player;
    }
    private void spawn()
    {
        var spawnedMonster = Instantiate(monster, transform.position, Quaternion.identity);
        spawnedMonster.Target = player;
        spawnedMonster.ParentSpawner = this;
        spawnedMonster.isPooled = true;

        enemyPool[enemyCount] = spawnedMonster;
        enemyCount++;
        poolAvailableCount--;
    }

    private void respawn(BasicEnemy enemy)
    {
        enemy.Alive = true;
        enemy.Revived = true;
        enemy.Health = enemy.maxHealth;
        enemy.transform.position = this.transform.position;
        enemy.gameObject.SetActive(true);
        poolAvailableCount--;
    }

    public void Initialize(Transform player)
    {
        this.player = player;
        if (SpawnContinuously)
        {
            spawn();
        }
        else
        {
            ForceSpawn();
        }
        

    }


}


