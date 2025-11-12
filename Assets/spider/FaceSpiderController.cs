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


    [SerializeField] private GameObject destroyParticlePrefab; // particle prefab for spider death

    [SerializeField] private GameObject _tensionImage; // UI image for tension

    [SerializeField] private AudioSource _tensionAudio; // audio for tension


    








    public void SetStats(SpiderStats spiderStats)
    {
        stats = spiderStats;
        UpdateClickRequirement();
        clickCounter = 0;

        AttachToPlayer();
    }

    /*void Start()
    {
        AttachToPlayer();
        _staff.SetActive(false); 
    }*/

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(1))
        {
            clickCounter++;
            if (clickCounter >= clicksToDetach)
            {
                DetachFromPlayer();
                _staff.SetActive(true);
            }

        }
    }

    void OnEnable()
    {
        clickCounter = 0;
        isAttached = false;

        // hide staff at start
        if (_staff != null)
            _staff.SetActive(false);

        // show tension UI
        if (_tensionImage != null)
            _tensionImage.SetActive(true);

        // play tension audio
        if (_tensionAudio != null)
            _tensionAudio.Play();

        AttachToPlayer();

        // Show "press mouse repeatedly" UI in Japanese
        UIController.instance.EKEY("<b>みぎマウス</b>をクリック!!");
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

        if (_tensionImage != null)
            _tensionImage.SetActive(true);

        if (_tensionAudio != null && !_tensionAudio.isPlaying)
            _tensionAudio.Play();

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamagePlayerOverTime());
    }


    void DetachFromPlayer()
    {

        UIController.instance.EKEY("");

        // spawn destroy particle
        if (destroyParticlePrefab != null)
            Instantiate(destroyParticlePrefab, transform.position, Quaternion.identity);

        // hide tension image
        if (_tensionImage != null)
            _tensionImage.SetActive(false);

        // stop tension audio
        if (_tensionAudio != null && _tensionAudio.isPlaying)
            _tensionAudio.Stop();

        // remove from scene
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