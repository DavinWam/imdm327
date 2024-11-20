using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChordIdentifier : MonoBehaviour
{

    private Dictionary<List<int>, string> chordDefinitions = new Dictionary<List<int>, string>
    {
    
    // Dyads
        { new List<int> { 0, 2 }, "Major Second Dyad (Implies Major/Add2)" },
        { new List<int> { 0, 4 }, "Major Third Dyad (Implies Major)" },
        { new List<int> { 0, 5 }, "Perfect Fourth Dyad (Implies Major/Add4)" },
        { new List<int> { 0, 7 }, "Perfect Fifth Dyad (Implies Major)" },
        { new List<int> { 0, 9 }, "Minor Sixth Dyad (Implies Minor)" },
        { new List<int> { 0, 10 }, "Minor Seventh Dyad (Implies Minor 7th)" },
    // Triads
        // Root position
        { new List<int> { 0, 4, 7 }, "Major" },
        { new List<int> { 0, 3, 7 }, "Minor" },
        { new List<int> { 0, 3, 6 }, "Diminished" },
        { new List<int> { 0, 4, 8 }, "Augmented" }, // Augmented chords are symmetrical
        // First inversion
        { new List<int> { 0, 3, 8 }, "Major (1st inversion)" },
        { new List<int> { 0, 4, 9 }, "Minor (1st inversion)" },
        { new List<int> { 0, 3, 9 }, "Diminished (1st inversion)" },
        // Second inversion
        { new List<int> { 0, 5, 9 }, "Major (2nd inversion)" },
        { new List<int> { 0, 5, 8 }, "Minor (2nd inversion)" },
        { new List<int> { 0, 6, 9 }, "Diminished (2nd inversion)" },

    //Sus Chords
        // Root position
        { new List<int> { 0, 2, 7 }, "Suspended Second (Sus2)" },
        { new List<int> { 0, 5, 7 }, "Suspended Fourth (Sus4)" },

        // First inversions(over lap with counter part sus)
        // Second inversions(overlaps with root sus2)
        { new List<int> { 0, 5, 10 }, "Suspended Second (2nd inversion)" },
    //Add Chords
        // Root position
        { new List<int> { 0, 4, 7, 2 }, "Major Add2 (Cadd2)" },
        { new List<int> { 0, 4, 7, 5 }, "Major Add4 (Cadd4)" },
        { new List<int> { 0, 4, 7, 9 }, "Major Add6 (Cadd6)" },
        
        // Inversions for Major Add2
        { new List<int> { 0, 2, 5, 9 }, "Major Add2 (1st inversion)" },
        { new List<int> { 0, 5, 7, 9 }, "Major Add2 (2nd inversion)" },
        { new List<int> { 0, 2, 4, 7 }, "Major Add2 (3rd inversion)" },

        // Inversions for Major Add4 they all overlap with a seven chord

        // Inversions for Major Add6 (other 2 inversions overlap with a seven chord)
        { new List<int> { 0, 3, 4, 8 }, "Major Add6 (1st inversion)" },
    //Seven Chords
        // Root position
        { new List<int> { 0, 4, 7, 11 }, "Major Seventh (Maj7)" },
        { new List<int> { 0, 3, 7, 10 }, "Minor Seventh (m7)" },
        { new List<int> { 0, 4, 7, 10 }, "Dominant Seventh (7)" },
        { new List<int> { 0, 3, 6, 10 }, "Half-Diminished Seventh (m7b5)" },
        { new List<int> { 0, 3, 6, 9 }, "Diminished Seventh (dim7)" }, // Diminished 7th chords are symmetrical
        { new List<int> { 0, 4, 8, 11 }, "Augmented Major Seventh (augMaj7)" },
        { new List<int> { 0, 4, 8, 10 }, "Augmented Seventh (aug7)" },
        { new List<int> { 0, 3, 7, 11 }, "Minor Major Seventh (mMaj7)" },

        // First Inversion
        { new List<int> { 0, 3, 8, 12 }, "Major Seventh (1st Inversion)" },
        { new List<int> { 0, 4, 7, 12 }, "Minor Seventh (1st Inversion)" },
      //{ new List<int> { 0, 3, 8, 12 }, "Dominant Seventh (1st Inversion)" },
        { new List<int> { 0, 3, 7, 12 }, "Half-Diminished Seventh (1st Inversion)" },
      //{ new List<int> { 0, 3, 8, 12 }, "Augmented Major Seventh (1st Inversion)" },
        { new List<int> { 0, 2, 8, 12 }, "Augmented Seventh (1st Inversion)" },
        { new List<int> { 0, 4, 8, 12 }, "Minor Major Seventh (1st Inversion)" },

        // Second Inversion
        { new List<int> { 0, 4, 5, 9 }, "Major Seventh (2nd Inversion)" },
        { new List<int> { 0, 3, 5, 9 }, "Minor Seventh (2nd Inversion)" },
      //{ new List<int> { 0, 3, 5, 9 }, "Dominant Seventh (2nd Inversion)" },
        { new List<int> { 0, 4, 6, 9 }, "Half-Diminished Seventh (2nd Inversion)" },
        { new List<int> { 0, 4, 5, 8 }, "Augmented Major Seventh (2nd Inversion)" },
        { new List<int> { 0, 4, 6, 8 }, "Augmented Seventh (2nd Inversion)" },
        { new List<int> { 0, 3, 5, 8 }, "Minor Major Seventh (2nd Inversion)" },

        // Third Inversion
        { new List<int> { 0, 1, 5, 8 }, "Major Seventh (3rd Inversion)" },
        { new List<int> { 0, 2, 5, 8 }, "Minor Seventh (3rd Inversion)" },
        //{ new List<int> { 0, 2, 5, 8 }, "Dominant Seventh (3rd Inversion)" },
        { new List<int> { 0, 2, 5, 9 }, "Half-Diminished Seventh (3rd Inversion)" },
        { new List<int> { 0, 1, 4, 8 }, "Augmented Major Seventh (3rd Inversion)" },
        { new List<int> { 0, 2, 4, 8 }, "Augmented Seventh (3rd Inversion)" },
        //{ new List<int> { 0, 1, 5, 8 }, "Minor Major Seventh (3rd Inversion)" }

    };



    // Converts note names to pitch classes, adjusts for the key, and identifies the chord
    public string IdentifyChord(List<NoteEventInfo> noteEvents, int keyNoteNumber)
    {
        // Print the input notes for debugging
        //Debug.Log($"Input Notes: {string.Join(", ", noteEvents.Select(ne => $"NoteNumber: {ne.NoteNumber}"))}");

        // Handle single notes instantly by identifying the note as a root note
        if (noteEvents.Count == 1)
        {
            return "not a chord";
        }

        int keyPitchClass = keyNoteNumber % 12;

        var adjustedNotes = noteEvents.Select(noteEvent => (noteEvent.NoteNumber - keyPitchClass + 12) % 12).ToList();
        adjustedNotes.Sort(); // Sort notes for consistent interval calculation

        // Print the adjusted (normalized) notes for debugging
        //Debug.Log($"Adjusted Notes: {string.Join(", ", adjustedNotes)}");

        List<int> intervals = new List<int>();
        for (int i = 0; i < adjustedNotes.Count; i++)
        {
            int interval = (adjustedNotes[i] - adjustedNotes[0] + 12) % 12;
            intervals.Add(interval);
        }
        intervals = intervals.Distinct().OrderBy(x => x).ToList(); // Unique, sorted intervals

        // Print the calculated intervals for debugging
        //Debug.Log($"Intervals: {string.Join(", ", intervals)}");

        foreach (var chord in chordDefinitions)
        {
            if (Enumerable.SequenceEqual(intervals, chord.Key))
            {
                return chord.Value;
            }
        }

        return "Unknown";
    }






    /*
     *     
    public Dictionary<string, int> noteToPitchClass = new Dictionary<string, int>
    {
        {"C", 0}, {"C#", 1}, {"Db", 1}, {"D", 2}, {"D#", 3}, {"Eb", 3},
        {"E", 4}, {"F", 5}, {"F#", 6}, {"Gb", 6}, {"G", 7}, {"G#", 8},
        {"Ab", 8}, {"A", 9}, {"A#", 10}, {"Bb", 10}, {"B", 11}, {"Cb", 11}
    };
    public string IdentifyChord(List<string> notes, string key)
    {
        int keyPitchClass = noteToPitchClass[key];

        // Convert notes to pitch classes and adjust for the key
        var adjustedNotes = notes.Select(note => (noteToPitchClass[note] - keyPitchClass + 12) % 12).ToList();
        adjustedNotes.Sort(); // Make sure notes are sorted for consistent interval calculation

        // Calculate intervals from the root note of the chord
        List<int> intervals = new List<int>();
        for (int i = 0; i < adjustedNotes.Count; i++)
        {
            int interval = (12 + adjustedNotes[i] - adjustedNotes[0]) % 12;
            intervals.Add(interval);
        }
        intervals = intervals.Distinct().OrderBy(x => x).ToList(); // Ensure unique intervals, sorted

        // Debugging: Print the adjusted notes and intervals
        //  Debug.Log($"Adjusted Notes: {string.Join(", ", adjustedNotes)}");
        // Debug.Log($"Intervals: {string.Join(", ", intervals)}");

        // Find a matching chord
        foreach (var chord in chordDefinitions)
        {
            if (Enumerable.SequenceEqual(intervals, chord.Key))
            {
                return chord.Value;
            }
        }

        return "Unknown";
    }
    // Uncomment the function above if you need to debug whether the chords are being correctly parsed from a group of notes
    */



}
