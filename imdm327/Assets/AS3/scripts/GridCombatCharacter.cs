using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DamageEvent : UnityEvent<GameObject, float> { }

public class GridCombatCharacter : MonoBehaviour
{
    public GridCharacterStats stats; // Assign the SO instance here in the Inspector
    public UnityEvent onDeath; // Event for when the object is destroyed
    public DamageEvent onDamageTaken; // Event for when the object takes damage

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
                    TakeDamage(other.gameObject, damage);
                }
            }
        }




    public void TakeDamage(GameObject attacker, float damage)
    {
        if (stats == null)
            return;

        // Reduce health
        stats.currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage from {attacker.name}. Remaining health: {stats.currentHealth}");

        // Invoke the damage taken event
        onDamageTaken?.Invoke(attacker, damage);

        // Check if health is 0 or less
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        onDeath?.Invoke();
        Destroy(gameObject); // Destroy the GameObject
    }
}
