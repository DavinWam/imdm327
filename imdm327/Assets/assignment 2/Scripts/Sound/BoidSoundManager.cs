using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSoundManager : MonoBehaviour
{
    public FMSynth wholeNoteSynth;  // Public synth for whole notes
    public FMSynth halfNoteSynth;   // Public synth for half notes
    public FMSynth quarterNoteSynth; // Public synth for quarter notes
    public FMSynth eighthNoteSynth;  // Public synth for eighth notes
    public float halfNoteThreshold = 1.0f;    // Threshold for turning off half notes
    public float quarterNoteThreshold = 2.0f; // Threshold for turning off quarter notes
    public float eighthNoteThreshold = 3.0f;  // Threshold for turning off eighth notes
    public int bpm = 120; // Beats per minute (set the BPM)

    private BoidSpawner spawner;

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

    void Start()
    {
        // Get the BoidSpawner component from the same GameObject
        spawner = GetComponent<BoidSpawner>();
        if (spawner == null)
        {
            Debug.LogError("BoidSoundManager requires a BoidSpawner component on the same GameObject.");
            return;
        }

        // Start the main coroutine to handle all note types
        if (wholeNoteSynth != null && halfNoteSynth != null && quarterNoteSynth != null && eighthNoteSynth != null)
        {
            StartCoroutine(SynthNoteCycle());
        }
        else
        {
            Debug.LogError("One or more synths are not assigned.");
        }
    }
    public int GetActiveSynthCount()
    {
        int activeSynthCount = 0;

        // Check whole note synth: both isActive and noteOn must be true
        if (wholeNoteSynth != null && wholeNoteSynth.isActive && wholeNoteSynth.noteOn)
            activeSynthCount++;

        // Check half note synth: both isActive and noteOn must be true
        if (halfNoteSynth != null && halfNoteSynth.isActive && halfNoteSynth.noteOn)
            activeSynthCount++;

        // Check quarter note synth: both isActive and noteOn must be true
        if (quarterNoteSynth != null && quarterNoteSynth.isActive && quarterNoteSynth.noteOn)
            activeSynthCount++;

        // Check eighth note synth: both isActive and noteOn must be true
        if (eighthNoteSynth != null && eighthNoteSynth.isActive && eighthNoteSynth.noteOn)
            activeSynthCount++;

        // Ensure at least 1 active synth is counted to avoid division by 0
        return Mathf.Max(1, activeSynthCount);
    }

    // Calculate the duration of an 8th note based on the BPM
    float CalculateEighthNoteDuration()
    {
        return 60f / (bpm * 2); // Duration of an eighth note in seconds
    }

    // Combined synth note cycle for whole, half, quarter, and eighth notes
    IEnumerator SynthNoteCycle()
    {
        float eighthNoteDuration = CalculateEighthNoteDuration();
        int eighthNoteCounter = 0; // Counter to track when to play different note types

        while (true)
        {
            // Eighth note: Always play on each cycle
            eighthNoteSynth.operators[0].baseFrequency = eighthNoteFrequencies[currentChordIndex][eighthNoteCounter % 8];
            eighthNoteSynth.NoteOn();

            // Quarter note: Play every second 8th note
            if (eighthNoteCounter % 2 == 0)
            {
                quarterNoteSynth.operators[0].baseFrequency = quarterNoteFrequencies[currentChordIndex][(eighthNoteCounter / 2) % 4];
                quarterNoteSynth.NoteOn();
            }

            // Half note: Play every fourth 8th note
            if (eighthNoteCounter % 4 == 0)
            {
                halfNoteSynth.operators[0].baseFrequency = triadFrequencies[currentChordIndex][(eighthNoteCounter / 4) % 2];
                halfNoteSynth.NoteOn();
            }

            // Whole note: Play every eighth 8th note
            if (eighthNoteCounter % 8 == 0)
            {
                wholeNoteSynth.operators[0].baseFrequency = ebMajorFrequencies[currentChordIndex];
                wholeNoteSynth.NoteOn();
            }

            // Wait for the duration of an eighth note
            yield return new WaitForSeconds(eighthNoteDuration);

            // Turn off the eighth note after its duration
            eighthNoteSynth.NoteOff();

            // Turn off quarter note if we're between beats
            if (eighthNoteCounter % 2 == 1)
            {
                quarterNoteSynth.NoteOff();
            }

            // Turn off half note if we're halfway through the full beat cycle
            if (eighthNoteCounter % 4 == 3)
            {
                halfNoteSynth.NoteOff();
            }

            // Turn off whole note at the end of the full 8th note cycle
            if (eighthNoteCounter % 8 == 7)
            {
                wholeNoteSynth.NoteOff();

                // Move to the next chord in the progression
                currentChordIndex = (currentChordIndex + 1) % ebMajorFrequencies.Length;
            }

            // Increment the 8th note counter
            eighthNoteCounter++;
        }
    }

    float separationWeight = 1.0f;
    float velocityWeight = 0.5f;
    float alignmentWeight = 0.75f;
    float accelerationWeight = 0.6f;
    void Update()
    {
        if (spawner == null) return;

        // Calculate current tension based on the boid simulation metrics
        float tension = CalculateTension();
        Debug.Log("t: "+ tension);
        // Dynamically set the synth volume or mark them active/inactive based on tension
        SetSynthActivityBasedOnTension(tension);
    }

    // Dynamically adjust synth activity based on tension
    void SetSynthActivityBasedOnTension(float tension)
    {
        // Whole note synth is always active
        if (wholeNoteSynth != null)
        {
            wholeNoteSynth.isActive = true;
        }

        // Activate or deactivate half note synth based on tension
        if (halfNoteSynth != null)
        {
            halfNoteSynth.isActive = tension >= halfNoteThreshold;
        }

        // Activate or deactivate quarter note synth based on tension
        if (quarterNoteSynth != null)
        {
            quarterNoteSynth.isActive = tension >= quarterNoteThreshold;
        }

        // Activate or deactivate eighth note synth based on tension
        if (eighthNoteSynth != null)
        {
            eighthNoteSynth.isActive = tension >= eighthNoteThreshold;
        }
    }
    float CalculateTension()
    {
        float averageSeparation = CalculateAverageSeparation(); // Higher separation increases tension
        float averageVelocity = CalculateAverageVelocity(); // Higher velocity contributes to tension
        float alignment = CalculateAlignment(); // Low alignment increases tension
        float acceleration = CalculateAverageAcceleration(); // Rapid changes contribute to tension

        // Combine these metrics into a single tension value
        // You can weight these values depending on how important each factor is
        float tension =
            (averageSeparation * separationWeight) +
            (averageVelocity * velocityWeight) +
            ((1f - alignment) * alignmentWeight) +  // Less alignment adds to tension
            (acceleration * accelerationWeight);

        return tension;
    }
    float CalculateAlignment()
    {
        Vector3 averageDirection = Vector3.zero;
        foreach (var boid in spawner.Boids)
        {
            averageDirection += boid.Velocity.normalized; // Sum normalized velocities
        }

        if (spawner.Boids.Count > 0)
        {
            averageDirection /= spawner.Boids.Count;
        }

        // Return the magnitude of the average direction vector as a measure of alignment
        return averageDirection.magnitude;
    }

    float CalculateAverageAcceleration()
    {
        float totalAcceleration = 0f;
        foreach (var boid in spawner.Boids)
        {
            totalAcceleration += (boid.Velocity - boid.PreviousVelocity).magnitude;
        }

        return totalAcceleration / spawner.Boids.Count;
    }

    // Calculates the average separation distance between all pairs of boids
    float CalculateAverageSeparation()
    {
        float totalSeparation = 0f;
        int pairCount = 0;
        int boidCount = spawner.Boids.Count;

        for (int i = 0; i < boidCount; i++)
        {
            for (int j = i + 1; j < boidCount; j++)
            {
                float distance = Vector3.Distance(spawner.Boids[i].transform.position,
                                                  spawner.Boids[j].transform.position);
                totalSeparation += distance;
                pairCount++;
            }
        }

        return pairCount > 0 ? totalSeparation / pairCount : 0f;
    }

    // Calculates the average velocity (speed) of all boids
    float CalculateAverageVelocity()
    {
        float totalSpeed = 0f;
        int boidCount = spawner.Boids.Count;

        foreach (var boid in spawner.Boids)
        {
            totalSpeed += boid.Velocity.magnitude;
        }

        return boidCount > 0 ? totalSpeed / boidCount : 0f;
    }
}