using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Beat : MonoBehaviour
{
    public GridSystem playerGrid;
    public GridSystem enemyGrid;
    private int numGridsReady;
    public GameObject beatObject;
    public float bpm = 120.0f; // Beats per minute
    public UnityEvent<int> OnBeat; // Event to broadcast the current beat

    private Vector2Int currentGridPosition;
    private float beatDuration;
    public int currentBeat {get; private set;} = 1;

    private void Start()
    {
        if (beatObject == null)
        {
            Debug.LogError("Beat object is not assigned.");
            return;
        }

        // Initialize the UnityEvent
        if (OnBeat == null)
            OnBeat = new UnityEvent<int>();

        // Calculate the duration of one beat in seconds
        beatDuration = 60.0f / bpm;

        // Subscribe to grid creation finished events
        playerGrid.OnGridCreationFinished.AddListener(OnGridReady);
        enemyGrid.OnGridCreationFinished.AddListener(OnGridReady);
    }

    private void OnGridReady()
    {
        numGridsReady++;
        if (numGridsReady == 2)
        {
            // Initialize the beat object's position
            beatObject.transform.position = playerGrid.GetWorldPosition(new Vector2Int(0, 0));

            // Start the movement coroutine
            StartCoroutine(MoveBeat());
        }
    }

   private IEnumerator MoveBeat()
{
    while (true)
    {
        // Define the sequence of grids to traverse
        GridSystem[] grids = { playerGrid, enemyGrid };

        foreach (GridSystem grid in grids)
        {
            for (int x = 0; x < grid.gridSize.x; x++)
            {
                currentGridPosition = new Vector2Int(x, 0);
                Vector3 targetPosition = grid.GetWorldPosition(currentGridPosition);

                // Trigger the OnBeat event with the current beat number
                OnBeat?.Invoke(currentBeat);

                // Trigger the OnBeat event for each panel in the current column
                List<Panel> columnPanels = grid.GetColumn(x);
                foreach (Panel panel in columnPanels)
                {
                    panel.TriggerBeat();
                }

                // Move to the target position over the duration of one beat
               // yield return MoveToPosition(targetPosition, beatDuration);
                        beatObject.transform.position = targetPosition;
                        yield return new WaitForSeconds(beatDuration);
                // Increment and wrap the beat counter
                currentBeat = (currentBeat % 8) + 1;
            }
        }
    }
}


}
