using UnityEngine;

public class WaveGenerator
{
    private double phase;
    private double sampleRate;

    public WaveGenerator()
    {
        sampleRate = AudioSettings.outputSampleRate;
        phase = 0;
    }

    public void ResetPhase()
    {
        phase = 0;
    }

    // Generate the next sample with modulation
    public float GenerateSample(double frequency, float amplitude, EWaveform waveformType, double modulation)
    {
        double increment = frequency * 2.0 * Mathf.PI / sampleRate;
        phase += increment + modulation * 2.0 * Mathf.PI / sampleRate; // Convert modulation to phase increment

        switch (waveformType)
        {
            case EWaveform.Sine:
                return Mathf.Sin((float)phase) * amplitude;

            case EWaveform.Square:
                return Mathf.Sign(Mathf.Sin((float)phase)) * amplitude;

            case EWaveform.Sawtooth:
                return (float)((phase / Mathf.PI) % 2.0 - 1.0f) * amplitude;

            case EWaveform.Triangle:
                return (Mathf.PingPong((float)(phase / Mathf.PI), 2.0f) - 1.0f) * amplitude;

            default:
                return 0;
        }
    }
}
