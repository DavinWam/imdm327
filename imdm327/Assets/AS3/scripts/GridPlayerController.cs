using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GridPlayerController : GridController
{


    public SynthAbility SineAbility;
    protected override void Start()
    {
       
        if(SineAbility != null){
            SineAbility.owner = gameObject;
            SineAbility.animator = GetComponentInChildren<Animator>();
         }
         base.Start();

    }   

    private void Update()
    {
        if (isMoving)
            return;

        Vector2Int direction = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) // Move Up
            direction = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) // Move Down
            direction = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) // Move Left
            direction = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) // Move Right
            direction = Vector2Int.right;

        if (direction != Vector2Int.zero)
        {
            Vector2Int newPosition = currentGridPosition + direction;
            Panel targetPanel = gridSystem.GetPanel(newPosition);
            if (targetPanel != null)
            {
                StartCoroutine(MoveToGridPosition(newPosition, direction, targetPanel));
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            panel.SetSynthAbility(BassAbility); // Replace "BassAbility" with the appropriate SynthAbility instance
            Debug.Log("SynthAbility set to BassAbility.");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            panel.SetSynthAbility(SineAbility); // Replace "BassAbility" with the appropriate SynthAbility instance
            Debug.Log("SynthAbility set to BassAbility.");
        }


        // Clear SynthAbility when Spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            panel.clearSynth(); // Clear the SynthAbility
            Debug.Log("SynthAbility cleared.");
        }
    }

}
