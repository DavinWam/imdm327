using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Separate class for frequency modulation synthesis logic
public class FrequencyModulationSynthesizer
{
    // Private variable to keep track of the time index
    private int timeIndex = 0;

    // Function to process audio and apply synthesis
    public void OnAudioFilterRead(float[] data, int channels, float frequency, float volume)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            // Generate sine wave
            data[i] = CreateSine(timeIndex, frequency, 44100, volume);

            // Clone the sine wave to the second audio channel for stereo output
            if (channels == 2)
                data[i + 1] = data[i];

            // Increment the time index for the next sample
            timeIndex++;
        }
    }

    // Function to create a sine wave with the specified parameters
    public float CreateSine(int timeIndex, float frequency, float sampleRate, float volume)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate) * volume;
    }
}