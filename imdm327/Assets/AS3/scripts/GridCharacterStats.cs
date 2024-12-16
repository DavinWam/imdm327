using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewStats", menuName = "Gameplay/GridCharStats", order = 1)]
public class GridCharacterStats : ScriptableObject
{
    public float maxHealth = 100f;
    public float currentHealth;

    // Reset health when the object is instantiated
    private void OnEnable()
    {
        currentHealth = maxHealth;
    }
    public void Heal(float amount){
        currentHealth += amount;
        if (currentHealth>maxHealth){
            currentHealth = maxHealth;
        }   
    }  

}
