using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GridController : MonoBehaviour
{
    public enum CharacterType{
        player,
        enemy
    }
    public GridSystem gridSystem  {get; protected set;}  
    public Panel panel { get; protected set; }
    public Vector2Int currentGridPosition;
    public float moveDuration = 0.5f; // Time to move to the next grid

    public UnityEvent<Vector2Int> OnMoveStart; // Event with direction
    public UnityEvent OnMoveEnd;
    public SynthAbility BassAbility;
    protected bool isMoving = false;
    public CharacterType  t = CharacterType.enemy;
    protected virtual void Start()
    
    {

        FindGrid();
        if (isMoving)
        if (gridSystem == null)
        {
            Debug.LogError("GridSystem is not assigned.");
            return;
        }

        // Subscribe to the grid creation finished event
        if(gridSystem.init == false){
            gridSystem.OnGridCreationFinished.AddListener(OnGridCreated);
        }else{
            OnGridCreated();
        }
        

        // Initialize events
        if (OnMoveStart == null)
            OnMoveStart = new UnityEvent<Vector2Int>();
        if (OnMoveEnd == null)
            OnMoveEnd = new UnityEvent();
            
        if(BassAbility){
            BassAbility = Instantiate(BassAbility);
            BassAbility.owner = gameObject;
            BassAbility.animator = GetComponentInChildren<Animator>();
        }
    }

    private void FindGrid(){
        if(t == CharacterType.player){
            gridSystem = GameObject.FindWithTag("PlayerGrid").GetComponent<GridSystem>();
        }else{
            gridSystem = GameObject.FindWithTag("EnemyGrid").GetComponent<GridSystem>();
        }
    }
    protected IEnumerator MoveToGridPosition(Vector2Int newPosition, Vector2Int direction,Panel target)
    {
        if(isMoving) yield return null;
        isMoving = true;
        OnMoveStart?.Invoke(direction);
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
        OnMoveEnd?.Invoke();
        target.SetCharacter(this);
        panel = target;
    }
    protected IEnumerator MoveToGridPosition(Vector2Int newPosition, Panel targetPanel)
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
    protected virtual void OnGridCreated()
    {
        // Start at the first grid position
        panel = gridSystem.GetPanel(currentGridPosition);
        panel.SetCharacter(this);
        transform.position = gridSystem.GetWorldPosition(currentGridPosition);
    }
    public void Move(Vector2Int position)
    {
        if(gridSystem == null){
            FindGrid();
        }
        // Start at the first grid position
        panel = gridSystem.GetPanel(position);
        panel.SetCharacter(this);
        transform.position = gridSystem.GetWorldPosition(position);
        StartCoroutine(MoveToGridPosition(position, panel));
    }

}
