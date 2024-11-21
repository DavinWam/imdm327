using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAIController : GridController
{
    public GridPlayerController playerController; // Reference to the player's controller
    public int maxPlacedBeats = 4; // Maximum number of beats the AI can place
    public bool onlyPlaceAttack; // Toggle for placing an attack and not moving
    public float delayBetweenMoves = 0.5f; // Delay before moving to avoid being too sticky
    public float abilityPlacementChance = 0.5f; // Chance to place an ability (0 to 1)

    private int placedBeats = 0; // Tracks the number of placed beats
    public Beat beat;
    private Coroutine aiRoutine;

    protected override void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned.");
            return;
        }
        base.Start();
    }

    private IEnumerator AIBehavior()
    {
        while (true)
        {
            if (isMoving)
            {
                yield return null; // Wait if the AI is currently moving
                continue;
            }

            // Determine movement direction based on the beat
            bool moveTowardsPlayerRow = beat.currentBeat > 4; // Adjust as per your beat logic

            Vector2Int newPosition = GetNextPositionTowardsOrAwayFromPlayerRow(moveTowardsPlayerRow);

            if (newPosition != currentGridPosition)
            {
                Panel targetPanel = gridSystem.GetPanel(newPosition);
                if (targetPanel != null)
                    yield return StartCoroutine(MoveToGridPosition(newPosition, targetPanel));
            }

            // Decide whether to place an ability based on chance
            if (placedBeats < maxPlacedBeats)
            {
                if (Random.value <= abilityPlacementChance)
                {
                    panel.SetSynthAbility(BassAbility);
                    placedBeats++;
                }
            }

            // Add a delay to avoid being too sticky
            yield return new WaitForSeconds(delayBetweenMoves);
        }
    }

    private Vector2Int GetNextPositionTowardsOrAwayFromPlayerRow(bool towardsPlayerRow)
    {
        Vector2Int playerPosition = playerController.currentGridPosition;
        Vector2Int currentPosition = currentGridPosition;
        int rows = gridSystem.GetRow(); // Number of columns
        int cols = gridSystem.GetCol(); // Number of rows

        int directionY = 0;

        if (playerPosition.y > currentPosition.y)
        {
            // Player is above
            directionY = towardsPlayerRow ? 1 : -1;
        }
        else if (playerPosition.y < currentPosition.y)
        {
            // Player is below
            directionY = towardsPlayerRow ? -1 : 1;
        }
        else // Same row
        {
            if (towardsPlayerRow)
            {
                directionY = 0; // Already on the player's row
            }
            else
            {
                // Move away from player's row
                if (currentPosition.y > 0)
                    directionY = -1; // Move down
                else if (currentPosition.y < cols - 1)
                    directionY = 1; // Move up
            }
        }

        // Randomly decide to shift left (-1), right (1), or stay (0)
        int directionX = Random.Range(-1, 2); // Returns -1, 0, or 1

        // Calculate new position
        Vector2Int newPosition = currentPosition + new Vector2Int(directionX, directionY);

        // Ensure newPosition is within grid bounds
        newPosition.x = Mathf.Clamp(newPosition.x, 0, rows - 1);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, cols - 1);

        return newPosition;
    }


    private IEnumerator MoveToGridPosition(Vector2Int newPosition, Panel targetPanel)
    {
        isMoving = true;
        panel.ClearCharacter();

        Vector3 startPosition = transform.position;
        Vector3 endPosition = gridSystem.GetWorldPosition(newPosition);
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        currentGridPosition = newPosition;
        isMoving = false;
        targetPanel.SetCharacter(this);
        panel = targetPanel;
    }

    protected override void OnGridCreated()
    {
        base.OnGridCreated();
        if (onlyPlaceAttack)
        {
            panel.SetSynthAbility(BassAbility);
        }
        else
        {
            aiRoutine = StartCoroutine(AIBehavior());
        }
    }
}
