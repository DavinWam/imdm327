using UnityEngine;

public class FMSynth : MonoBehaviour
{
    public Operator[] operators; // Array of operators (carriers and modulators)
    public float duration = 2.0f; // Duration in seconds
 
    // Envelope parameters
    public float attackTime = 0.1f;
    public float decayTime = 0.2f;
    [Range(0, 1)]
    public float sustainLevel = 0.7f;
    public float releaseTime = 0.5f;

    public bool playOnStart = true;

    private float envelopeVolume = 0;
    private float timeSinceNoteOn = 0;
    public bool noteOn = false;
    private float noteDurationTimer = 0f;

    private WaveGenerator[] waveGenerators;

    void Start()
    {
        // Initialize the array of wave generators
        waveGenerators = new WaveGenerator[operators.Length];
        for (int i = 0; i < operators.Length; i++)
        {
            waveGenerators[i] = new WaveGenerator();
        }

        if (playOnStart)
        {
            NoteOn();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NoteOn();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            NoteOff();
        }

        ProcessEnvelope(Time.deltaTime);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        int sampleCount = data.Length / channels;

        // Arrays to keep track of operator outputs
        float[] operatorOutputs = new float[operators.Length];

        // Initialize data array
        System.Array.Clear(data, 0, data.Length);
 
        // For each sample
        for (int sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
        {
            // Process each operator
            for (int opIndex = 0; opIndex < operators.Length; opIndex++)
            {
                // Use ref to get a reference to the original Operator
                ref Operator op = ref operators[opIndex];
                WaveGenerator generator = waveGenerators[opIndex];
                float modulationDepth = op.modulationDepth;

                // Get base frequency and apply frequency multiplier
                float baseFreq = op.GetQuantizedFrequency();
                double frequency = baseFreq * op.frequencyMultiplier;

                // Calculate modulation from modulator
                double modulation = 0;

                // Get modulation from the assigned modulator
                if (op.modulatorIndex >= 0 && op.modulatorIndex < operators.Length)
                {
                    modulation = operatorOutputs[op.modulatorIndex] * modulationDepth;
                }

                // Generate sample with modulation
                float amplitude = op.volume * envelopeVolume;
                float sample = generator.GenerateSample(frequency, amplitude, op.waveformType, modulation);

                // Store output for modulators to use
                operatorOutputs[opIndex] = sample;

                // If this operator is a carrier, add its output to the data buffer
                if (op.isCarrier)
                {
                    float outputSample = sample;

                    for (int channel = 0; channel < channels; channel++)
                    {
                        data[sampleIndex * channels + channel] += outputSample;
                    }
                }
            }
        }

        // Prevent clipping
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Mathf.Clamp(data[i], -1f, 1f);
        }
    }

    void ProcessEnvelope(float deltaTime)
    {
        if (noteOn)
        {
            timeSinceNoteOn += deltaTime;
            noteDurationTimer -= deltaTime;

            if (noteDurationTimer <= 0)
            {
                NoteOff();
            }

            if (timeSinceNoteOn <= attackTime)
            {
                envelopeVolume = Mathf.Lerp(0, 1, timeSinceNoteOn / attackTime);
            }
            else if (timeSinceNoteOn <= attackTime + decayTime)
            {
                float decayProgress = (timeSinceNoteOn - attackTime) / decayTime;
                envelopeVolume = Mathf.Lerp(1, sustainLevel, decayProgress);
            }
            else
            {
                envelopeVolume = sustainLevel;
            }
        }
        else
        {
            envelopeVolume -= deltaTime / releaseTime;
            if (envelopeVolume < 0)
            {
                envelopeVolume = 0;
            }
        }
    }


    public void NoteOff()
    {
        noteOn = false;
    }

    public void NoteOn()
    {
        if (noteOn)
        {
            NoteOff();
        }
        noteOn = true;
        timeSinceNoteOn = 0;
        noteDurationTimer = duration; // Reset the duration timer

        // Reset phases
        foreach (var generator in waveGenerators)
        {
            generator.ResetPhase();
        }
    }

}
