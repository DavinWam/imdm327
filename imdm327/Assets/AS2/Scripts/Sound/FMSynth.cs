using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class FMSynth : MonoBehaviour
{
    private enum EnvelopeState
    {
        Idle,
        Attack,
        Decay,
        Sustain,
        Release
    }

    public Operator[] operators; // Array of operators (carriers and modulators)
    public float duration = 2.0f; // Duration in seconds

    // Envelope parameters
    public float attackTime = 0.1f;
    public float decayTime = 0.2f;
    [Range(0, 1)]
    public float sustainLevel = 0.7f;
    public float releaseTime = 0.5f;

    public bool playOnStart = true;
    public bool noteOn = false;
    // Envelope state tracking
    private EnvelopeState envelopeState = EnvelopeState.Idle;
    private float envelopeTime = 0f;
    private float currentAmplitude = 0f;

    // Cached sample rate
    private float sampleRate;

  public BoidSoundManager boidSoundManager;

    void Awake()
    {
        // Assuming BoidSoundManager is on the same GameObject

        sampleRate = AudioSettings.outputSampleRate;

        if (playOnStart)
        {
            NoteOn();
        }
    }

  private void OnAudioFilterRead(float[] data, int channels)
    {
        if (operators == null || operators.Length == 0 || boidSoundManager == null)
        {
            return;
        }

        // Check with BoidSoundManager whether this synth can be active
        if (!boidSoundManager.CanSynthBeActive(this))
        {
            return; // Don't output sound if synth is not allowed to be active
        }

        Operator mainOperator = operators[0];

        // Get the active synth count from BoidSoundManager
        int activeSynthCount = boidSoundManager != null ? boidSoundManager.GetActiveSynthCount() : 1;

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            // Update envelope
            UpdateEnvelope();

            // Generate the next sample for the current operator
            float sampleValue = mainOperator.GetFrequencySampleValue();

            // Apply the envelope to the sample
            sampleValue *= currentAmplitude;

            // Assign the sample value to all channels (stereo/mono)
            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] += sampleValue / activeSynthCount; // Dynamic mixing
                data[sample + channel] = Mathf.Clamp(data[sample + channel], -1.0f, 1.0f);
            }
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


    }

    private void UpdateEnvelope()
    {
        switch (envelopeState)
        {
            case EnvelopeState.Idle:
                currentAmplitude = 0f;
                break;

            case EnvelopeState.Attack:
                envelopeTime += 1f / sampleRate;
                currentAmplitude = Mathf.Clamp01(envelopeTime / attackTime);
                if (envelopeTime >= attackTime)
                {
                    envelopeState = EnvelopeState.Decay;
                    envelopeTime = 0f;
                }
                break;

            case EnvelopeState.Decay:
                envelopeTime += 1f / sampleRate;
                currentAmplitude = Mathf.Lerp(1f, sustainLevel, envelopeTime / decayTime);
                if (envelopeTime >= decayTime)
                {
                    envelopeState = EnvelopeState.Sustain;
                    envelopeTime = 0f;
                }
                break;

            case EnvelopeState.Sustain:
                currentAmplitude = sustainLevel;
                break;

            case EnvelopeState.Release:
                envelopeTime += 1f / sampleRate;
                currentAmplitude = Mathf.Lerp(currentAmplitude, 0f, envelopeTime / releaseTime);
                if (envelopeTime >= releaseTime)
                {
                    envelopeState = EnvelopeState.Idle;
                    envelopeTime = 0f;
                    currentAmplitude = 0f;
                }
                break;
        }
    }



    public void NoteOn()
    {
        // If a note is already on, restart the envelope
        envelopeState = EnvelopeState.Attack;
        envelopeTime = 0f;
        noteOn = true;
    }

    public void NoteOff()
    {
        if (envelopeState != EnvelopeState.Idle && envelopeState != EnvelopeState.Release)
        {
            envelopeState = EnvelopeState.Release;
            envelopeTime = 0f;
        }
        noteOn = false;
    }


}