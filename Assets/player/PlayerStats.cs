using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    //stats
    public int maxHealth = 320;
    public int maxMana = 320;

    public int currentHealth;
    public int currentMana;

    //regenration
    [SerializeField] private int healthRegenPerSecond = 10;
    [SerializeField] private int manaRegenPerSecond = 10;
    [SerializeField] private float regenDelay = 3f;

    //UI
    [SerializeField] private Image deathImage;
    [SerializeField] private Scrollbar healthBar; 

    private float regenTickTimer = 0f;
    private float healthRegenCooldown = 0f;
    private float manaRegenCooldown = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        if (deathImage != null)
            deathImage.gameObject.SetActive(false);

        UpdateHealthBar();
    }

    void Update()
    {
        regenTickTimer += Time.deltaTime;

        if (healthRegenCooldown > 0f)
            healthRegenCooldown = Mathf.Max(0f, healthRegenCooldown - Time.deltaTime);

        if (manaRegenCooldown > 0f)
            manaRegenCooldown = Mathf.Max(0f, manaRegenCooldown - Time.deltaTime);

        if (regenTickTimer >= 1f)
        {
            if (healthRegenCooldown <= 0f)
                RegenerateHealth();

            if (manaRegenCooldown <= 0f)
                RegenerateMana();

            regenTickTimer = 0f;
        }

        UpdateHealthBar();
    }

    void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegenPerSecond;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }

    void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenPerSecond;
            if (currentMana > maxMana)
                currentMana = maxMana;
        }
    }

    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }

    public void ConsumeMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
        manaRegenCooldown = regenDelay;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        healthRegenCooldown = regenDelay;

        if (currentHealth <= 0)
            Die();

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar == null) return;
        float ratio = (float)currentHealth / maxHealth;
        healthBar.size = Mathf.Clamp01(ratio);
    }

    void Die()
    {
        Debug.Log("Player died!");
        if (deathImage != null)
            StartCoroutine(ShowDeathImageAndReload());
        else
            SceneManager.LoadScene("HomeWorld");
    }

    IEnumerator ShowDeathImageAndReload()
    {
        deathImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("HomeWorld");
    }
}