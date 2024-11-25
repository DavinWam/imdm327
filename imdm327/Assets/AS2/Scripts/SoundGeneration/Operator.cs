using UnityEngine;
using System.Collections.Generic;
using System.Net;

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
    public List<WavePair> noteWaveGenerators { get; private set; }
    public WaveGenerator mainWaveGenerator;
    public WaveGenerator lfoWaveGenerator; // LFO wave generator
    public bool UseLFO = false;
    [Range(0.1f, 20f)]
    public float lfoFrequency = 5f; // Default LFO frequency in Hz
        // Custom LFO curve for 'custom' waveform type (used only by the LFO)
    public AnimationCurve customLfoWaveformCurve;
    public ADSREnvelope envelope = ADSREnvelope.Default(); // Moved envelope here




    private void Awake()
    {
        if (modulator)
        {
            modulator.baseFrequency = baseFrequency;
        }
        if (noteWaveGenerators == null)
        {
            noteWaveGenerators = new List<WavePair>();
        }
        // Debug.Log($"{this.name} awake");
        OnValidate();
    }
    
    private void OnValidate()
    {
        if (mainWaveGenerator == null)
        {
            mainWaveGenerator = new WaveGenerator();
            //  Debug.Log($"{this.name} main");
        }
        if (noteWaveGenerators == null)
        {
            noteWaveGenerators = new List<WavePair>();
        }
        if (lfoWaveGenerator == null)
        {
            lfoWaveGenerator = new WaveGenerator(customLfoWaveformCurve);
        }
    }

    public WavePair AddWave(float frequency)
    {
        WavePair wp = new WavePair( frequency, envelope, noteWaveGenerators.Count+1);
        noteWaveGenerators.Add(wp);
        return wp;
    }

    public void RemoveWave(WavePair wavePair)
    {
        noteWaveGenerators.Remove(wavePair);
        WavePair foundWavePair = noteWaveGenerators.Find(wp => ReferenceEquals(wp, wavePair));
        foundWavePair.Remove();
        //noteWaveGenerators.Remove(wavePair);
    }
    public void RemoveAllWaves()
    {
        foreach(WavePair wp in noteWaveGenerators){
            wp.Remove();
        }
    }


    public float GetFrequencySampleValue(bool noteOn, float deltaTime)
    {
        if ((noteWaveGenerators == null || noteWaveGenerators.Count == 0) && mainWaveGenerator == null)
        {
            return 0f;
        }

        float sampleValue = 0f;
        float lfoModulation = 1f;
        float modulatedFrequency = baseFrequency;
            // Update envelope and retrieve current amplitude
        float currentAmplitude = envelope.Update(noteOn, deltaTime);

        if(modulatedFrequency > 0){
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
            sampleValue += mainWaveGenerator.GenerateSample(modulatedFrequency, volume * currentAmplitude, waveformType);
        }
        
        noteWaveGenerators.RemoveAll(wavePair => wavePair.toRemove);
        //wavePair.envelope.Update(noteOn,deltaTime)
        // foreach (WavePair wavePair in noteWaveGenerators)
        // {
        //     float noteAmplitude = wavePair.envelope.Update(noteOn, deltaTime);
        //     sampleValue += wavePair.waveGenerator.GenerateSample(wavePair.frequency, volume * currentAmplitude , waveformType);
        // }
        
        for (int i = noteWaveGenerators.Count - 1; i >= 0; i--)
        {
            WavePair wavePair = noteWaveGenerators[i];
            float noteAmplitude = wavePair.EnvelopeUpdate(wavePair.NoteOn, deltaTime);
            sampleValue += wavePair.waveGenerator.GenerateSample(wavePair.frequency, volume * noteAmplitude, waveformType);
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
    public bool NoteOn;
    public bool toRemove;
    public WavePair(float frequency, ADSREnvelope mainEnvelope, float id)
    {
        this.waveGenerator = new WaveGenerator();
        this.frequency = frequency; 
        this.NoteOn = true;
        this.toRemove = false; 

        // Initialize the envelope with the main envelope settings
        this.envelope = ADSREnvelope.Default();
        envelope.id = id;
        envelope.attackTime = mainEnvelope.attackTime;
        envelope.decayTime = mainEnvelope.decayTime;
        envelope.sustainLevel = mainEnvelope.sustainLevel;
        envelope.releaseTime = mainEnvelope.releaseTime;
        envelope.canInterrupt = true;

        // this.envelope.Update(NoteOn, 0);
    }
    public void Remove(){
        toRemove = true;
        NoteOn = false;
    }
    public float EnvelopeUpdate(bool on, float deltaTime){
        return envelope.Update(on, deltaTime);
    }
}
