using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    [Header("Boss Object (drag your boss here)")]
    public GameObject bossObject;

    [Header("Smaller Objects To Activate / Destroy")]
    public List<GameObject> objectsToActivate;

    [Header("Timer Settings")]
    public float totalTime = 60f;
    public float spawnInterval = 3f;

    [Header("UI")]
    public TMP_Text timerText;

    [Header("Particles")]
    public GameObject spawnEffect;     // plays when a small object activates
    public GameObject bossDeadEffect;  // plays when boss dies
    public GameObject smallKillEffect; // plays ON EACH small object before boss dies

    private float timer;
    private bool isRunning = true;
    private Coroutine spawnRoutine;

    private void Start()
    {
        timer = totalTime;

        // disable small objects initially
        foreach (var obj in objectsToActivate)
            if (obj != null)
                obj.SetActive(false);

        spawnRoutine = StartCoroutine(SpawnRoutine());
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isRunning) return;

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        UpdateTimerUI();

        if (timer <= 0 && isRunning)
            StartCoroutine(KillSequence());
    }

    IEnumerator SpawnRoutine()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(spawnInterval);
            ActivateRandomObject();
        }
    }

    private void ActivateRandomObject()
    {
        List<GameObject> inactive = objectsToActivate.FindAll(o => o != null && !o.activeSelf);

        if (inactive.Count == 0) return;

        GameObject chosen = inactive[Random.Range(0, inactive.Count)];
        chosen.SetActive(true);

        if (spawnEffect != null)
            Instantiate(spawnEffect, chosen.transform.position, Quaternion.identity);
    }

    // ----------------------
    // FULL END SEQUENCE !!!
    // ----------------------
    IEnumerator KillSequence()
    {
        isRunning = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        // 1) Kill all small objects FIRST
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj == null) continue;

            // play small kill particle on each one
            if (smallKillEffect != null)
                Instantiate(smallKillEffect, obj.transform.position, Quaternion.identity);

            Destroy(obj);
        }

        // 2) WAIT 3 seconds before killing the boss
        yield return new WaitForSeconds(3f);

        // 3) Kill the boss and play the big effect
        if (bossObject != null)
        {
            if (bossDeadEffect != null)
                Instantiate(bossDeadEffect, bossObject.transform.position, Quaternion.identity);

            Destroy(bossObject);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int t = Mathf.CeilToInt(timer);
        int min = t / 60;
        int sec = t % 60;

        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }
}