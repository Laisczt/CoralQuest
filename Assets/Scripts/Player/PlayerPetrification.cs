using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(PlayerControl))]
public class PlayerPetrification : MonoBehaviour
{
    [SerializeField, HideInInspector] PlayerControl m_PlayerControl;     // Controlador do player
    [SerializeField, HideInInspector] Animator m_Animator;
    public int shakeOffAmount;
    private int rShakeOffAmount;
    public float progressDecayRate = 1;
    private float progressDecayCounter;

    private int shaking;
    

    private void OnValidate()
    {
        m_PlayerControl = GetComponent<PlayerControl>();
        m_Animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        rShakeOffAmount = shakeOffAmount;
        shaking = 0;
        progressDecayCounter = 0;
        prevTouchCount = 0;
    }

    private int prevTouchCount;
    private void Update()
    {
        var touchcount = Input.touchCount;
        var touchProgress = touchcount > prevTouchCount;
        prevTouchCount = touchcount;
        if(Input.anyKeyDown) shaking = 1;
        if(touchProgress) shaking = 5;


        progressDecayCounter += Time.deltaTime;
    }

    private int shook; // 
    private void FixedUpdate()
    {
        if (rShakeOffAmount <= 0)
        {
            m_Animator.ResetTrigger("Shake");
            m_PlayerControl.Depetrify();
            this.enabled = false;
        }

        if (shaking > 0)
        {
            rShakeOffAmount--;
            shook++;
            if(shook >= 5)
            {
                shook = 0;
                m_Animator.SetTrigger("Shake");
            };
        }

        if (progressDecayCounter >= 1 / progressDecayRate)
        {
            progressDecayCounter -= 1 / progressDecayRate;
            if (rShakeOffAmount < shakeOffAmount) rShakeOffAmount++;
        }

        if(shaking > 0) shaking--;
    }

    public void Shake(){
        shaking = 1;
    }
}
