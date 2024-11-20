using UnityEngine;
using UnityEngine.Events;

public class GridSystem : MonoBehaviour
{
    public GameObject spritePrefab; // Prefab for grid sprites
    public Vector2 gridSize = new Vector2(5, 5); // Width x Height of the grid
    public Vector2 cellSpacing = new Vector2(1, 1); // Base spacing between cells (relative to the sprite size)
    public Color gridColor = Color.green; // Color for the grid gizmos

    public UnityEvent OnGridCreationFinished; // Event for when grid creation is complete

    private Vector2 cellSize; // Size of each cell based on sprite
    private Panel[,] gridPanels;

    private void Start()
    {
        CalculateCellSize();
        CreateGrid();
    }

    private void CalculateCellSize()
    {
        if (spritePrefab != null)
        {
            SpriteRenderer renderer = spritePrefab.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sprite != null)
            {
                // Use the sprite's size as the cell size
                cellSize = renderer.sprite.bounds.size;
            }
            else
            {
                Debug.LogWarning("SpritePrefab is missing a SpriteRenderer or Sprite. Defaulting cell size to (1, 1).");
                cellSize = new Vector2(1, 1);
            }
        }
        else
        {
            Debug.LogError("SpritePrefab is not assigned! Defaulting cell size to (1, 1).");
            cellSize = new Vector2(1, 1);
        }
    }

    private void CreateGrid()
    {
        gridPanels = new Panel[(int)gridSize.x, (int)gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 position = transform.position + new Vector3(
                    x * (cellSize.x + cellSpacing.x),
                    y * (cellSize.y + cellSpacing.y),
                    0
                );

                GameObject gridCell = Instantiate(spritePrefab, position, Quaternion.identity, transform);
                gridCell.transform.localScale = Vector3.one; // Ensure scale matches sprite dimensions

                gridPanels[x, y] = new Panel(new Vector2Int(x, y), gridCell);
            }
        }

        // Trigger the event to signal grid creation is complete
        OnGridCreationFinished?.Invoke();
    }

    public Panel GetPanel(Vector2Int gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < gridSize.x &&
            gridPosition.y >= 0 && gridPosition.y < gridSize.y)
        {
            return gridPanels[gridPosition.x, gridPosition.y];
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        if (gridSize == Vector2.zero || spritePrefab == null)
            return;

        Gizmos.color = gridColor;

        CalculateCellSize(); // Ensure gizmos reflect the sprite size in editor

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 position = transform.position + new Vector3(
                    x * (cellSize.x + cellSpacing.x),
                    y * (cellSize.y + cellSpacing.y),
                    0
                );

                Vector3 size = new Vector3(cellSize.x, cellSize.y, 0);
                Gizmos.DrawWireCube(position, size);
            }
        }
    }
}
