using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSoundManager : MonoBehaviour
{
    public FMSynth wholeNoteSynth; // Public synth for whole notes
    public FMSynth halfNoteSynth;  // Public synth for half notes
    public int bpm = 120; // Beats per minute (set the BPM)
    
    private BoidSpawner spawner;

    // E-flat major scale frequencies for the baseline (Ab, Bb, G, C) corresponding to 4, 5, 3, 6
    private float[] ebMajorFrequencies = { 349.23f, 466.16f, 392.00f, 587.33f }; // Ab (4), Bb (5), G (3), C (6)
    
    // Triads for each chord, corresponding to Ab, Bb, G, C
    private float[][] triadFrequencies = new float[][]
    {
        new float[] { 261.63f, 311.13f }, // C and Eb for Ab (4)
        new float[] { 293.66f, 349.23f }, // D and F for Bb (5)
        new float[] { 293.66f, 392.00f }, // D and G for G (3)
        new float[] { 329.63f, 392.00f }  // E and G for C (6)
    };

    void Start()
    {
        // Get the BoidSpawner component from the same GameObject
        spawner = GetComponent<BoidSpawner>();
        if (spawner == null)
        {
            Debug.LogError("BoidSoundManager requires a BoidSpawner component on the same GameObject.");
            return;
        }

        // Start the combined synth note cycle
        if (wholeNoteSynth != null && halfNoteSynth != null)
        {
            StartCoroutine(SynthNoteCycle(CalculateNoteDuration())); // Combined whole and half notes
        }
        else
        {
            Debug.LogError("wholeNoteSynth or halfNoteSynth is not assigned.");
        }
    }

    // Calculate the duration of a whole note based on the BPM (4 beats per whole note)
    float CalculateNoteDuration()
    {
        return 240f / bpm; // Duration of a whole note in seconds (4 beats)
    }

    // Combined synth note cycle for both whole and half notes
    IEnumerator SynthNoteCycle(float duration)
    {
        int currentChordIndex = 0;

        while (true)
        {
            // Whole note: Set the current frequency from the baseline
            wholeNoteSynth.operators[0].baseFrequency = ebMajorFrequencies[currentChordIndex];
            wholeNoteSynth.NoteOn();

            // Half notes: Play each note in the triad (2 notes for half notes)
            for (int noteIndex = 0; noteIndex < 2; noteIndex++)
            {
                halfNoteSynth.operators[0].baseFrequency = triadFrequencies[currentChordIndex][noteIndex];
                halfNoteSynth.NoteOn();

                // Wait for half the duration (half note)
                yield return new WaitForSeconds(duration / 2);

                halfNoteSynth.NoteOff();
            }

            // Turn off the whole note after the full duration
            wholeNoteSynth.NoteOff();

            // Move to the next chord in the 4-5-3-6 progression
            currentChordIndex = (currentChordIndex + 1) % ebMajorFrequencies.Length;
        }
    }

    void Update()
    {
        if (spawner == null || wholeNoteSynth == null || halfNoteSynth == null) return;

        // Calculate simulation metrics
        float averageSeparation = CalculateAverageSeparation();
        float averageVelocity = CalculateAverageVelocity();
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
