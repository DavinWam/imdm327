using UnityEngine;

[RequireComponent(typeof(ShearSprite))]
public class GridAnimationController : MonoBehaviour
{
    public GridPlayerController playerController;
    private ShearSprite shearSprite;

    private void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned.");
            return;
        }

        shearSprite = GetComponent<ShearSprite>();

        // Subscribe to movement events
        playerController.OnMoveStart.AddListener(HandleMoveStart);
        playerController.OnMoveEnd.AddListener(HandleMoveEnd);
    }

    private void HandleMoveStart(Vector2Int direction)
    {
        // Adjust shear based on movement direction
        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            shearSprite.shearAmountX = 0.0f;
            shearSprite.shearAmountY = direction.y * 0.1f; // Adjust the shear amount as needed
        }
        else if (direction == Vector2Int.left || direction == Vector2Int.right)
        {
            shearSprite.shearAmountX = direction.x * 0.1f; // Adjust the shear amount as needed
            shearSprite.shearAmountY = 0.0f;
        }
    }

    private void HandleMoveEnd()
    {
        // Reset shear when movement ends
        shearSprite.shearAmountX = 0.0f;
        shearSprite.shearAmountY = 0.0f;
    }
}
