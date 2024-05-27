using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(BasicEnemy))]
[RequireComponent(typeof(Rigidbody2D))]
public class Stalk : MonoBehaviour, IEnemy
{
    private Vector2 homePos;
    public float TerritoryRadius;
    public float TopSpeed = 4.5f;
    private float speed = 0;
    private float baseSpeed = 1f;
    [SerializeField, HideInInspector] BasicEnemy basicEnemy;
    private PlayerControl target;
    private Animator m_Animator;
    private Rigidbody2D m_RigidBody;

    [SerializeField] AudioSource chaseSound;
    private bool playingSound;

    private bool moving;



    private void OnValidate()
    {
        basicEnemy = GetComponent<BasicEnemy>();
    }

    LayerMask playerMask;
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerControl.Instance;
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        playerMask = LayerMask.GetMask("Player", "Solid");
        homePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (basicEnemy.LockMovement) return;
        if (!basicEnemy.Alive)
        {
            m_RigidBody.velocity /= 1.3f;
            return;
        }

        Vector2 travelDirection = Vector2.zero;
        if (Vector3.Distance(homePos, target.transform.position) <= TerritoryRadius && transform.Sees(target.transform, 8, playerMask))
        {
            travelDirection = target.transform.position - transform.position;
            speed += 0.1f;

            moving = true;
        }
        else if (Vector2.Distance(transform.position, homePos) > 0.3f)
        {
            travelDirection = homePos - (Vector2)transform.position;
            speed -= 0.0125f;
            moving = true;
        }
        else
        {
            speed = baseSpeed;
            moving = false;
        }

        travelDirection.Normalize();

        if (speed > TopSpeed) speed = TopSpeed;
        if (speed < baseSpeed) speed = baseSpeed;

        m_RigidBody.velocity = travelDirection * speed;

        // Audio
        if (moving == true && playingSound == false)
        {
            chaseSound.Play();
            playingSound = true;
        }
        if (moving == false && playingSound == true)
        {
            chaseSound.Stop();
            playingSound = false;
        }

        // Sprite
        m_Animator.SetBool("Moving", moving);
    }
    public void Knockback()
    {
        speed = baseSpeed + 1f;
    }

    public void Damage(int _value)
    {
        m_Animator.SetTrigger("Damage");
    }

    public void Kill()
    {
        chaseSound.Stop();
        m_Animator.SetTrigger("Death");
        basicEnemy.DeathStall(55);
    }

    
}
