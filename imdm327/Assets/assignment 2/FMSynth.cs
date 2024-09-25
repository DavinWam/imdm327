using UnityEngine;
using UnityEngine.Events;

public class FMSynth : MonoBehaviour
{
    public Wave wave; // The wave used by the FMSynth

    // Envelope parameters
    public float attackTime = 0.1f; // Time to reach full volume
    public float decayTime = 0.2f;  // Time to decay to sustain level
    [Range(0, 1)]
    public float sustainLevel = 0.7f; // Level during sustain phase
    public float releaseTime = 0.5f;  // Time to fade out

    public bool playOnStart = true; // Play on start boolean

    private float envelopeVolume = 0; // Envelope volume over time
    private float timeSinceNoteOn = 0; // Time since the note started
    public bool noteOn = false;       // Keeps track of whether the note is on

    // Unity Events for NoteOn and NoteOff
    public UnityEvent onNoteOn;
    public UnityEvent onNoteOff;

    // Reference to the FM Synth class
    private FrequencyModulationSynthesizer fmSynth;

    // Initialize FM Synthesizer and the wave
    void Start()
    {
        fmSynth = new FrequencyModulationSynthesizer();

        // Play the note on start if playOnStart is true
        if (playOnStart)
        {
            NoteOn();
        }
    }

    // Update is called once per frame and handles envelope processing on the main thread
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Press space to play a note
        {
            NoteOn();
        }

        if (Input.GetKeyUp(KeyCode.Space)) // Release space to stop the note
        {
            NoteOff();
        }
        // Process the envelope on the main thread, using Time.deltaTime
        ProcessEnvelope(Time.deltaTime);
    }

    // OnAudioFilterRead is called by Unity's audio system when generating the audio data
    void OnAudioFilterRead(float[] data, int channels)
    {
        // Get the quantized frequency from the wave
        float outputFrequency = wave.GetQuantizedFrequency();

        // Generate the audio data with quantized frequency and apply the envelope
        fmSynth.OnAudioFilterRead(data, channels, outputFrequency, wave.volume * envelopeVolume);
    }

    // Process the ADSR envelope over time
    void ProcessEnvelope(float deltaTime)
    {
        if (noteOn)
        {
            timeSinceNoteOn += deltaTime;

            if (timeSinceNoteOn <= attackTime)
            {
                // Attack phase: increase volume to 1.0
                envelopeVolume = Mathf.Lerp(0, 1, timeSinceNoteOn / attackTime);
            }
            else if (timeSinceNoteOn <= attackTime + decayTime)
            {
                // Decay phase: decrease volume to sustain level
                float decayProgress = (timeSinceNoteOn - attackTime) / decayTime;
                envelopeVolume = Mathf.Lerp(1, sustainLevel, decayProgress);
            }
            else
            {
                // Sustain phase: maintain sustain level
                envelopeVolume = sustainLevel;
            }
        }
        else
        {
            // Release phase: decrease volume to 0
            envelopeVolume -= deltaTime / releaseTime;
            if (envelopeVolume < 0)
            {
                envelopeVolume = 0;
            }
        }
    }

    // Call this to stop the note and trigger the release phase
    public void NoteOff()
    {
        noteOn = false;
        onNoteOff?.Invoke(); // Trigger the UnityEvent for NoteOff
    }

    // Call this to trigger the note on (attack phase)
    public void NoteOn()
    {
        if(noteOn){
            NoteOff();
        }
        noteOn = true;
        timeSinceNoteOn = 0;
        onNoteOn?.Invoke(); // Trigger the UnityEvent for NoteOn
    }
}
