using UnityEngine;

public class Panel
{
    public Vector2Int GridPosition { get; private set; }
    public GameObject ObjectReference { get; private set; }

    public Panel(Vector2Int gridPosition, GameObject objectReference)
    {
        GridPosition = gridPosition;
        ObjectReference = objectReference;
    }

    public void SetObjectReference(GameObject obj)
    {
        ObjectReference = obj;
    }
}
