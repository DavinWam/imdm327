using UnityEngine;

[System.Serializable]
public struct Operator
{
    [Range(1, 5000)]
    public float baseFrequency; // Base frequency of the operator

    public float frequencyMultiplier; // Frequency multiplier

    [Range(0, 1)]
    public float volume; // Volume of the operator (used for carriers)

    public EWaveform waveformType; // Type of waveform

    public bool quantizeToScale; // Whether to quantize the frequency
    private ENote currentNote; // Current note after quantization

    public int modulatorIndex; // Index of the operator that modulates this operator

    [Range(0, 1000)]
    public float modulationDepth; // Controls the extent of frequency deviation

    public bool isCarrier; // Indicates if this operator is a carrier

    // Constructor
    public Operator(float baseFrequency, float frequencyMultiplier, float volume, EWaveform waveformType,
                    bool quantizeToScale, int modulatorIndex, bool isCarrier, float modulationDepth)
    {
        this.baseFrequency = baseFrequency;
        this.frequencyMultiplier = frequencyMultiplier;
        this.volume = volume;
        this.waveformType = waveformType;
        this.quantizeToScale = quantizeToScale;
        this.currentNote = ENote.None;
        this.modulatorIndex = modulatorIndex;
        this.isCarrier = isCarrier;
        this.modulationDepth = modulationDepth;
    }

    // Quantizes the input frequency to the closest note on the selected scale
    public float GetQuantizedFrequency()
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
}
