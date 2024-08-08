using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    /*
        Script mestre da Boss
        Lida com funcionalidades de movimento, ataque, e vida (exc. barra de vida)
    */
    [SerializeField] List<GameObject> arenaGates;   // Portoes da arena (que trancam quando a boss spawna)

    [SerializeField] AudioSource Scream;            // Audio de grito
    public AudioSource digSound;
    [SerializeField] AudioSource SliceSound;        // Audio do slice
    [SerializeField] AudioSource PetrifyChargeSound;// Audio do inicio do petrify
    [SerializeField] Animator m_Animator;           // Animator
    [SerializeField] BossHealthTentacle HealthTentaclePF; // Prefab tentaculo de vida
    [SerializeField] GameObject Rock;               // Prefab da rocha do ataque tossrock
    private PlayerControl target;                   // Player

    public int AttackCooldownP1;    // Cooldown entre ataques na primeira fase
    public int AttackCooldownP2;    // Cooldown entre ataques na segunda fase
    private int rAttackCooldown;    // Cooldown faltante
    private int turnsSinceLastPetrify;  // Quantidade de ataques desde a última vez que o petrify foi usado
    private int turnsSinceLastSlice;

    public int SummonTentacleAttemptCooldown;   // Cooldown das tentativas de spawnar um tentaculo de vida
    private int rSummonTentacleAttemptCooldown; // Cooldown faltante
    public float TentacleChance;    // Chance de spawnar um tentaculo de vida por tentativa
    private int turnsSinceLastHealthTentacle;   // Numero de tentativas feitas desde que foi spawnado um tentaculo de vida
    

    public float ArenaWidth;        // Tamanho total no eixo x da arena

    public bool phase2;             // Se a boss está na fase 2
    private bool active;            // Se a boss esta ativa (falso durante a animacao de spawn e ao ser derrotada)
    private bool attacking;         // Se esta atacando 
    private int lastAttack;         // Ultimo ataque realizado (para evitar repetição)
    public float sliceSize = 4f;    // Tamanho do ataque slice
    public int MaxHealth = 500;     // Vida maxima
    public int Health;              // Vida atual

    public Transform PetrifiedPlantsParent; // Plantas que sao destruidas durante a sequencia de spawn
    public SlideTentacle ArmL;      // Braco esquerdo do slide
    public SlideTentacle ArmR;      // ''    direito ''
    private Vector3 homePos;        // Posicao inicial
    public BossHealthBar m_HealthBar;   // Barra de vida



    LayerMask playerMask;

    void Start()
    {
        Health = MaxHealth;
        homePos = transform.position;
        rAttackCooldown = AttackCooldownP1;
        rSummonTentacleAttemptCooldown = SummonTentacleAttemptCooldown;

        playerMask = LayerMask.GetMask("Player");

        Scream.Play();
        ArmL.gameObject.SetActive(true);
        ArmR.gameObject.SetActive(true);
        m_HealthBar.gameObject.SetActive(true);
        target = PlayerControl.Instance;

        foreach(var i in arenaGates)    // Ativa os portões
        {
            i.SetActive(true);
        }

        StartCoroutine(decayStage());   // 
    }
    float x;
    void FixedUpdate()
    {
        if (!active) return;

        if(rSummonTentacleAttemptCooldown <= 0) // Tentativa de sumonar um tentaculo de vida
        {
            rSummonTentacleAttemptCooldown = SummonTentacleAttemptCooldown;
            if((Random.Range(0f, 1f) <= TentacleChance) || turnsSinceLastHealthTentacle >= 15) 
            {
                summonTentacle();
                turnsSinceLastHealthTentacle = 0;
            }
            else{
                turnsSinceLastHealthTentacle++;
            }
        }

        rSummonTentacleAttemptCooldown--;
        
        if(attacking) return;

        if(rAttackCooldown <= 0)    // Ataque
        {
            attacking = true;
            AttackRandom();
        }

        

        // Varia levemente a posicao em x da boss
        transform.position = homePos + new Vector3(Mathf.Cos(x)/4, 0);
        x += Time.fixedDeltaTime;

        if(rAttackCooldown > 0) rAttackCooldown--;
    }




    
    public void AttackRandom()
    {
        var maxRange = (!phase2) ? 4 : 5;   // Numero de ataques a depender da fase (fase 2 comeca a usar petrify)

        int rand;
        if(turnsSinceLastSlice >= 3) rand = 1;
        else
        {
            do  // Seleciona um ataque aleatorio diferente do ultimo usado (so podendo usar petrify depois de 2 ataques diferentes)
            {
                rand = Random.Range(1, maxRange);
            } while (rand == lastAttack || (rand == 4 && turnsSinceLastPetrify <= 2));   
        }
        

        if(rand == 4)
        {
            turnsSinceLastPetrify = 0;
        }
        else
        {
            turnsSinceLastPetrify++;
        }
        
        if (rand == 1)
        {
            turnsSinceLastSlice = 0;
        }
        else
        {
            turnsSinceLastSlice++;
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

                // Toss Rock
             
                StartCoroutine(tossRock());
                resetCooldown(-20);
                break;
            case 4:
                // Petrify

                StartCoroutine(petrify());
                resetCooldown(-100);    // Petrify permite que outro ataque seja usado imediatamente
                break;
        }

        lastAttack = rand;
    }

    private bool isPosOnLeft(float xPos)        // Retorna se a posicao passada esta a esquerda da boss
    {
        return xPos < transform.position.x;
    }

    private void resetCooldown(int offset)  // Reseta o cooldown do ataque 
    {
        rAttackCooldown = (!phase2) ? AttackCooldownP1 : AttackCooldownP2 + offset;
        if(rAttackCooldown < 0) rAttackCooldown = 0;
    }
    private void resetCooldown()
    {
        resetCooldown(0);
    }
    private void idle() // Retorna ao estado padrao
    {
        m_Animator.SetTrigger("Idle"); 
        StartCoroutine(checkAttack());
    }
    private IEnumerator checkAttack()   // Espera a boss retornar a um estado de idle no animador para finalizar o ataque
    {
        while(attacking == true)
        {
            var state = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(state.IsName("Base.Idle") || state.IsName("Base.Idle2")) attacking = false;
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator tossRock()  // Lanca uma pedra no teto que se despedaca
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
    private IEnumerator petrify()   // Petrifica a player
    {
        m_Animator.SetTrigger("Petrify");

        PetrifyChargeSound.PlayOneShot(PetrifyChargeSound.clip);

        var i = 84;

        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        target.GetComponent<PlayerControl>().Petrify();

        i = 20;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        idle();
    }
    private void summonTentacle()   // Sumona um tentaculo de vida
    {
        var t = Random.Range(0f, 1f);

        var posX = Mathf.Lerp(0, ArenaWidth, t) - ArenaWidth/2;

        var newTentacle = Instantiate(HealthTentaclePF, transform.position + new Vector3(posX, -5.7375f, -0.5f), Quaternion.identity);


        newTentacle.parentBoss = this;
        newTentacle.BubbleOriginY = transform.position.y - bossHeight + 0.5f;
    }

    

    float bossHeight = 4.6875f; // Altura da boss (da posicao do transform até o chao)

    private IEnumerator slice() // ataque Corte/soco/smack
    {
        m_Animator.SetTrigger("Slice");

        var strikeposX = target.transform.position.x;
        m_Animator.SetBool("LeftSide", isPosOnLeft(strikeposX));
        m_Animator.SetBool("TargetFar", Mathf.Abs(strikeposX - transform.position.x) > 5.5f);


        var topleft = new Vector2(strikeposX - sliceSize, transform.position.y - bossHeight + 2.5f);        // // Cantos que definem a hitbox do ataque 
        var bottomright = new Vector2(strikeposX + sliceSize, transform.position.y - bossHeight);

        Vector2 topright2 = Vector2.zero;   // Cantos que definem a hitbox do ataque espelhado da fase 2
        Vector2 bottomleft2 = Vector2.zero;

        if (phase2) // espelha o ataque na fase 2
        {
            topright2 = new Vector2(2 * transform.position.x - topleft.x, topleft.y);
            bottomleft2 = new Vector2(2 * transform.position.x - bottomright.x, bottomright.y);
            SpawnSliceBubbles(bottomleft2.x, topright2.x, transform.position.y - bossHeight + 0.5f);
        }


        SpawnSliceBubbles(topleft.x, bottomright.x, transform.position.y - bossHeight + 0.5f); // Bolhas que anunciam o ataque

        var dur = 40;
        while (dur > 0)
        {
            dur--;
            yield return new WaitForFixedUpdate();
        }

        SliceSound.PlayOneShot(SliceSound.clip);
        
        if(phase2)  // Toca o audio outra vez para o ataque espelhado da fase 2
        {
            yield return new WaitForFixedUpdate();
            SliceSound.PlayOneShot(SliceSound.clip);
            dur--;
        }


        dur += 25;

        while (dur > 0)
        {
            dur--;
            yield return new WaitForFixedUpdate();
        }

        // Checa se o jogador foi pego na area do ataque e da dano
        var hit = Physics2D.OverlapArea(topleft, bottomright, playerMask);
        if(hit != null)
        {
            if (target.Damage(1))       // Aplica knockback no jogador somente se foi possivel dar dano
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
                if (target.Damage(1))   // Aplica knockback no jogador somente se foi possivel dar dano
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

    public int bubbleCount; // Numero de bolhas por slice
    void SpawnSliceBubbles(float leftX, float rightX, float height)
    {
        // Limita a area das bolhas ao tamanho da arena
        if(leftX - sliceSize < homePos.x - ArenaWidth / 2)
        {
            leftX = homePos.x - ArenaWidth / 2;
        }
        if(rightX + sliceSize > homePos.x + ArenaWidth / 2)
        {
            rightX = homePos.x + ArenaWidth / 2;
        }

        // bolhas nos limites esquerdo e direito do ataque
        BubbleManager.Instance.SpawnBubble(new Vector3(leftX, height), BubbleType.Any); 
        BubbleManager.Instance.SpawnBubble(new Vector3(rightX, height), BubbleType.Any);

        // bolhas em posicoes aleatorias entre os limites
        for (var i = bubbleCount - 2; i > 0; i--)
        {
            BubbleManager.Instance.SpawnBubble(new Vector3(Random.Range(leftX, rightX), height), BubbleType.Any);
        }
    }

    private IEnumerator dig()   // Ataque dig (tentaculo que aparece do lado da arena e a percorre)
    {
        m_Animator.SetTrigger("Dig");
        m_Animator.SetBool("LeftSide", isPosOnLeft(target.transform.position.x));
        var arm = isPosOnLeft(target.transform.position.x) ? ArmL : ArmR;   // Escolhe o tentaculo a usar dependendo da posicao do jogador
        

        var i = 15;
        while(i > 0)
        {
            i--;
            yield return new WaitForFixedUpdate();
        }

        digSound.Play();

        i = 20;
        while(i > 0)    // roda a animacao de cavar antes do tentaculo aparecer
        {
            i--;
            yield return new WaitForFixedUpdate();
        }
        arm.Slide();    // ativa o tentaculo

        // Ataque deve ser finalizado pelo tentáculo por meio de StopDigging()
    }

    public void StopDigging()   // Finaliza o dig
    {
        idle();
        digSound.Stop();
    }

    private IEnumerator decayStage()
    {
        var decayDuration = 112;    // Duracao do decay
        var extraDuration = 56;     // Duracao extra antes de ativar a boss

        var stages = PetrifiedPlantsParent.childCount;  // numero de estagios de decay
        var step = decayDuration / stages;  // duracao de cada estagio
        
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

        while (decayDuration > 0)   // Esperar qualquer duracao restante
        {
            decayDuration--;
            yield return new WaitForFixedUpdate();
        }

        while (extraDuration > 0)   // Esperar a duracao extra
        {
            extraDuration--;
            yield return new WaitForFixedUpdate();
        }

        active = true;          // Ativa a boss
    }

    public void Damage(int amount)  // Danifica a boss
    {
        if(!active) return;
        Health -= amount;
        if(Health <= MaxHealth/2 && !phase2)    // Ativa a segunda fase ao alcancar metade da vida
        { 
            phase2 = true;
            m_Animator.SetBool("Phase2", true);
            sliceSize -= 0.5f;  // Reduz levemente o tamanho do slice (para compensar o fato que a fase 2 tem 2 slices simultaneos)
        }
        if(Health <= 0)
        {
            Health = 0;
            active = false;
            StartCoroutine(deathStall());

            var tents = FindObjectsOfType<BossHealthTentacle>();    // tentaculo(s) de vida ativos
            foreach(var i in tents)
            {
                i.Kill();
            }

            ArmL.OutAndDestroy(); // Remove os tentaculos do ataque slide
            ArmR.OutAndDestroy();

            foreach( var i in arenaGates) // "Mata" os portoes da arena (Eles sao destruidos em deathstall)
            {
                var anim = i.GetComponent<Animator>();
                if(anim != null) anim.SetTrigger("Death");
            }

            m_Animator.SetTrigger("Defeat");
            StopAllCoroutines();
            StartCoroutine(deathStall());
        }

        m_HealthBar.ChangeHealth((float)Health / MaxHealth);    // Atualiza a barra de vida
    }

    private IEnumerator deathStall()    // Destroi os objetos da boss apos finalizar as animacoes
    {
        var i = 150;

        while(i > 110) // espera somente 40 frames antes de destruir os portoes
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

        m_HealthBar.Kill();


        LevelMusicPlayer.Instance.Muffle(true, true);   // abafa a musica

        Destroy(gameObject);
    }
}
