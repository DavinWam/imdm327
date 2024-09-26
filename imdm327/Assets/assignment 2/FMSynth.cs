using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class FMSynth : MonoBehaviour
{
    public Wave[] carrierWaves; // Array of carrier waves

    // Envelope parameters
    public float attackTime = 0.1f;
    public float decayTime = 0.2f;
    [Range(0, 1)]
    public float sustainLevel = 0.7f;
    public float releaseTime = 0.5f;

    public bool playOnStart = true;

    private float envelopeVolume = 0;
    private float timeSinceNoteOn = 0;
    public bool noteOn = false;

    // Reference to the FM Synth class
    private FrequencyModulationSynthesizer[] fmSynths;

    void Start()
    {
        // Initialize the array of synthesizers
        fmSynths = new FrequencyModulationSynthesizer[carrierWaves.Length];
        for (int i = 0; i < carrierWaves.Length; i++)
        {
            fmSynths[i] = new FrequencyModulationSynthesizer();
        }

        if (playOnStart)
        {
            NoteOn();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NoteOn();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            NoteOff();
        }

        ProcessEnvelope(Time.deltaTime);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        System.Array.Clear(data, 0, data.Length);
        float[] tempData = new float[data.Length];

        for (int i = 0; i < carrierWaves.Length; i++)
        {
            Wave wave = carrierWaves[i];
            FrequencyModulationSynthesizer synth = fmSynths[i];

            float outputFrequency = wave.GetQuantizedFrequency();
            System.Array.Clear(tempData, 0, tempData.Length);

            synth.OnAudioFilterRead(tempData, channels, outputFrequency, wave.volume * envelopeVolume, wave.waveformType);

            for (int j = 0; j < data.Length; j++)
            {
                data[j] += tempData[j];
            }
        }

        // Prevent clipping
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Mathf.Clamp(data[i], -1f, 1f);
        }
    }

    void ProcessEnvelope(float deltaTime)
    {
        if (noteOn)
        {
            timeSinceNoteOn += deltaTime;

            if (timeSinceNoteOn <= attackTime)
            {
                envelopeVolume = Mathf.Lerp(0, 1, timeSinceNoteOn / attackTime);
            }
            else if (timeSinceNoteOn <= attackTime + decayTime)
            {
                float decayProgress = (timeSinceNoteOn - attackTime) / decayTime;
                envelopeVolume = Mathf.Lerp(1, sustainLevel, decayProgress);
            }
            else
            {
                envelopeVolume = sustainLevel;
            }
        }
        else
        {
            envelopeVolume -= deltaTime / releaseTime;
            if (envelopeVolume < 0)
            {
                envelopeVolume = 0;
            }
        }
    }

    public void NoteOff()
    {
        noteOn = false;
    }

    public void NoteOn()
    {
        if (noteOn)
        {
            NoteOff();
        }
        noteOn = true;
        timeSinceNoteOn = 0;

        // Reset phases
        foreach (var synth in fmSynths)
        {
            synth.ResetPhase();
        }
    }
}
