using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    private float filled;
    private RectTransform filledBar;
    private float fullSize;
    private Animator c_Animator;
    private int damageFrames;

    public static BossHealthBar Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        filledBar = transform.GetChild(1).GetComponent<RectTransform>();
        c_Animator = filledBar.GetComponent<Animator>();
        fullSize = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        StartCoroutine(increaseHealthStart());
    }

    private void FixedUpdate()
    {
        if (damageFrames > 0) damageFrames--;
        c_Animator.SetInteger("DamageFrames", damageFrames);
    }

    IEnumerator increaseHealthStart()
    {
        filled = 0;
        var i = 150;
        float t = 0f;
        while(i > 0)
        {
            i--;
            t += 1f / 150f;
            filled = t;
            ChangeHealth(filled);
            yield return new WaitForFixedUpdate();
        }
        filled = 1;
        ChangeHealth(filled);
    }

    public void ChangeHealth(float newPercent)
    {
        if (newPercent < filled) damage();
        filled = newPercent;

        filledBar.sizeDelta = new Vector2(newPercent * fullSize, filledBar.sizeDelta.y);
    }

    private void damage()
    {
        damageFrames = 5;
    }

    private void exit()
    {
        Destroy(gameObject);
    }
}
