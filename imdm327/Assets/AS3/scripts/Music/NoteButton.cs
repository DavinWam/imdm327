using UnityEngine;
using UnityEngine.UI;

public class NoteButton : MonoBehaviour
{
    public int noteNumber;
    public AudioClip noteClip; // Assign your piano.adg.ogg here in the inspector

    public MusicKeyManager keyManager;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

    }

    void OnClick()
    {
        if (keyManager != null)
        {
            GameObject tempAudioSource = new GameObject("TempAudioSource"); // Create a temporary GameObject
            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>(); // Add an AudioSource
            audioSource.clip = noteClip;
            audioSource.pitch = keyManager.GetPitchAdjustment(noteNumber); // Get pitch adjustment from MusicKeyManager
            audioSource.Play();
            Destroy(tempAudioSource, noteClip.length);
        }
    }
}
