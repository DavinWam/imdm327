using UnityEngine;

public class GridPlayerController : MonoBehaviour
{
    public GridSystem gridSystem;
    public Vector2Int currentGridPosition;

    private void Start()
    {
        // Subscribe to the grid creation finished event
        gridSystem.OnGridCreationFinished.AddListener(OnGridCreated);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) // Move Up
            MoveToGridPosition(currentGridPosition + Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.S)) // Move Down
            MoveToGridPosition(currentGridPosition + Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.A)) // Move Left
            MoveToGridPosition(currentGridPosition + Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D)) // Move Right
            MoveToGridPosition(currentGridPosition + Vector2Int.right);
    }

    private void MoveToGridPosition(Vector2Int newPosition)
    {
        Panel targetPanel = gridSystem.GetPanel(newPosition);
        if (targetPanel != null)
        {
            currentGridPosition = newPosition;
            transform.position = targetPanel.ObjectReference.transform.position;
        }
    }

    private void OnGridCreated()
    {
        // Start at the first grid position
        currentGridPosition = new Vector2Int(0, 0);
        MoveToGridPosition(currentGridPosition);
    }
}
