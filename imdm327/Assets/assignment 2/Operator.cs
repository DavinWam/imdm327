using UnityEngine;

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
    private ENote currentNote; // Current note after quantization
    public Modulator modulator;
    public float modulationDepth = 80f;
    public WaveGenerator[] waveGenerators; // Main wave generators for the operator
    public WaveGenerator lfoWaveGenerator; // LFO wave generator
    public bool ChorusEffect;
    public bool UseLFO = false;
    [Range(0.1f, 20f)]
    public float lfoFrequency = 5f; // Default LFO frequency in Hz

    // Custom LFO curve for 'custom' waveform type (used only by the LFO)
    public AnimationCurve customLfoWaveformCurve;

    public void Awake()
    {
        if (modulator)
        {
            modulator.baseFrequency = baseFrequency;
        }
    }

    private void OnValidate()
    {
        // Initialize or adjust wave generators based on chorus effect
        if (waveGenerators == null || waveGenerators.Length == 0)
        {
            int numWaves = ChorusEffect ? 2 : 1;
            waveGenerators = new WaveGenerator[numWaves];
            for (int i = 0; i < numWaves; i++)
            {
                waveGenerators[i] = new WaveGenerator(); // No custom waveform needed for main generators
            }
        }

        // Initialize the LFO wave generator and assign the custom curve if needed
        if (lfoWaveGenerator == null)
        {
            lfoWaveGenerator = new WaveGenerator(customLfoWaveformCurve); // LFO uses custom waveform curve
        }
    }

    // Quantizes the input frequency to the closest note on the selected scale
    public virtual float GetQuantizedFrequency()
    {
        if (!quantizeToScale)
        {
            currentNote = ENote.None;
            return baseFrequency;
        }

        // Calculate the number of semitones from A4 (440 Hz)
        float semitonesFromA4 = 12f * Mathf.Log(baseFrequency * frequencyMultiplier / 440f, 2f);
        int nearestSemitone = Mathf.RoundToInt(semitonesFromA4);

        // Calculate the quantized frequency
        float quantizedFrequency = 440f * Mathf.Pow(2f, nearestSemitone / 12f);

        // Corrected note index calculation
        int noteIndex = (nearestSemitone % 12 + 12) % 12;
        currentNote = TwelveToneScaleNotes[noteIndex];

        return quantizedFrequency;
    }

    public static readonly ENote[] TwelveToneScaleNotes = {
        ENote.A, ENote.ASharp, ENote.B, ENote.C, ENote.CSharp,
        ENote.D, ENote.DSharp, ENote.E, ENote.F, ENote.FSharp,
        ENote.G, ENote.GSharp
    };


    // Function to get the frequency sample value for the operator
    public float GetFrequencySampleValue()
    {
        if (waveGenerators == null || waveGenerators.Length == 0)
        {
            return 0f;
        }

        float sampleValue = 0f;
        float lfoModulation = 1f;
        float modulatedFrequency = baseFrequency; // Start with the base frequency

        if (quantizeToScale)
        {
            modulatedFrequency = GetQuantizedFrequency();
        }

        // Apply modulation from the Modulator object if available
        if (modulator != null)
        {
            // Get the modulator's frequency modulation value
            float modulatorValue = modulator.GetFrequencySampleValue();
            // Apply additive modulation with modulation depth
            modulatedFrequency += modulatorValue*modulationDepth;
        }

        // If LFO exists, generate its modulation value (affects frequency)
        if (lfoWaveGenerator != null && UseLFO)
        {
            lfoModulation = lfoWaveGenerator.GenerateSample(lfoFrequency, 1f, EWaveform.Custom); // LFO modulates frequency
        }

        // Apply LFO modulation to the modulated frequency
        modulatedFrequency *= lfoModulation;

        foreach (var waveGen in waveGenerators)
        {
            // Generate the sample value with the modulated and possibly quantized frequency
            sampleValue += waveGen.GenerateSample(modulatedFrequency, volume, waveformType);
        }

        // If ChorusEffect is enabled, normalize the sample value
        if (ChorusEffect && waveGenerators.Length > 1)
        {
            sampleValue /= waveGenerators.Length;
        }

        return sampleValue;
    }

}
