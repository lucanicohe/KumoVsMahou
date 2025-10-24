using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FaceSpiderController : MonoBehaviour
{

    // other scripts and objects needed for this script 
    [SerializeField] private SpiderStats stats;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Scrollbar healthBar;




    private int clickCounter;
    private int clicksToDetach = 9;
    private bool isAttached = false;
    private Coroutine damageCoroutine;



    public void SetStats(SpiderStats spiderStats)
    {
        stats = spiderStats;
        UpdateClickRequirement();
    }



    void Start()
    {
        
        AttachToPlayer();
    }



    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0)) //if click spider dies
        {
            clickCounter++;

            if (clickCounter >= clicksToDetach)
                DetachFromPlayer();
        }

        UpdateHealthBar();
    }



    void UpdateClickRequirement()
    {
        if (stats == null) return;

        switch (stats.variant) // how many clicks for each spider it takes
        {
            case SpiderVariant.BrownStone: clicksToDetach = 9; break;
            case SpiderVariant.RedFire: clicksToDetach = 18; break;
            case SpiderVariant.YellowWind: clicksToDetach = 27; break;
            case SpiderVariant.CyanIce: clicksToDetach = 36; break;
            case SpiderVariant.BlueWater: clicksToDetach = 45; break;
            case SpiderVariant.WhiteBoss: clicksToDetach = 54; break;
            default: clicksToDetach = 9; break;
        }
    }


    void AttachToPlayer()
    {
        if (isAttached || stats == null || playerStats == null) return;

        isAttached = true;

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamagePlayerOverTime());
    }



    void DetachFromPlayer() //facespider gets set active false, click counter returns to zero 
    {
        gameObject.SetActive(false);
        clickCounter = 0;
        isAttached = false;

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }



    IEnumerator DamagePlayerOverTime()
    {
        while (isAttached)
        {
            playerStats.TakeDamage(stats.damagePerSecond);
            UpdateHealthBar();
            yield return new WaitForSeconds(1f); // wait 1 sec, then damage again
        }
    }



    void UpdateHealthBar()
    {
        if (healthBar == null || playerStats == null) return;

        float ratio = (float)playerStats.currentHealth / playerStats.maxHealth;
        healthBar.size = Mathf.Clamp01(ratio);
    }
}