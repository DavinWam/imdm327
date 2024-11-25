using UnityEngine;
using UnityEngine.Events;

public class Panel : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    public GridController CharacterSlot { get; private set; }
    public SynthAbility SynthAbilitySlot { get; private set; }
    public GameObject ObjectReference { get; private set; }
    public UnityEvent OnBeat; // Event triggered on each beat
    public Animator animator;
    public ESynthType allowedSynth;
     Color OriginalColor;
    public void Initialize(Vector2Int gridPosition, GameObject objectReference)
    {
        ObjectReference = objectReference;
        GridPosition = gridPosition;
        OnBeat = new UnityEvent();
        // Get the SpriteRenderer from the only child
        if (transform.childCount > 0)
        {
            childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        if (childSpriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the child object!");
        }
        OriginalColor = childSpriteRenderer.material.GetColor("_Color");

    }

    public void SetCharacter(GridController character)
    {
        CharacterSlot = character;
    }

    [Header("Colors for Synth Types")]
    public Color bassColor = new Color(0.29f, 0.00f, 0.51f);   // Deep Purple
    public Color leadColor = new Color(1.00f, 0.27f, 0.00f);   // Bright Red
    public Color padColor = new Color(0.27f, 0.51f, 0.71f);    // Soft Blue
    public Color rhythmColor = new Color(1.00f, 0.84f, 0.00f); // Golden Yellow
    public Color effectsColor = new Color(0.20f, 0.80f, 0.20f); // Vibrant Green

    private SpriteRenderer childSpriteRenderer;
  public void SetSynthAbility(SynthAbility synthAbility)
{
    SynthAbilitySlot = synthAbility;

    if (SynthAbilitySlot == null)
    {
        Debug.LogWarning("No SynthAbility assigned!");
        return;
    }

    if (childSpriteRenderer == null)
    {
        Debug.LogError("Child SpriteRenderer is missing!");
        return;
    }

    // Access the material of the SpriteRenderer's child
    Material material = childSpriteRenderer.material;

    if (material == null)
    {
        Debug.LogError("Material is missing on the child SpriteRenderer!");
        return;
    }

    // Change the material color based on SynthType
    switch (SynthAbilitySlot.synthType)
    {
        case ESynthType.Bass:
            material.SetColor("_Color", bassColor);
            break;
        case ESynthType.Lead:
            material.SetColor("_Color", leadColor);
            break;
        case ESynthType.Pad:
            material.SetColor("_Color", padColor);
            break;
        case ESynthType.Effects:
            material.SetColor("_Color", effectsColor);
            break;
        default:
            Debug.LogError("Unknown SynthType!");
            break;
    }
    animator.SetTrigger("Place");
    Debug.Log($"SynthAbility set to {SynthAbilitySlot.synthType} with updated material color.");
}


    public void TriggerBeat()
    {
        OnBeat?.Invoke();
        animator.SetTrigger("Beat");
        if(SynthAbilitySlot && SynthAbilitySlot.owner !=null){
            SynthAbilitySlot.TryAbility();
        }
    }

    public void ClearCharacter()
    {
        // Clear the CharacterSlot and destroy the object if it exists
        if (CharacterSlot != null)
        {
            CharacterSlot = null;
        }


    }
    public void clearSynth(){
                // Clear the SynthAbilitySlot and destroy the object if it exists
        Material material = childSpriteRenderer.material;
        material.SetColor("_Color", OriginalColor);
        if (SynthAbilitySlot != null)
        {
            // Destroy(SynthAbilitySlot);
            SynthAbilitySlot = null;
        }
    }
    void Update()
    {
        if (CharacterSlot != null)
        {
            // Ensure the object is at the panel's position
            // CharacterSlot.transform.position = transform.position;
        }
    }
}
