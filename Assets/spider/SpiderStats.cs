using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpiderVariant
{
    BrownStone,
    RedFire,
    YellowWind,
    CyanIce,
    BlueWater,
    WhiteBoss
}



public class SpiderStats : MonoBehaviour
{

    // public to be able to communicate 
    public SpiderVariant variant;

    public int health = 20;
    public int damagePerSecond = 20;


    // one material for all spiders 
    [SerializeField] private Material _spiderMaterial;


    // dropped stone
    public GameObject _magicStone;


    // face chase player 
    public Transform player;
    

    //delete spider at its root
    [SerializeField] private GameObject _rootSpider;

// to differiante the whole magic stone from just the child object 
    [SerializeField] private GameObject _stoneRoot;


    // for material 
    private Renderer _spiderRenderer;

    [SerializeField] private AudioSource _damage;



    // EYES

    private Material _eyes; 
    [SerializeField] private Material _eyesOriginal;
    public float _eyepauseTime = 0.25f;
    [SerializeField] private Renderer[] _eyesRenderers; 






    // death effects
    [SerializeField] private GameObject deathParticleEffect;

    //[SerializeField] private AudioSource _deathSound;
    
    //private Color originalColor;

    

    






    void Start()
    {
        if (player == null)
        {
            //GameObject p = GameObject.FindGameObjectWithTag("Player"); // find player
            //if (p != null) player = p.transform;
        }

        _spiderRenderer = GetComponent<Renderer>();



        //originalColor = _spiderMaterial.GetColor("_Color");

        _eyes = new Material(_eyesOriginal);
        foreach (var r in _eyesRenderers)
        {
            r.material = _eyes;  // give all eye renderers the same unique material
        }



       
        UpdateStats();

        if (_stoneRoot != null)
            _stoneRoot.SetActive(false);
    }

    private void OnValidate()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (_spiderRenderer == null)
            _spiderRenderer = GetComponent<Renderer>();

        if (_spiderMaterial != null && _spiderRenderer != null)
            _spiderRenderer.material = _spiderMaterial;

        switch (variant) // different material and body size
        {
            case SpiderVariant.BrownStone:
                health = 20;
                damagePerSecond = 20;
                SetMaterialColor(new Color32(120, 85, 60, 255));
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case SpiderVariant.RedFire:
                health = 30;
                damagePerSecond = 20;
                SetMaterialColor(new Color32(180, 60, 50, 255));
                transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                break;
            case SpiderVariant.YellowWind:
                health = 40;
                damagePerSecond = 20;
                SetMaterialColor(new Color32(230, 200, 90, 255));
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case SpiderVariant.CyanIce:
                health = 50;
                damagePerSecond = 20;
                SetMaterialColor(new Color32(120, 200, 210, 255));
                transform.localScale = new Vector3(0.9f, 0.5f, 0.9f);
                break;
            case SpiderVariant.BlueWater:
                health = 60;
                damagePerSecond = 20;
                SetMaterialColor(new Color32(70, 100, 200, 255));
                transform.localScale = new Vector3(1f, 0.4f, 1f);
                break;
            case SpiderVariant.WhiteBoss:
                health = 70;
                damagePerSecond = 20;
                SetMaterialColor(new Color32(220, 220, 220, 255));
                transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }



    void SetMaterialColor(Color color)
    {
        if (_spiderMaterial != null)
        {
            _spiderMaterial.SetColor("_Color", color);
            if (_spiderMaterial.HasProperty("_BaseColor"))
                _spiderMaterial.SetColor("_BaseColor", color);
        }
    }



    public void TakeDamage(int amount)
    {

        _damage.Play();


        health -= amount;
        Debug.Log($"{variant} spider took {amount} damage. Health now {health}");



        if (health <= 0)
        {
            Die();
        }


        //StartCoroutine(DamageFlash());
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        _eyes.EnableKeyword("_EMISSION");

        for (int i = 0; i < 2; i++)
        {
            _eyes.SetColor("_EmissionColor", Color.red); // Red
            yield return new WaitForSeconds(_eyepauseTime);

            _eyes.SetColor("_EmissionColor", Color.black); // Off
            yield return new WaitForSeconds(_eyepauseTime);
        }

        _eyes.SetColor("_EmissionColor", Color.red); // End on red
    }


    
    /*

    private IEnumerator DamageFlash()
    {
        SetMaterialColor(Color.red);
        yield return new WaitForSeconds(0.2f);
        UpdateStatsColorOnly();   
    }

    private void UpdateStatsColorOnly()
{
    switch (variant)
    {
        case SpiderVariant.BrownStone:
            SetMaterialColor(new Color32(120, 85, 60, 255));
            break;
        case SpiderVariant.RedFire:
            SetMaterialColor(new Color32(180, 60, 50, 255));
            break;
        case SpiderVariant.YellowWind:
            SetMaterialColor(new Color32(230, 200, 90, 255));
            break;
        case SpiderVariant.CyanIce:
            SetMaterialColor(new Color32(120, 200, 210, 255));
            break;
        case SpiderVariant.BlueWater:
            SetMaterialColor(new Color32(70, 100, 200, 255));
            break;
        case SpiderVariant.WhiteBoss:
            SetMaterialColor(new Color32(220, 220, 220, 255));
            break;
    }
}

*/




    public int GetDamage() => damagePerSecond;

    void Die()
    {
        Debug.Log($"{variant} spider died!");

        Instantiate(deathParticleEffect, transform.position, Quaternion.identity);

        if (_stoneRoot != null)
            _stoneRoot.SetActive(true);

        if (_magicStone != null)
        {
            GameObject stone = Instantiate(_magicStone, transform.position, Quaternion.identity);
            MagicStone ms = stone.GetComponent<MagicStone>();
            if (ms != null)
            {
                switch (variant) // to make sure same spider drop same magic stone
                {
                    case SpiderVariant.BrownStone: ms.stoneType = MagicStone.TypeList.stone; ms.stoneColor = MagicStone.ColorList.brown; break;
                    case SpiderVariant.RedFire: ms.stoneType = MagicStone.TypeList.fire; ms.stoneColor = MagicStone.ColorList.red; break;
                    case SpiderVariant.YellowWind: ms.stoneType = MagicStone.TypeList.wind; ms.stoneColor = MagicStone.ColorList.yellow; break;
                    case SpiderVariant.CyanIce: ms.stoneType = MagicStone.TypeList.ice; ms.stoneColor = MagicStone.ColorList.cyan; break;
                    case SpiderVariant.BlueWater: ms.stoneType = MagicStone.TypeList.water; ms.stoneColor = MagicStone.ColorList.blue; break;
                    case SpiderVariant.WhiteBoss: ms.stoneType = MagicStone.TypeList.light; ms.stoneColor = MagicStone.ColorList.white; break;
                }

                ms.UpdateStoneAppearance();
            }
        }

        if (_rootSpider != null)
            Destroy(_rootSpider);
        else
            Destroy(gameObject);
    }
}