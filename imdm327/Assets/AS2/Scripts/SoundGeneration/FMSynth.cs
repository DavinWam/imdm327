using UnityEngine;

public class FMSynth : MonoBehaviour
{
    public Operator[] operators; // Array of operators (carriers and modulators)
    public bool playOnStart = true;
    public bool noteOn { get; private set; } = false;

    private float sampleRate;
    public SoundManager soundManager;

    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;

        if (playOnStart && soundManager)
        {
            soundManager.RegisterSynth(this, true);
            NoteOn();
        }
    }
    // C# Method: ProcessAudioData
    public ComputeShader audioComputeShader;
    private ComputeBuffer audioDataBuffer;

    void ProcessAudioData(float[] audioData, float volume)
    {
        int dataLength = audioData.Length;

        // Initialize the compute buffer
        audioDataBuffer = new ComputeBuffer(dataLength, sizeof(float));
        audioDataBuffer.SetData(audioData);

        // Find the kernel
        int kernelHandle = audioComputeShader.FindKernel("CSMain");

        // Set the buffer and parameters
        audioComputeShader.SetBuffer(kernelHandle, "audioDataBuffer", audioDataBuffer);
        audioComputeShader.SetFloat("volumeMultiplier", volume);

        // Dispatch the compute shader
        int threadGroups = Mathf.CeilToInt(dataLength / 256.0f);
        audioComputeShader.Dispatch(kernelHandle, threadGroups, 1, 1);

        // Retrieve the processed data
        audioDataBuffer.GetData(audioData);

        // Release the buffer
        audioDataBuffer.Release();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (operators == null || operators.Length == 0 || soundManager == null)
        {
            return;
        }

        // Check if the synth should be active
        bool isActive = soundManager.CanSynthBeActive(this);

        if (!isActive)
        {
            return;
        }

        Operator mainOperator = operators[0];
        int activeSynthCount = soundManager != null ? soundManager.GetActiveSynthCount() : 1;
       // ProcessAudioData(data, mainOperator.GetFrequencySampleValue(noteOn, 1f/sampleRate));

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            float sampleValue = mainOperator.GetFrequencySampleValue(noteOn, 1f/sampleRate);

            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] += sampleValue / activeSynthCount;
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

    public void NoteOn()
    {
        noteOn = true;
        Debug.Log("Note On");
    }

    public void NoteOff()
    {
        noteOn = false;
        Debug.Log("Note Off");
    }

    // Function to play a MIDI note
    public void PlayNote(float frequency)
    {
        foreach (Operator op in operators)
        {
            // Add a WaveGenerator for the given MIDI note frequency
            WaveGenerator waveGen = op.AddWave(frequency);
        }
    }

    // Function to stop a MIDI note
    public void StopNote()
    {
        foreach (Operator op in operators)
        {
            // Remove all WaveGenerators
            foreach (WavePair wavePair in op.waveGenerators)
            {
                op.RemoveWave(wavePair.waveGenerator);
                Debug.Log("Removed wave");
            }
        }
    }

}
