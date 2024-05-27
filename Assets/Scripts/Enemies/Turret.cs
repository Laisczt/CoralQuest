using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicEnemy))]
public class Turret : MonoBehaviour, IEnemy
{
    public float Radius; // Distância de detecção
    public int Cooldown; // Cooldown entre tiros
    public int AttentionSpan;   // Quantidade de tempo entre o player sair do campo de visão do inimigo e o inimigo voltar pro pote
    private int rAttentionSpan;


    [SerializeField, HideInInspector] BasicEnemy basicEnemy;
    private Animator m_Animator;
    private PlayerControl target;
    
    [SerializeField] GameObject shot; // The projectile shot
    private int rCooldown = 0;  // keeps track of cooldown passing
    private Vector3 offset = new Vector3(0 , 0.48f, 0); // Offset for the position where the projectile will spawn
    private float shotSpawnDistance = 0.4f; // Offset in the direction the projectile is shot
    private Vector3 headPos;
    private bool attacking;

    LayerMask playerMask;
    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerMask = LayerMask.GetMask("Player", "Solid");
        m_Animator = GetComponent<Animator>();
        target = PlayerControl.Instance;
        headPos = transform.position + offset;
        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(!basicEnemy.Alive) return;
        var seesTarget = transform.Sees(offset, target.transform, Radius, playerMask);
        m_Animator.SetBool("Target In View", seesTarget);

        if (seesTarget)
        {
            rAttentionSpan = AttentionSpan;
            if (rCooldown > 0) rCooldown--;
            m_Animator.SetBool("Mirror", target.transform.position.x < transform.position.x);

            var state = getAnimatorState();

            if (!attacking && rCooldown == 0 && !state.IsName("Base.Spawn") && !state.IsName("Base.Spawn_Mirror"))
            {
                m_Animator.SetTrigger("Attack");

                StartCoroutine(Attack());
            }
        }
        else
        {
            if(rAttentionSpan > 0){
                rAttentionSpan--;
                if(rCooldown > Cooldown/3) rCooldown--;
            }
            else rCooldown = Cooldown;

            if(rAttentionSpan == 1) {
                m_Animator.SetTrigger("Despawn");
            }
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
        
        direction = target.transform.position + (Vector3)target.GetComponent<Collider2D>().offset - transform.position;
        direction.Normalize();

        var projec = Instantiate(shot, headPos + (Vector3)(direction * shotSpawnDistance), Quaternion.identity);
        projec.GetComponent<Projectile_Straight>().SetDirection(direction);
        projec.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction));

        rCooldown = Cooldown;
        attacking = false;
    }

    AnimatorStateInfo getAnimatorState()
    {
        return m_Animator.GetCurrentAnimatorStateInfo(m_Animator.GetLayerIndex("Base"));
    }

    public void Knockback()
    {
        //Empty
    }

    public void Damage(int _value)
    {
        m_Animator.SetTrigger("Damage");
        
        if(rCooldown < Cooldown * 2 / 3)
        {
            rCooldown = Cooldown * 2 / 3;
        }
    }

    public void Kill()
    {
        m_Animator.SetTrigger("Death");
        basicEnemy.DeathStall(72);
    }
}
