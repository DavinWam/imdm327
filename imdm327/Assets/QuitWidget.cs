using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitWidget : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
        #else
            Application.Quit(); // Close the game in a built application
        #endif
    }
}
