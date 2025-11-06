using System.Collections;
using UnityEngine;

public class FaceSpiderController : MonoBehaviour
{
    [SerializeField] private SpiderStats stats;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject _staff; 

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
        _staff.SetActive(false); 
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            clickCounter++;
            if (clickCounter >= clicksToDetach)
            {
                DetachFromPlayer();
                _staff.SetActive(true);
            }
            
        }
    }

    void UpdateClickRequirement()
    {
        if (stats == null) return;

        switch (stats.variant)
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

       //_staff.SetActive(false); 

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamagePlayerOverTime());
    }

    void DetachFromPlayer()
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
            yield return new WaitForSeconds(1f);
        }
    }
}