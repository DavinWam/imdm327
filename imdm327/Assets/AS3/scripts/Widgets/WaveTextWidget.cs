using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveTextWidget : MonoBehaviour
{
    public TMP_Text waveText; // Reference to the Text UI component

    private EnemyWaveSpawner waveSpawner;

    private void Start()
    {
        // Find the WaveSpawner in the scene
        waveSpawner = FindObjectOfType<EnemyWaveSpawner>();

        if (waveSpawner == null)
        {
            Debug.LogError("WaveSpawner not found in the scene.");
            return;
        }

        // Update the text initially
        // UpdateWaveText();

        // Start listening for wave changes
        // StartCoroutine(UpdateWaveTextCoroutine());
    }

    public void UpdateWaveText()
    {
        if (waveSpawner != null && waveText != null)
        {
            int currentWave = waveSpawner.currentWave;
            waveText.text = $"Wave: {currentWave}";
        }
        GetComponent<Animator>().SetTrigger("NewWave");
    }

    private IEnumerator UpdateWaveTextCoroutine()
    {
        while (true)
        {
            UpdateWaveText();
            yield return new WaitForSeconds(0.5f); // Adjust update frequency as needed
        }
    }
}

