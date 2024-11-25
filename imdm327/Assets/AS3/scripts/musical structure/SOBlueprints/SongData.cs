using UnityEngine;
using NAudio.Midi;
using System.Collections.Generic;
using System.IO;


[CreateAssetMenu(fileName = "NewSongData", menuName = "Sound/Song Data")]
public class SongData : ScriptableObject
{ 
    // Fields
    public string songName;
    public MusicalKeys musicalKey = MusicalKeys.C;
    /* Here are the values for each key might make the input a string instead since it would be easier
    {"C", 0}, {"C#", 1}, {"Db", 1}, {"D", 2}, {"D#", 3}, {"Eb", 3},
    {"E", 4}, {"F", 5}, {"F#", 6}, {"Gb", 6}, {"G", 7}, {"G#", 8},
    {"Ab", 8}, {"A", 9}, {"A#", 10}, {"Bb", 10}, {"B", 11}, {"Cb", 11}
    */
    
    //MIDI fields
    public MidiFile midi {get; private set;} // MidiFile field
    public float offset;
    public float bpm = 120f;
    public float trackNumber = 0;
    [HideInInspector]
    public float tickRate;

    public TextAsset data;
    // List to store all notes
    private List<NoteEventInfo> allNotes = new List<NoteEventInfo>();

    //takes all midiEvents in the midiEvents list to NoteEventInfo's and sort's them by start time
    public void InitializeNotes()
    {
        if(data == null){
            Debug.LogError("Data file is not assigned.");
            return;
        }
        Stream s = new MemoryStream(data.bytes);
        midi = new MidiFile(s, true);

        if (midi == null)
        {
            Debug.LogError("MIDI file could not be parsed from data.");
            return;
        }
        tickRate = midi.DeltaTicksPerQuarterNote;
        
        if (trackNumber >= midi.Events.Tracks)
        {
            Debug.LogError("Track number does not exist. Please select a valid track.");
            return; // Exit the method if the track does not exist
        }
        // Clear existing notes
        allNotes.Clear();

        // Fetch and sort all notes by their start time
        foreach (MidiEvent note in midi.Events[(int)trackNumber])
        {
            if (note.CommandCode == MidiCommandCode.NoteOn)
            {
                NoteOnEvent noe = (NoteOnEvent)note;
                float seconds = (noe.NoteLength / tickRate) * (60.0f / bpm);

                allNotes.Add(new NoteEventInfo(noe.NoteNumber, noe.AbsoluteTime, seconds, bpm, tickRate));
            }
        }
        //colllect track information
        UpdateTrackInfo();
        // Sort allNotes by start time
        allNotes.Sort();
    }

    public List<NoteEventInfo> GetNotes()
    {
        return new List<NoteEventInfo>(allNotes);
    }
    [HideInInspector]
    public List<string> trackInfo = new List<string>();
    public void UpdateTrackInfo()
    {
        trackInfo.Clear();

        if (midi == null)
        {
            Debug.LogError("MIDI file not initialized. InitializeNotes must be called first.");
            return;
        }

        for (int i = 0; i < midi.Events.Tracks; i++)
        {
            bool hasNotes = false;

            foreach (MidiEvent midiEvent in midi.Events[i])
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    hasNotes = true;
                    break;
                }
            }

            if (hasNotes)
            {
                trackInfo.Add($"Track {i + 1} (Has Notes)");
            }
            else
            {
                trackInfo.Add($"Track {i + 1} (No Notes)");
            }
        }
    }


}
