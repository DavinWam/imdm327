using UnityEngine;
using System.Collections;
using NAudio.Midi;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SynthRegistration
    {
        public FMSynth synth;
        public bool isActive;

        public SynthRegistration(FMSynth synth, bool isActive)
        {
            this.synth = synth;
            this.isActive = isActive;
        }
    }


    private List<SynthRegistration> registeredSynths = new List<SynthRegistration>();
    public int maxSynthCount = 4; // Max number of synths (whole, half, quarter, eighth notes)

    public FMSynth wholeNoteSynth;  // Public synth for whole notes
    public FMSynth halfNoteSynth;   // Public synth for half notes
    public FMSynth quarterNoteSynth; // Public synth for quarter notes
    public FMSynth eighthNoteSynth;  // Public synth for eighth notes

    public bool play;
    void Awake(){
        // Ensure the synths are properly assigned
        if (wholeNoteSynth == null || halfNoteSynth == null || quarterNoteSynth == null || eighthNoteSynth == null)
        {
            Debug.LogError("One or more synths are not assigned in BoidSoundManager.");
            return;
        }

        // Register the synths
        RegisterSynths();
        SetSynthActive(wholeNoteSynth, true);
    }
    void Start()
    {
        if(play == false) return;
        if (songData == null)
        {
            Debug.LogError("SongData is not assigned in BoidSoundManager.");
            return;
        }
        songData.InitializeNotes();
        songNotes = songData.GetNotes();
        // Start single playback of notes
        StartSinglePlayback();
    }
    void Update()
    {
        // Remove null entries from the registeredSynths list
        registeredSynths.RemoveAll(registration => registration.synth == null);

        // Your other update logic here
    }

     public SongData songData;       // Reference to SongData for notes
    private List<NoteEventInfo> songNotes; // List to hold the notes from SongData
    public void StartSinglePlayback()
    {
        // Iterate over each note and start a coroutine to play it
        foreach (NoteEventInfo note in songNotes)
        {
            StartCoroutine(PlayNoteCoroutine(note));
        }
    }

    IEnumerator PlayNoteCoroutine(NoteEventInfo note)
    {
        // Wait until the start time of the note
        yield return new WaitForSeconds(note.StartTime);

        // Convert the MIDI note number to frequency
        float frequency = NoteNumberToFrequency(note.NoteNumber);

        // Set the frequency and play the note
        wholeNoteSynth.operators[0].baseFrequency = frequency;
        wholeNoteSynth.NoteOn();

        // Calculate the note duration and wait
        // float duration = (note.EndTime - note.StartTime) * (60f / songData.bpm);
        // yield return new WaitForSeconds(duration);

        // Stop the note
        // wholeNoteSynth.NoteOff();

        // Log note playback for debugging
        Debug.Log($"Playing note: {note.NoteNumber}/pitch: {frequency} at {note.StartTime} for {note.length} seconds");
    }

    float NoteNumberToFrequency(int noteNumber)
    {
        // Convert MIDI note number to frequency (using A4 = 440 Hz as reference)
        return 440f * Mathf.Pow(2f, (noteNumber - 69) / 12f);
    }


    void RegisterSynths()
    {
        // Register each synth with an inactive state initially
        RegisterSynth(wholeNoteSynth, false);
        RegisterSynth(halfNoteSynth, false);
        RegisterSynth(quarterNoteSynth, false);
        RegisterSynth(eighthNoteSynth, false);
    }

    public void RegisterSynth(FMSynth synth, bool active)
    {
        // Check if the synth is already registered
        if (!registeredSynths.Any(s => s.synth == synth))
        {
            registeredSynths.Add(new SynthRegistration(synth, active));
        }
    }


    public int GetActiveSynthCount()
    {
        int activeSynthCount = 0;

        // Create a shallow copy of registeredSynths to safely iterate
        var synthsCopy = new List<SynthRegistration>(registeredSynths);

        // Count active synths in the copied list
        foreach (var registration in synthsCopy)
        {
            if (registration.isActive && registration.synth.noteOn)
            {
                activeSynthCount++;
            }
        }

        // Ensure at least 1 active synth is counted to avoid division by 0
        return Mathf.Max(1, activeSynthCount);
    }

    public bool CanSynthBeActive(FMSynth synth)
    {
        // Create a shallow copy to safely iterate
        List<SynthRegistration> synthCopy = new List<SynthRegistration>(registeredSynths);

        foreach (var registration in synthCopy)
        {
            if (registration.synth == synth)
            {
                return registration.isActive;
            }
        }
        return false; // Default to inactive if synth is not registered
    }


    public void SetSynthActive(FMSynth synth, bool isActive)
    {
        // Update the active status for a given synth
        for (int i = 0; i < registeredSynths.Count; i++)
        {
            if (registeredSynths[i].synth == synth)
            {
                registeredSynths[i] = new SynthRegistration(synth, isActive);
                break;
            }
        }
    }
}