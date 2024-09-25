using UnityEngine;

[System.Serializable]
public struct Wave
{

    [Range(65, 5000)]
    public float baseFrequency; // The frequency of the wave

    [Range(0, 1)]
    public float volume; // The volume of the wave

    public bool quantizeToScale; // Whether to quantize the frequency to a musical scale
    public ScaleType selectedScale; // Allows selection between different scales
    public Note currentNote;

    // Static major scale intervals (for a major scale starting from A)
    public static readonly float[] MajorScale = { 1.0f, 1.125f, 1.25f, 1.333f, 1.5f, 1.667f, 1.875f };

    // Updated mapping for A major scale notes
    public static readonly Note[] MajorScaleNotes = {
        Note.A, Note.B, Note.CSharp, Note.D, Note.E, Note.FSharp, Note.GSharp
    };

    // Static 12-tone scale intervals
    public static readonly float[] TwelveToneScale = {
        1.0f, 1.05946f, 1.12246f, 1.18921f, 1.25992f, 1.33484f,
        1.41421f, 1.49831f, 1.58740f, 1.68179f, 1.78180f, 1.88775f
    };

    // Static mapping for 12-tone scale notes
    public static readonly Note[] TwelveToneScaleNotes = {
        Note.A, Note.ASharp, Note.B, Note.C, Note.CSharp,
        Note.D, Note.DSharp, Note.E, Note.F, Note.FSharp,
        Note.G, Note.GSharp
    };

    // Constructor for the Wave struct
    public Wave(float baseFrequency, float volume, bool quantizeToScale, ScaleType selectedScale)
    {
        this.baseFrequency = baseFrequency;
        this.volume = volume;
        this.quantizeToScale = quantizeToScale;
        this.selectedScale = selectedScale;
        this.currentNote = Note.None;
    }

    // Quantizes the input frequency to the closest note on the selected scale
    public float GetQuantizedFrequency()
    {
        if (!quantizeToScale)
        {
            currentNote = Note.None;
            return baseFrequency;
        }

        float baseNoteFrequency = 440f; // A4 (the reference pitch in Hz)
        float[] selectedScaleIntervals = (selectedScale == ScaleType.Major) ? MajorScale : TwelveToneScale;
        Note[] selectedScaleNotes = (selectedScale == ScaleType.Major) ? MajorScaleNotes : TwelveToneScaleNotes;

        float quantizedFrequency = baseNoteFrequency;
        float minDifference = Mathf.Infinity;

        // Loop through octaves (multiply by powers of 2 to account for octave shifts)
        for (int octave = -4; octave <= 4; octave++)
        {
            float octaveMultiplier = Mathf.Pow(2, octave);

            // Loop through the selected scale intervals
            for (int i = 0; i < selectedScaleIntervals.Length; i++)
            {
                float currentNoteFreq = baseNoteFrequency * selectedScaleIntervals[i] * octaveMultiplier;
                float difference = Mathf.Abs(baseFrequency - currentNoteFreq);

                if (difference < minDifference)
                {
                    quantizedFrequency = currentNoteFreq;
                    minDifference = difference;
                    currentNote = selectedScaleNotes[i]; // Update the current note based on the selected scale
                }
            }
        }

        return quantizedFrequency;
    }
}
