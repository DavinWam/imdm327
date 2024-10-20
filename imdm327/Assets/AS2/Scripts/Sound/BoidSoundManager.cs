using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSoundManager : MonoBehaviour
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
    public float halfNoteThreshold = 1.0f;    // Threshold for turning off half notes
    public float quarterNoteThreshold = 2.0f; // Threshold for turning off quarter notes
    public float eighthNoteThreshold = 3.0f;  // Threshold for turning off eighth notes
    public int bpm = 120; // Beats per minute (set the BPM)


    // E-flat major scale frequencies for the baseline (Ab, Bb, G, C) corresponding to 4, 5, 3, 6
    private float[] ebMajorFrequencies = {
        415.30f,  // A♭4
        466.16f,  // B♭4
        392.00f,  // G4 
        523.25f   // C5
    };

    // Triads for each chord, corresponding to Ab, Bb, G, C
    private float[][] triadFrequencies = new float[][]
    {
        new float[] { 261.63f, 311.13f }, // C and Eb for Ab (4)
        new float[] { 293.66f, 349.23f }, // D and F for Bb (5)
        new float[] { 466.16f, 293.66f }, // Bb and D for G (3)
        new float[] { 329.63f, 392.00f }  // E and G for C (6)
    };

    // 4th note patterns for each chord
    private float[][] quarterNoteFrequencies = new float[][]
    {
        new float[] { 415.30f, 311.13f, 830.61f, 311.13f }, // Ab, Eb, Ab (octave), Eb for Ab
        new float[] { 466.16f, 349.23f, 932.32f, 349.23f }, // Bb, F, Bb (octave), F for Bb
        new float[] { 392.00f, 293.66f, 784.00f, 293.66f }, // G, D, G (octave), D for G
        new float[] { 261.63f, 392.00f, 523.25f, 392.00f }  // C, G, C (octave), G for C
    };

    // 8th note patterns for each chord (repeated twice)
    private float[][] eighthNoteFrequencies = new float[][]
    {
        new float[] { 261.63f, 311.13f, 349.23f, 311.13f, 261.63f, 311.13f, 349.23f, 311.13f }, // C, Eb, F, Eb twice for Ab
        new float[] { 293.66f, 349.23f, 392.00f, 349.23f, 293.66f, 349.23f, 392.00f, 349.23f }, // D, F, G, F twice for Bb
        new float[] { 466.16f, 293.66f, 311.13f, 293.66f, 466.16f, 293.66f, 311.13f, 293.66f }, // Bb, D, Eb, D twice for G
        new float[] { 329.63f, 392.00f, 440.00f, 392.00f, 329.63f, 392.00f, 440.00f, 392.00f }  // E, G, A, G twice for C
    };

    // Shared chord index
    private int currentChordIndex = 0;
    private BoidSoundSimulation simulation; // Use the new simulation class
    private BoidSpawner spawner;

    void Start()
    {
        // Get the BoidSpawner component from the same GameObject
        spawner = GetComponent<BoidSpawner>();
        if (spawner == null)
        {
            Debug.LogError("BoidSoundManager requires a BoidSpawner component on the same GameObject.");
            return;
        }

        // Initialize the BoidSoundSimulation
        simulation = new BoidSoundSimulation(spawner);

        // Ensure the synths are properly assigned
        if (wholeNoteSynth == null || halfNoteSynth == null || quarterNoteSynth == null || eighthNoteSynth == null)
        {
            Debug.LogError("One or more synths are not assigned in BoidSoundManager.");
            return;
        }

        // Register the synths
        RegisterSynths();

        // Start the main coroutine to handle all note types
        StartCoroutine(SynthNoteCycle());
    }

    void RegisterSynths()
    {
        // Register each synth with an inactive state initially
        registeredSynths.Add(new SynthRegistration(wholeNoteSynth, false));
        registeredSynths.Add(new SynthRegistration(halfNoteSynth, false));
        registeredSynths.Add(new SynthRegistration(quarterNoteSynth, false));
        registeredSynths.Add(new SynthRegistration(eighthNoteSynth, false));
    }

    public int GetActiveSynthCount()
    {
        int activeSynthCount = 0;

        // Count active synths in the registered list
        foreach (var registration in registeredSynths)
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
        // Find the synth registration and return its active status
        foreach (var registration in registeredSynths)
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

    void Update()
    {
        if (spawner == null) return;

        float tension = simulation.CalculateTension();
        Debug.Log("Tension: " + tension);
        SetSynthActivityBasedOnTension(tension);
    }

    // Dynamically adjust synth activity based on tension
    void SetSynthActivityBasedOnTension(float tension)
    {
        // Whole note synth is always active
        if (wholeNoteSynth != null)
        {
            SetSynthActive(wholeNoteSynth, true);
        }

        // Activate or deactivate half note synth based on tension
        if (halfNoteSynth != null)
        {
            bool isActive = tension >= halfNoteThreshold;
            SetSynthActive(halfNoteSynth, isActive);
        }

        // Activate or deactivate quarter note synth based on tension
        if (quarterNoteSynth != null)
        {
            bool isActive = tension >= quarterNoteThreshold;
            SetSynthActive(quarterNoteSynth, isActive);
        }

        // Activate or deactivate eighth note synth based on tension
        if (eighthNoteSynth != null)
        {
            bool isActive = tension >= eighthNoteThreshold;
            SetSynthActive(eighthNoteSynth, isActive);
        }
    }


    float CalculateEighthNoteDuration()
    {
        return 60f / (bpm * 2); // Duration of an eighth note in seconds
    }

    IEnumerator SynthNoteCycle()
    {
        float eighthNoteDuration = CalculateEighthNoteDuration();
        int eighthNoteCounter = 0;

        while (true)
        {
            eighthNoteSynth.operators[0].baseFrequency = eighthNoteFrequencies[currentChordIndex][eighthNoteCounter % 8];
            eighthNoteSynth.NoteOn();

            if (eighthNoteCounter % 2 == 0)
            {
                quarterNoteSynth.operators[0].baseFrequency = quarterNoteFrequencies[currentChordIndex][(eighthNoteCounter / 2) % 4];
                quarterNoteSynth.NoteOn();
            }

            if (eighthNoteCounter % 4 == 0)
            {
                halfNoteSynth.operators[0].baseFrequency = triadFrequencies[currentChordIndex][(eighthNoteCounter / 4) % 2];
                halfNoteSynth.NoteOn();
            }

            if (eighthNoteCounter % 8 == 0)
            {
                wholeNoteSynth.operators[0].baseFrequency = ebMajorFrequencies[currentChordIndex];
                wholeNoteSynth.NoteOn();
            }

            yield return new WaitForSeconds(eighthNoteDuration);

            eighthNoteSynth.NoteOff();

            if (eighthNoteCounter % 2 == 1)
            {
                quarterNoteSynth.NoteOff();
            }

            if (eighthNoteCounter % 4 == 3)
            {
                halfNoteSynth.NoteOff();
            }

            if (eighthNoteCounter % 8 == 7)
            {
                wholeNoteSynth.NoteOff();
                currentChordIndex = (currentChordIndex + 1) % ebMajorFrequencies.Length;
            }

            eighthNoteCounter++;
        }
    }


}