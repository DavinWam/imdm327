using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewOperator", menuName = "Sound/Operator")]
public class Operator : ScriptableObject
{
    [Range(1, 5000)]
    public float baseFrequency; // Base frequency of the operator
    public float frequencyMultiplier; // Frequency multiplier
    [Range(0, 1)]
    public float volume; // Volume/Amplitude
    public EWaveform waveformType; // Type of waveform
    public bool quantizeToScale; // Whether to quantize the frequency
    private ENoteSharps currentNote; // Current note after quantization
    public Modulator modulator;
    public float modulationDepth = 80f;
    public List<WavePair> waveGenerators { get; private set; }
    public WaveGenerator mainWaveGenerator;
    public WaveGenerator lfoWaveGenerator; // LFO wave generator
    public bool UseLFO = false;
    [Range(0.1f, 20f)]
    public float lfoFrequency = 5f; // Default LFO frequency in Hz

    public ADSREnvelope envelope = ADSREnvelope.Default(); // Moved envelope here

    // Custom LFO curve for 'custom' waveform type (used only by the LFO)
    public AnimationCurve customLfoWaveformCurve;

    private void Awake()
    {
        if (modulator)
        {
            modulator.baseFrequency = baseFrequency;
        }
        if (waveGenerators == null)
        {
            waveGenerators = new List<WavePair>();
        }
    }

    private void OnValidate()
    {
        if (mainWaveGenerator == null)
        {
            mainWaveGenerator = new WaveGenerator();
        }
        if (waveGenerators == null)
        {
            waveGenerators = new List<WavePair>();
        }
        if (lfoWaveGenerator == null)
        {
            lfoWaveGenerator = new WaveGenerator(customLfoWaveformCurve);
        }
    }

    public WaveGenerator AddWave(float frequency)
    {
        WaveGenerator waveGenerator = new WaveGenerator();
        waveGenerators.Add(new WavePair(waveGenerator, frequency, envelope));
        return waveGenerator;
    }

    public void RemoveWave(WaveGenerator waveGenerator)
    {
        waveGenerators.RemoveAll(wavePair => wavePair.waveGenerator == waveGenerator);
    }

    public float GetFrequencySampleValue(bool noteOn, float deltaTime)
    {
        if ((waveGenerators == null || waveGenerators.Count == 0) && mainWaveGenerator == null)
        {
            return 0f;
        }

        float sampleValue = 0f;
        float lfoModulation = 1f;
        float modulatedFrequency = baseFrequency;

        if (quantizeToScale)
        {
            modulatedFrequency = GetQuantizedFrequency();
        }

        if (modulator != null)
        {
            float modulatorValue = modulator.GetFrequencySampleValue(noteOn,deltaTime);
            modulatedFrequency += modulatorValue * modulationDepth;
        }

        if (lfoWaveGenerator != null && UseLFO)
        {
            lfoModulation = lfoWaveGenerator.GenerateSample(lfoFrequency, 1f, EWaveform.Custom);
        }

        modulatedFrequency *= lfoModulation;

        // Update envelope and retrieve current amplitude
        float currentAmplitude = envelope.Update(noteOn, deltaTime);

        sampleValue += mainWaveGenerator.GenerateSample(modulatedFrequency, volume * currentAmplitude, waveformType);
        foreach (WavePair wavePair in waveGenerators)
        {
            sampleValue += wavePair.waveGenerator.GenerateSample(wavePair.frequency, volume * currentAmplitude, waveformType);
        }

        return sampleValue;
    }

    public float GetQuantizedFrequency()
    {
        if (!quantizeToScale)
        {
            currentNote = ENoteSharps.None;
            return baseFrequency;
        }

        float semitonesFromA4 = 12f * Mathf.Log(baseFrequency * frequencyMultiplier / 440f, 2f);
        int nearestSemitone = Mathf.RoundToInt(semitonesFromA4);
        float quantizedFrequency = 440f * Mathf.Pow(2f, nearestSemitone / 12f);
        int noteIndex = (nearestSemitone % 12 + 12) % 12;
        currentNote = TwelveToneScaleNotes[noteIndex];

        return quantizedFrequency;
    }

    public static readonly ENoteSharps[] TwelveToneScaleNotes = {
        ENoteSharps.A, ENoteSharps.ASharp, ENoteSharps.B, ENoteSharps.C, ENoteSharps.CSharp,
        ENoteSharps.D, ENoteSharps.DSharp, ENoteSharps.E, ENoteSharps.F, ENoteSharps.FSharp,
        ENoteSharps.G, ENoteSharps.GSharp
    };
}

[System.Serializable]
public struct WavePair
{
    public WaveGenerator waveGenerator;
    public float frequency;
    public ADSREnvelope envelope;    
    public WavePair(WaveGenerator waveGenerator, float frequency, ADSREnvelope mainEnvelope)
    {
        this.waveGenerator = waveGenerator;
        this.frequency = frequency;

        // Initialize the envelope with the main envelope settings
        this.envelope = mainEnvelope;
        envelope.attackTime = mainEnvelope.attackTime;
        envelope.decayTime = mainEnvelope.decayTime;
        envelope.sustainLevel = mainEnvelope.sustainLevel;
        envelope.releaseTime = mainEnvelope.releaseTime;
        envelope.canInterrupt = mainEnvelope.canInterrupt;
    }
}
