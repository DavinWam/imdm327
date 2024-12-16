using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DamageEvent : UnityEvent<GameObject, float> { }

public class GridCombatCharacter : MonoBehaviour
{
    public GridCharacterStats stats; // Assign the SO instance here in the Inspector
    public UnityEvent onDeath; // Event for when the object is destroyed
    public UnityEvent<GameObject, float>  onDamageTaken; // Event for when the object takes damage

    public LayerMask damageLayer; // Assign the damage layer (e.g., EnemyDamage or PlayerDamage)

    private void Awake()
    {
        if (stats == null)
        {
            Debug.LogError($"HealthData not assigned to {gameObject.name}");
            return;
        }

        // Restore health on awake
        stats.currentHealth = stats.maxHealth;
        stats = Instantiate(stats);
    }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the other object's layer matches the damage layer
            if ((damageLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                // Check if the other object has a SynthAbility component
                SynthAbility synthAbility = other.GetComponent<SynthAttack>().synthAbility;
                if (synthAbility != null)
                {
                    // Use the damage value from the SynthAbility
                    float damage = synthAbility.damage;

                    // Apply damage
                    TakeDamage(other.gameObject, damage,synthAbility.hitstop);
                }
            }
        }




    public void TakeDamage(GameObject attacker, float damage, float hitstop)
    {
        if (stats == null)
            return;

        // Reduce health
        stats.currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage from {attacker.name}. Remaining health: {stats.currentHealth}");

        // Invoke the damage taken event
        onDamageTaken?.Invoke(attacker, damage);
        
        // Trigger hitstop effect
        StartCoroutine(ApplyHitstop(hitstop));
        //does death check after hitstop ends
    }
    public void ResolveDamage(){
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(float amount){
        stats.Heal(amount);
    }    
    private IEnumerator ApplyHitstop(float duration)
    {
        // Cache original timescale
        float originalTimeScale = Time.timeScale;

        // Set timescale to 0 for the hitstop duration
        // Time.timeScale = 0;

        // Wait for the hitstop duration in real time (ignores Time.timeScale)
        yield return new WaitForSecondsRealtime(duration);

        // Restore original timescale
        Time.timeScale = originalTimeScale;
        ResolveDamage();
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        onDeath?.Invoke();
        Destroy(gameObject); // Destroy the GameObject
    }
}
