using UnityEngine;

public class WaveGenerator
{
    public double phase { get; private set; }
    public double sampleRate { get; private set; }

    // AnimationCurve for custom waveform
    private AnimationCurve customWaveformCurve;

    public WaveGenerator(AnimationCurve customWaveformCurve = null)
    {
        sampleRate = AudioSettings.outputSampleRate;
        phase = 0;
        this.customWaveformCurve = customWaveformCurve;
    }

    public void ResetPhase()
    {
        phase = 0;
    }

    // Generate the next sample with modulation
    public float GenerateSample(double frequency, float amplitude, EWaveform waveformType)
    {
        // Increment phase using the frequency
        double increment = frequency / sampleRate;  // No 2 * PI here
        phase += increment;  // Apply frequency as phase increment

        // Wrap phase to prevent it from growing indefinitely
        if (phase >= 1.0)  // Phase should be wrapped between 0 and 1 (normalized)
        {
            phase -= 1.0;
        }

        // Generate waveform based on the waveform type
        switch (waveformType)
        {
            case EWaveform.Sine:
                return Mathf.Sin((float)(phase * 2.0 * Mathf.PI)) * amplitude;  // Apply 2 * PI here

            case EWaveform.Square:
                return Mathf.Sign(Mathf.Sin((float)(phase * 2.0 * Mathf.PI))) * amplitude;

            case EWaveform.Sawtooth:
                return (float)(2.0 * phase - 1.0) * amplitude;

            case EWaveform.Triangle:
                return (Mathf.PingPong((float)(phase * 2.0), 2.0f) - 1.0f) * amplitude;

            case EWaveform.Custom:
                // Ensure the custom curve is not null
                if (customWaveformCurve != null)
                {
                    // Evaluate the custom waveform based on phase (between 0 and 1)
                    return Mathf.Clamp(customWaveformCurve.Evaluate((float)phase) * amplitude,0,1);
                }
                else
                {
                    // Return silence if no curve is provided
                    return 0;
                }

            default:
                return 0;
        }
    }
}
