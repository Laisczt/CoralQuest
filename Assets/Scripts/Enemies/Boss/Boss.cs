using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] List<GameObject> arenaGates;
    [SerializeField] Animator m_Animator;
    [SerializeField] BossHealthTentacle HealthTentaclePF;
    [SerializeField] List<SpriteRenderer> Bubbles;
    [SerializeField] GameObject Rock;
    private PlayerControl target;

    public int AttackCooldownP1;
    public int AttackCooldownP2;
    private int rAttackCooldown;
    public int SummonTentacleAttemptCooldown;
    private int rSummonTentacleAttemptCooldown;
    public float TentacleChance;
    

    public float ArenaWidth = 26;

    public bool phase2;
    private bool active = false;
    private bool attacking = false;
    private int lastAttack;

    public int MaxHealth = 500;
    public int Health;

    public Transform PetrifiedPlantsParent;
    public SlideTentacle ArmL;
    public SlideTentacle ArmR;
    private Vector3 homePos;
    public BossHealthBar m_HealthBar;



    LayerMask playerMask;

    void Start()
    {
        Health = MaxHealth;
        homePos = transform.position;
        playerMask = LayerMask.GetMask("Player");
        rAttackCooldown = AttackCooldownP1;
        rSummonTentacleAttemptCooldown = SummonTentacleAttemptCooldown;
    }

    private void OnEnable()
    {
        ArmL.gameObject.SetActive(true);
        ArmR.gameObject.SetActive(true);
        m_HealthBar.gameObject.SetActive(true);
        target = PlayerControl.Instance;

        foreach(var i in arenaGates)
        {
            i.SetActive(true);
        }

        StartCoroutine(decayStage());
        
    }

    float x;
    private int timeSinceLastTentacle;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active || attacking) return;

        if (phase2) m_Animator.SetBool("Phase2", true);

        if(rAttackCooldown <= 0)
        {
            attacking = true;
            AttackRandom();
        }

        if(rSummonTentacleAttemptCooldown <= 0)
        {
            rSummonTentacleAttemptCooldown = SummonTentacleAttemptCooldown;
            if(Random.Range(0f, 1f) <= TentacleChance || timeSinceLastTentacle >= 4) 
            {
                summonTentacle();
                timeSinceLastTentacle = 0;
            }
            else{
                timeSinceLastTentacle++;
            }
        }

        transform.position = homePos + new Vector3(Mathf.Cos(x)/4, 0);
        x += Time.fixedDeltaTime;

        rSummonTentacleAttemptCooldown--;
        if(rAttackCooldown > 0) rAttackCooldown--;
    }

    private int turnsSinceLastPetrify;
    public void AttackRandom()
    {
        var maxRange = (!phase2) ? 4 : 5;

        int rand;
        do
        {
            rand = Random.Range(1, maxRange);
        } while (rand == lastAttack || (rand == 4 && turnsSinceLastPetrify < 2));

        if(rand == 4)
        {
            turnsSinceLastPetrify = 0;
        }
        else
        {
            turnsSinceLastPetrify++;
        }

        switch (rand){
            case 1:
                // Slice

                StartCoroutine(slice());
                resetCooldown();
                break;
            case 2:
                // Dig

                StartCoroutine(dig());
                resetCooldown();
                break;
            case 3:

                // Lob Rock
             
                StartCoroutine(tossRock());
                resetCooldown(-20);
                break;
            case 4:
                // Petrify

                StartCoroutine(petrify());
                // Petrify não reseta o cooldown
                break;
        }

        lastAttack = rand;
    }

    private void resetCooldown(int offset)
    {
        rAttackCooldown = (!phase2) ? AttackCooldownP1 : AttackCooldownP2 + offset;
    }
    private void resetCooldown()
    {
        resetCooldown(0);
    }
    private void idle()
    {
        m_Animator.SetTrigger("Idle"); 
        StartCoroutine(checkAttack());
    }
    private IEnumerator checkAttack()
    {
        while(attacking == true)
        {
            var state = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(state.IsName("Base.Idle") || state.IsName("Base.Idle2")) attacking = false;
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator tossRock()
    {
        m_Animator.SetTrigger("TossRock");

        var i = 55;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        Instantiate(Rock, transform.position + new Vector3(0, 5, -0.25f), Quaternion.identity);

        i = 30;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        idle();

    }
    private IEnumerator petrify()
    {
        var i = 84;

        m_Animator.SetTrigger("Petrify");

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        target.GetComponent<PlayerControl>().Petrify();

        i = 50;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        idle();
    }
    private void summonTentacle()
    {
        var t = Random.Range(0f, 1f);

        var posX = Mathf.Lerp(0, ArenaWidth, t) - ArenaWidth/2;

        var newTentacle = Instantiate(HealthTentaclePF, transform.position + new Vector3(posX, -5.7375f, -0.5f), Quaternion.identity);


        newTentacle.parentBoss = this;
        newTentacle.BubbleOriginY = transform.position.y - 3.63f;
        //newTentacle.GetComponent<BasicEnemy>().Target = target.transform;
    }

    public float sliceSize = 4f;
    private IEnumerator slice()
    {
        m_Animator.SetTrigger("Slice");

        var strikeposX = target.transform.position.x;
        m_Animator.SetBool("LeftSide", isPosOnLeft(strikeposX));
        m_Animator.SetBool("TargetFar", Mathf.Abs(strikeposX - transform.position.x) > 5.5f);

        var topleft = new Vector2(strikeposX - sliceSize, transform.position.y - 1.5f);
        var bottomright = new Vector2(strikeposX + sliceSize, transform.position.y - 3.6875f);

        Vector2 topright2 = Vector2.zero;
        Vector2 bottomleft2 = Vector2.zero;
        if (phase2)
        {
            topright2 = new Vector2(2 * transform.position.x - topleft.x, topleft.y);
            bottomleft2 = new Vector2(2 * transform.position.x - bottomright.x, bottomright.y);
            StartCoroutine(SpawnBubbles(bottomleft2.x, topright2.x, transform.position.y - 3.6875f));
        }


        StartCoroutine(SpawnBubbles(topleft.x, bottomright.x, transform.position.y - 3.6875f));

        var dur = 70;
        while (dur > 0)
        {
            dur--;
            yield return new WaitForFixedUpdate();
        }

        var hit = Physics2D.OverlapArea(topleft, bottomright, playerMask);
        if(hit != null)
        {
            if (target.Damage(1))
            {
                var direction = (target.transform.position - transform.position).normalized.x;
                target.Knockback(20, direction);
            }
        }
        if (phase2)
        {
            hit = Physics2D.OverlapArea(bottomleft2, topright2, playerMask);
            if (hit != null)
            {
                if (target.Damage(1))
                {
                    var direction = (target.transform.position - transform.position).normalized.x;
                    target.Knockback(20, direction);
                }
            }
        }

        dur = 8;
        while(dur > 0)
        {
            dur--;
            yield return new WaitForFixedUpdate();
        }


        attacking = false;
        idle();
    }

    public int bubbleCount = 15;
    private IEnumerator SpawnBubbles(float leftX, float rightX, float height)
    {
        if(leftX - sliceSize < homePos.x - ArenaWidth / 2)
        {
            leftX = homePos.x - ArenaWidth / 2;
        }
        if(rightX + sliceSize > homePos.x + ArenaWidth / 2)
        {
            rightX = homePos.x + ArenaWidth / 2;
        }

        List<GameObject> _bubbles = new List<GameObject>();


        // Leftmost and rightmost bubbles
        var newbub = Instantiate(Bubbles[Random.Range(0, Bubbles.Count)], new Vector3(leftX, height), Quaternion.identity);
        if (Random.Range(0f, 1f) < 0.5f) newbub.flipX = true;
        _bubbles.Add(newbub.gameObject);
        newbub = Instantiate(Bubbles[Random.Range(0, Bubbles.Count)], new Vector3(rightX, height), Quaternion.identity);
        if (Random.Range(0f, 1f) < 0.5f) newbub.flipX = true;
        _bubbles.Add(newbub.gameObject);

        // Random bubbles in the middle
        for (var i = bubbleCount - 2; i > 0; i--)
        {

            newbub = Instantiate(
                                        Bubbles[Random.Range(0, Bubbles.Count)],
                                        new Vector3(Random.Range(leftX, rightX), height),
                                        Quaternion.identity
                                    );
            if (Random.Range(0f, 1f) < 0.5f) newbub.flipX = true;

            _bubbles.Add(newbub.gameObject);
            
        }

        var j = 72;
        while(j > 0)
        {
            j--;
            yield return new WaitForFixedUpdate();
        }
        foreach(var element in _bubbles)
        {
            Destroy(element);
        }
    }

    private IEnumerator dig()
    {
        m_Animator.SetTrigger("Dig");
        m_Animator.SetBool("LeftSide", isPosOnLeft(target.transform.position.x));
        var arm = ((isPosOnLeft(target.transform.position.x)) ? ArmR : ArmL);
        var i = 55;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        
        arm.Slide();

        // Ataque deve ser finalizado pelo tentáculo por meio de StopDigging()
    }

    private IEnumerator decayStage()
    {
        var decayDuration = 112;
        var extraDuration = 56;
        var stages = PetrifiedPlantsParent.childCount;
        var step = decayDuration / stages;
        var i = 0;
        foreach (Transform child in PetrifiedPlantsParent)
        {
            child.gameObject.SetActive(false);

            while(i < step)
            {
                i++;
                yield return new WaitForFixedUpdate();
            }
            i = 0;
            decayDuration -= step;
        }

        while (decayDuration > 0)
        {
            decayDuration--;
            yield return new WaitForFixedUpdate();
        }

        while (extraDuration > 0)
        {
            extraDuration--;
            yield return new WaitForFixedUpdate();
        }

        active = true;
    }

    public void Damage(int amount)
    {
        Health -= amount;
        if(Health <= MaxHealth/2 && !phase2)
        { 
            phase2 = true;
            sliceSize -= 0.5f;
            // todo
        }
        if(Health <= 0)
        {
            Health = 0;
            active = false;
            StartCoroutine(deathStall());
            var tents = FindObjectsOfType<BossHealthTentacle>();
            foreach(var i in tents)
            {
                i.Kill();
            }
            ArmL.OutAndDestroy();
            ArmR.OutAndDestroy();

            foreach( var i in arenaGates)
            {
                var anim = i.GetComponent<Animator>();
                if(anim != null) anim.SetTrigger("Death");
            }

            m_Animator.SetTrigger("Defeat");
            StopAllCoroutines();
            StartCoroutine(deathStall());
        }

        m_HealthBar.ChangeHealth((float)Health / MaxHealth);
    }

    private IEnumerator deathStall()
    {
        var i = 150;

        while(i > 110)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        foreach(var j in arenaGates)
        {
            Destroy(j);
        }

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        Destroy(m_HealthBar.gameObject);

        Destroy(gameObject);
    }
    private bool isPosOnLeft(float xPos)
    {
        return xPos < transform.position.x;
    }

    public void StopDigging()
    {
        idle();
    }
}
