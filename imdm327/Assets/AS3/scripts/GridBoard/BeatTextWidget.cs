using UnityEngine;
using TMPro;

public class BeatTextWidget : MonoBehaviour
{
    // Reference to the TMP_Text component
    public TMP_Text beatText;

    // Method to update the beat text
    public void UpdateBeatText(int currentBeat)
    {
        if (beatText != null)
        {
            beatText.text = $"{currentBeat}/8";
        }
        else
        {
            Debug.LogWarning("BeatTextWidget: beatText reference is not assigned.");
        }
    }
}
