using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    // public beause we need it somewhere else
    public int maxHealth = 320;
    public int maxMana = 320;

    public int currentHealth;
    public int currentMana;



    //death screen
    [SerializeField] private Image deathImage;




    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;



        if (deathImage != null)
            deathImage.gameObject.SetActive(false); // hide at start
    }




    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }




    public void ConsumeMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
        Debug.Log($"Drained mana: {amount} | Remaining: {currentMana}");
    }




    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"That hurt!!!: {amount} | Remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
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
        yield return new WaitForSeconds(1f); // show for 1 second
        SceneManager.LoadScene("HomeWorld");
    }
}