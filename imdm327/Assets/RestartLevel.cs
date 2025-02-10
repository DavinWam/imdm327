using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    public TMP_Text waveText; // Reference to the Text UI component

    private EnemyWaveSpawner waveSpawner;
    // Start is called before the first frame update
    void Start()
    {
        // Find the WaveSpawner in the scene
        waveSpawner = FindObjectOfType<EnemyWaveSpawner>();

        if (waveSpawner == null)
        {
            Debug.LogError("WaveSpawner not found in the scene.");
            return;
        }
    }
    void Update(){
        UpdateWaveText();
    }
    public void ReloadCurrentLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void UpdateWaveText()
    {
        if (waveSpawner != null && waveText != null)
        {
            int currentWave = waveSpawner.currentWave;
            waveText.text = $"Got To Wave: {currentWave}";
        }
    }

}
