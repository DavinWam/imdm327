using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SongData))]
public class SongDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SongData songData = (SongData)target;

        // Default Inspector
        DrawDefaultInspector();

        // Button to refresh track info
        if (GUILayout.Button("Refresh Track Info"))
        {
            songData.InitializeNotes();
            songData.UpdateTrackInfo();
        }

        // Display track information
        if (songData.trackInfo != null && songData.trackInfo.Count > 0)
        {
            EditorGUILayout.LabelField("Tracks:", EditorStyles.boldLabel);
            foreach (string track in songData.trackInfo)
            {
                EditorGUILayout.LabelField(track);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No track info available. Load a MIDI file and click 'Refresh Track Info'.", MessageType.Info);
        }
    }
}
