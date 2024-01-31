using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class Turret : MonoBehaviour
{
    public float radius; // Detection distance for the turret
    public int cooldown; // Cooldown between shots
    public bool BlocksPath;


    [SerializeField, HideInInspector] BasicEnemy basicEnemy;
    [SerializeField] GameObject shot; // The projectile shot
    private int curr_Cooldown = 0;  // keeps track of cooldown passing
    private float y_Offset = 0.48f; // Vertical offset for the position where the projectile will spawn
    private float shotSpawnDistance = 0.4f; // Offset in the direction the projectile is shot
    private Vector2 headPos;
    private Animator m_Animator;
    private bool attacking;

    LayerMask playerMask;
    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (BlocksPath) transform.GetChild(0).gameObject.SetActive(true);
        playerMask = LayerMask.GetMask("Player", "Solid");
        m_Animator = GetComponent<Animator>();
        headPos = transform.position + new Vector3(0, y_Offset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        var seesTarget = this.transform.Sees(basicEnemy.Target, radius, playerMask);
        m_Animator.SetBool("Target In View", seesTarget);

        if (seesTarget)
        {
            if (curr_Cooldown > 0) curr_Cooldown--;
            m_Animator.SetBool("Mirror", basicEnemy.Target.position.x < transform.position.x);

            var state = getAnimatorState();

            if (!attacking && curr_Cooldown == 0 && !state.IsName("Base.Spawn") && !state.IsName("Base.Spawn_Mirror"))
            {
                m_Animator.SetTrigger("Attack");

                StartCoroutine(Attack());
            }
        }
        else
        {
            curr_Cooldown = cooldown;
        }
    }

    IEnumerator Attack()
    {
        attacking = true;
        int i = 21;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        Vector2 direction;
        
        direction = (basicEnemy.Target.position + (Vector3)basicEnemy.Target.GetComponent<Collider2D>().offset) - transform.position;
        direction.Normalize();

        var projec = Instantiate(shot, headPos + (direction * shotSpawnDistance), Quaternion.identity);
        projec.GetComponent<Projectile_Straight>().direction = direction;
        projec.transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(direction, Vector2.right));

        curr_Cooldown = cooldown;
        attacking = false;
    }

    AnimatorStateInfo getAnimatorState()
    {
        return m_Animator.GetCurrentAnimatorStateInfo(m_Animator.GetLayerIndex("Base"));
    }
}
