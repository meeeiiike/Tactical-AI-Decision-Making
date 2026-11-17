using UnityEngine;
using System;
using UnityEngine.UI;
public class AgentHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    [field: SerializeField]
    public float CurrentHealth {get; private set;}

    [Header("UI")]
    public Image healthBarFill;

    public event Action OnDeath;

    void Start()
    {
        CurrentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return; // Already dead, no need to process further damage
        CurrentHealth -= amount;
        UpdateHealthUI();

        if (CurrentHealth <= 0) Die();
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = CurrentHealth / maxHealth;
        }
    }

    private void Die()
    {
        CurrentHealth = 0;
        OnDeath?.Invoke();
        gameObject.SetActive(false); // For now, just deactivate the GameObject
        Invoke(nameof(Respawn), 3f); 
    }

    private void ResetHealth()
    {
        CurrentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Respawn()
    {
        ResetHealth();
        gameObject.SetActive(true);
    }
  
}
