using UnityEngine;

public class FrequencyModulationSynthesizer
{
    private double phase;
    private double sampleRate;

    public FrequencyModulationSynthesizer()
    {
        sampleRate = AudioSettings.outputSampleRate;
        phase = 0;
    }

    public void ResetPhase()
    {
        phase = 0;
    }

    // Generate the next sample based on the waveform type
    private float GenerateWaveSample(Waveform waveformType, double frequency)
    {
        double increment = frequency * 2.0 * Mathf.PI / sampleRate;
        phase += increment;

        switch (waveformType)
        {
            case Waveform.Sine:
                return Mathf.Sin((float)phase);

            case Waveform.Square:
                return Mathf.Sign(Mathf.Sin((float)phase));

            case Waveform.Sawtooth:
                return (float)((phase / Mathf.PI) % 2.0 - 1.0);

            case Waveform.Triangle:
                return Mathf.PingPong((float)(phase / Mathf.PI), 2.0f) - 1.0f;

            default:
                return 0;
        }
    }

    public void OnAudioFilterRead(float[] data, int channels, float frequency, float amplitude, Waveform waveformType)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = GenerateWaveSample(waveformType, frequency) * amplitude;

            for (int channel = 0; channel < channels; channel++)
            {
                data[i + channel] = sample;
            }
        }
    }
}
