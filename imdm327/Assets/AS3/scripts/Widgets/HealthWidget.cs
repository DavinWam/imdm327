using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthWidget : MonoBehaviour
{
    public TMP_Text healthText;
    public GridCombatCharacter character;
    // Start is called before the first frame update
    void Start()
    {
        
        UpdateHealthText(character.gameObject,character.stats.currentHealth);
    }

    // Update is called once per frame
    public void UpdateHealthText(GameObject gameObject, float health){
        healthText.text = character.stats.currentHealth.ToString();
    }
}
