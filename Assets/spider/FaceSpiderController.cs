using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSpiderController : MonoBehaviour
{
    SpiderStats stats;
    int clickCounter;
    int clicksToDetach = 9;

    public void SetStats(SpiderStats spiderStats)
    {
        stats = spiderStats;
        UpdateClickRequirement();
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            clickCounter++;

            if (clickCounter >= clicksToDetach)
            {
                gameObject.SetActive(false);
                clickCounter = 0;
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
}