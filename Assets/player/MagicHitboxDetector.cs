using System.Collections.Generic;
using UnityEngine;

public class MagicHitboxDetector : MonoBehaviour
{
    public HashSet<SpiderStats> spidersInRange = new HashSet<SpiderStats>();

    

    void OnTriggerEnter(Collider other)
    {
        SpiderStats s = other.GetComponent<SpiderStats>();
        if (s != null)
            spidersInRange.Add(s);
    }

    void OnTriggerExit(Collider other)
    {
        SpiderStats s = other.GetComponent<SpiderStats>();
        if (s != null)
            spidersInRange.Remove(s);
    }
}