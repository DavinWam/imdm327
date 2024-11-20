using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicKeyManager", menuName = "MusicKeyManager", order = 1)]
public class MusicKeyManager : ScriptableObject
{
    // Enum for all major keys


    // Current key the application is in
    public MusicalKeys currentKey = MusicalKeys.C;

    // Mapping note numbers (C, D, E, F, G, A, B) to their pitch adjustments
    private Dictionary<MusicalKeys, float[]> keyPitchAdjustments = new Dictionary<MusicalKeys, float[]>()
    {
        { MusicalKeys.C, new float[] {1.0f, 1.12246204831f, 1.25992104989f, 1.33483985417f, 1.49830707688f, 1.68179283051f, 1.88774862536f} }, // C Major
        { MusicalKeys.G, new float[] {1.49830707688f, 1.68179283051f, 1.88774862536f, 2.0f, 2.24492409662f, 2.51984209979f, 2.82842712475f} }, // G Major
        { MusicalKeys.D, new float[] {1.12246204831f, 1.25992104989f, 1.41421356237f, 1.49830707688f, 1.68179283051f, 1.88774862536f, 2.12246204831f} }, // D Major
        { MusicalKeys.A, new float[] {1.68179283051f, 1.88774862536f, 2.12246204831f, 2.24492409662f, 2.51984209979f, 2.82842712475f, 3.17480210394f} }, // A Major
        { MusicalKeys.E, new float[] {1.25992104989f, 1.41421356237f, 1.58740105197f, 1.68179283051f, 1.88774862536f, 2.12246204831f, 2.37841423001f} }, // E Major
        { MusicalKeys.B, new float[] {1.88774862536f, 2.12246204831f, 2.37841423001f, 2.51984209979f, 2.82842712475f, 3.17480210394f, 3.56359487257f} }, // B Major (Cb Major)
        { MusicalKeys.Fs, new float[] {1.41421356237f, 1.58740105197f, 1.78179743628f, 1.88774862536f, 2.12246204831f, 2.37841423001f, 2.66840164872f} }, // F# Major (Gb Major)
        
        { MusicalKeys.Cs, new float[] { 1.05946309436f, 1.12246204831f, 1.25992104989f, 1.33483985417f, 1.49830707688f, 1.68179283051f, 1.88774862536f } },// Db Major (C# Major)

        { MusicalKeys.Cb, new float[] { 0.943874312681f, 1.0f, 1.12246204831f, 1.189207115f, 1.33483985417f, 1.49830707688f, 1.68179283051f } },// B Major (Cb Major)
        { MusicalKeys.Gb, new float[] { 1.33483985417f, 1.41421356237f, 1.58740105197f, 1.68179283051f, 1.88774862536f, 2.12246204831f, 2.37841423001f } }, // F# Major (Gb Major)
        { MusicalKeys.Db, new float[] {1.05946309436f, 1.189207115f, 1.33483985417f, 1.41421356237f, 1.58740105197f, 1.78179743628f, 2.0f} }, // Db Major (C# Major)
        { MusicalKeys.Ab, new float[] {1.58740105197f, 1.78179743628f, 2.0f, 2.12246204831f, 2.37841423001f, 2.66840164872f, 3.0f} }, // Ab Major 
        { MusicalKeys.Eb, new float[] {1.189207115f, 1.33483985417f, 1.49830707688f, 1.58740105197f, 1.78179743628f, 2.0f, 2.24492409662f} }, // Eb Major
        { MusicalKeys.Bb, new float[] {1.78179743628f, 2.0f, 2.24492409662f, 2.37841423001f, 2.66840164872f, 3.0f, 3.36358566101f} }, // Bb Major (A# Major)
        { MusicalKeys.F, new float[] {1.33483985417f, 1.49830707688f, 1.68179283051f, 1.78179743628f, 2.0f, 2.24492409662f, 2.51984209979f} }, // F Major



        { MusicalKeys.Am, new float[] { 1.0f, 1.12246204831f, 1.189207115f, 1.33483985417f, 1.49830707688f, 1.58740105197f, 1.78179743628f } },
        { MusicalKeys.Em, new float[] { 1.49830707688f, 1.58740105197f, 1.68179283051f, 1.88774862536f, 2.12246204831f, 2.24492409662f, 2.37841423001f } },
        { MusicalKeys.Bm, new float[] { 1.88774862536f, 2.0f, 2.12246204831f, 2.37841423001f, 2.51984209979f, 2.66840164872f, 2.82842712475f } },
        { MusicalKeys.Fsm, new float[] { 1.41421356237f, 1.49830707688f, 1.58740105197f, 1.78179743628f, 1.88774862536f, 2.0f, 2.12246204831f } },
        { MusicalKeys.Csm, new float[] { 1.05946309436f, 1.12246204831f, 1.189207115f, 1.33483985417f, 1.41421356237f, 1.49830707688f, 1.58740105197f } },
        { MusicalKeys.Gsm, new float[] { 1.68179283051f, 1.78179743628f, 1.88774862536f, 2.0f, 2.12246204831f, 2.24492409662f, 2.37841423001f } },
        { MusicalKeys.Dsm, new float[] { 1.25992104989f, 1.33483985417f, 1.41421356237f, 1.49830707688f, 1.58740105197f, 1.68179283051f, 1.78179743628f } },
        
        { MusicalKeys.Asm, new float[] { 1.78179743628f, 1.88774862536f, 2.0f, 2.12246204831f, 2.24492409662f, 2.37841423001f, 2.51984209979f } },
        
        { MusicalKeys.Abm, new float[] { 1.58740105197f, 1.68179283051f, 1.78179743628f, 1.88774862536f, 2.0f, 2.12246204831f, 2.24492409662f } },
        { MusicalKeys.Ebm, new float[] { 1.189207115f, 1.25992104989f, 1.33483985417f, 1.41421356237f, 1.49830707688f, 1.58740105197f, 1.68179283051f } },
        { MusicalKeys.Bbm, new float[] { 1.78179743628f, 1.88774862536f, 1.98107365659f, 2.12246204831f, 2.24492409662f, 2.37841423001f, 2.51984209979f } },
        { MusicalKeys.Fm, new float[] { 1.33483985417f, 1.41421356237f, 1.49830707688f, 1.58740105197f, 1.68179283051f, 1.78179743628f, 1.88774862536f } },
        { MusicalKeys.Cm, new float[] { 1.0f, 1.05946309436f, 1.12246204831f, 1.189207115f, 1.25992104989f, 1.33483985417f, 1.41421356237f } },
        { MusicalKeys.Gm, new float[] { 1.49830707688f, 1.58740105197f, 1.68179283051f, 1.78179743628f, 1.88774862536f, 2.0f, 2.12246204831f } },
        { MusicalKeys.Dm, new float[] { 1.12246204831f, 1.189207115f, 1.25992104989f, 1.33483985417f, 1.41421356237f, 1.49830707688f, 1.58740105197f } }


    };



    public float GetPitchAdjustment(int noteNumber)
    {
        // Adjust for zero-based indexing and validate range
        int index = noteNumber - 1;
        if (index < 0 || index >= 7)
        {
            Debug.LogError("Note number out of range. Please assign a number between 1 and 7.");
            return 1.0f; // Return standard pitch if out of range
        }

        if (keyPitchAdjustments.TryGetValue(currentKey, out float[] adjustments))
        {
            return adjustments[index];
        }
        else
        {
            Debug.LogError("Key not found. Defaulting to C Major.");
            return 1.0f; // Default to standard pitch if key not found
        }
    }

    // Method to change the current key
    public void SetCurrentKey(MusicalKeys key)
    {
        currentKey = key;
    }
}
