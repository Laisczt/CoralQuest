using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        playerMask = LayerMask.GetMask("Player", "Solid");
        homePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (basicEnemy.lockMovement || !basicEnemy.Alive) return;

        Vector2 travelDirection = Vector2.zero;
        if (Vector3.Distance(homePos, basicEnemy.Target.position) <= TerritoryRadius && this.transform.Sees(basicEnemy.Target, 8, playerMask))
        {
            travelDirection = basicEnemy.Target.transform.position - transform.position;
            speed += 0.05f;

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

        basicEnemy.m_RigidBody.velocity = travelDirection * speed;

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
        basicEnemy.m_Animator.SetBool("Moving", moving);
    }
    public void Knockback()
    {
        speed = baseSpeed + 1f;
    }

    public void Damage(int _value)
    {
        basicEnemy.m_Animator.SetTrigger("Damage");
    }

    public void Kill()
    {
        chaseSound.Stop();
        basicEnemy.m_RigidBody.velocity = Vector2.zero;
        basicEnemy.m_Animator.SetTrigger("Death");
        StartCoroutine(deathStall());
    }

    IEnumerator deathStall()
    {
        var i = 55;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
