using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridSystem : MonoBehaviour
{
    public GameObject spritePrefab; // Prefab for grid sprites
    public Vector2 gridSize = new Vector2(5, 5); // Width x Height of the grid
    public Vector2 cellSpacing = new Vector2(1, 1); // Base spacing between cells (relative to the sprite size)
    public Vector2 panelScale = Vector2.one; // Scale for each grid panel
    public Color gridColor = Color.green; // Color for the grid gizmos
    public UnityEvent OnGridCreationFinished; // Event for when grid creation is complete

    private Vector2 cellSize; // Size of each cell based on sprite
    [HideInInspector]
    public Panel[,] gridPanels;

    private void Start()
    {
        CalculateCellSize();
        CreateGrid();
    }

    private void CalculateCellSize()
    {
        if (spritePrefab != null)
        {
            SpriteRenderer renderer = spritePrefab.GetComponentInChildren<SpriteRenderer>();
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
                    x * (cellSize.x * panelScale.x + cellSpacing.x),
                    y * (cellSize.y * panelScale.y + cellSpacing.y),
                    0
                );

                GameObject gridCell = Instantiate(spritePrefab, position, Quaternion.identity, transform);
                gridCell.transform.localScale = new Vector3(panelScale.x, panelScale.y, 1); // Apply panel scale

                Panel panel = gridCell.GetComponent<Panel>();
                if (panel != null)
                {
                    panel.Initialize(new Vector2Int(x, y), gridCell);
                    gridPanels[x, y] = panel;
                }
                else
                {
                    Debug.LogWarning($"Grid cell at ({x}, {y}) is missing a Panel component.");
                }
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

    // Method to get all panels in a specific column
    public List<Panel> GetColumn(int columnIndex)
    {
        List<Panel> columnPanels = new List<Panel>();

        if (columnIndex >= 0 && columnIndex < gridSize.x)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Panel panel = gridPanels[columnIndex, y];
                if (panel != null)
                {
                    columnPanels.Add(panel);
                }
            }
        }
        else
        {
            Debug.LogWarning("Column index out of range.");
        }

        return columnPanels;
    }

    // Method to get the world position of a panel based on grid coordinates
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < gridSize.x &&
            gridPosition.y >= 0 && gridPosition.y < gridSize.y)
        {
            Panel panel = gridPanels[gridPosition.x, gridPosition.y];
            if (panel != null && panel.ObjectReference != null)
            {
                return panel.ObjectReference.transform.position;
            }
        }

        Debug.LogWarning("Invalid grid position or panel has no object reference.");
        return Vector3.zero;
    }
    // Method to get all panels in a specific row
    public int GetRow()
    {
        return (int)gridSize.y;
    }
    public int GetCol(){
        return (int)gridSize.x;
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
                    x * (cellSize.x * panelScale.x + cellSpacing.x),
                    y * (cellSize.y * panelScale.y + cellSpacing.y),
                    0
                );

                Vector3 size = new Vector3(cellSize.x * panelScale.x, cellSize.y * panelScale.y, 0);
                Gizmos.DrawWireCube(position, size);
            }
        }
    }
}
