using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public List<SpawnPair> pairs;
    }
    [System.Serializable]
    public class SpawnPair{
        public Vector2Int spawnPosition;
        public GameObject enemyPrefab;
    }
    public int currentWave {get; private set;}
    public List<Wave> waves; // List of waves to spawn
    public float waveDelay = 2f; // Delay between waves, in seconds
    public UnityEvent WaveEnd;
    public AnimationClip transitionAnimation; // Animation clip for transition between waves


    private List<GameObject> activeEnemies = new List<GameObject>(); // List of active enemies

    private void Start()
    {
        GridSystem gs = GameObject.FindGameObjectWithTag("EnemyGrid").GetComponent<GridSystem>(); 
        if(gs.init){
            StartWaves();
        }else{
            gs.OnGridCreationFinished.AddListener(StartWaves);
        }
        
        currentWave = 1;

    }
    private void StartWaves(){
        if (waves.Count > 0)
        {
            StartCoroutine(SpawnWaveCoroutine());
        }
        else
        {
            Debug.LogError("No waves defined in the WaveSpawner.");
        }
    }
    private IEnumerator SpawnWaveCoroutine()
    {
            WaveEnd.Invoke();
            // Wait for the animation delay
            yield return new WaitForSeconds(transitionAnimation.length);
            // Pick a random wave index
            int randomWaveIndex = Random.Range(0, waves.Count);

            // Spawn the selected wave
            SpawnWave(waves[randomWaveIndex]);

            // Wait for all enemies in the current wave to be destroyed
            yield return null;

        
    }


    private void SpawnWave(Wave wave)
    {
        Debug.Log($"Spawning wave {currentWave + 1}/{waves.Count}");

        foreach (SpawnPair spawnPair in wave.pairs)
        {
            // Instantiate the enemy prefab
            GameObject enemy = Instantiate(spawnPair.enemyPrefab, transform);

            // Get the GridCombatCharacter component
            GridCombatCharacter combatCharacter = enemy.GetComponent<GridCombatCharacter>();
            GridController gridController = enemy.GetComponent<GridController>();

            if (combatCharacter == null || gridController == null)
            {
                Debug.LogError("Enemy prefab is missing GridCombatCharacter or controller component. ");

                Destroy(enemy);
            }else{
                // Subscribe to the enemy's death event
                combatCharacter.onDeath.AddListener(() => OnEnemyDeath(enemy));
                
                gridController.Move(spawnPair.spawnPosition);

                // Add the enemy to the active list
                activeEnemies.Add(enemy);
            }

        }
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        // Remove the enemy from the active list
        activeEnemies.Remove(enemy);

        Debug.Log($"Enemy {enemy.name} defeated. Remaining enemies: {activeEnemies.Count}");
        if(activeEnemies.Count == 0){
            currentWave++;
            
            Debug.Log("wave end");
            StartCoroutine(SpawnWaveCoroutine());
        }
    }
}
