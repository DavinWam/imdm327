using System;
using UnityEngine;

public class NoteEventInfo : IComparable<NoteEventInfo>
{
    public int NoteNumber;
    public float StartTime;
    public float EndTime;
    public float length;
    public ENoteSharps Type; // Add the NoteType 

    public NoteEventInfo(int noteNumber, float startTime, float length, float bpm, float ticks)
    {
        this.NoteNumber = noteNumber;
        this.StartTime = (60f * startTime) / (bpm * ticks);
        this.EndTime = this.StartTime + (60f * length) / (bpm * ticks);
        this.length = length;
        // Calculate the NoteType based on the MIDI note number
        this.Type = (ENoteSharps)((noteNumber - 12) % 12); // MIDI note numbers start at 12 (C0)


    }

    // Merge two NoteEventInfo objects
    public void MergeNotes(NoteEventInfo otherNote)
    {
        // Check if the notes have the same pitch (Type)
        if (this.Type == otherNote.Type)
        {
            // Update the target note's start time to the minimum of the two
            this.StartTime = Mathf.Min(this.StartTime, otherNote.StartTime);

            // Update the target note's end time to the maximum of the two
            this.EndTime = Mathf.Max(this.EndTime, otherNote.EndTime);
        }
    }

    // Implement the CompareTo method of the IComparable interface
    public int CompareTo(NoteEventInfo other)
    {
        if (other == null)
        {
            return 1; // If the other object is null, this object is greater
        }
        return this.StartTime.CompareTo(other.StartTime);
    }

}