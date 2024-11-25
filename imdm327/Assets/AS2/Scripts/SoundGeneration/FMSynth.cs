using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SynthMode{
    Midi, //plays midi notes
    Dynamic // You're expected to turn on and off any aspect of the synth yourself
}
public class FMSynth : MonoBehaviour
{
    public Operator[] operators; // Array of operators (carriers and modulators)
    public bool playOnStart = true;
    public bool RegisterOnStart = false;

    //if can make sound if not then it won't
    public bool noteOn { get; private set; } = false;

    private float sampleRate;
    public SoundManager soundManager;
    
    public SynthMode mode = SynthMode.Dynamic;
        
    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        // Instantiate unique copies of operators
        InstantiateOperators();
        SwitchMode();
        
        if (playOnStart && soundManager)
        {
            soundManager.RegisterSynth(this, true);
            NoteOn();
        }
        if(RegisterOnStart){
            soundManager.RegisterSynth(this,true);
        }
    }
    private void InstantiateOperators()
    {
        if (operators == null || operators.Length == 0)
        {
            Debug.LogWarning("No operators assigned to FMSynth.");
            return;
        }

        for (int i = 0; i < operators.Length; i++)
        {
            if (operators[i] != null)
            {
                // Clone the ScriptableObject
                operators[i] = Instantiate(operators[i]);
            }
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

        // Check if the synth is allowed to be active
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
        if(mode == SynthMode.Midi){
            noteOn = true;
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     NoteOn();
        // }

        // if (Input.GetKeyUp(KeyCode.Space))
        // {
        //     NoteOff();
        // }
    }
    public void SwitchMode(){
        switch(mode){
            case SynthMode.Midi:
                SwitchToMidiManagement();
                break;
            case SynthMode.Dynamic:
                SwitchToDynamicManagement();
                break; 
        }
    }
    public void SwitchToMidiManagement(){
        mode = SynthMode.Midi;
        foreach(Operator op in operators){
            op.baseFrequency = 0;
        }
    }
    public void SwitchToDynamicManagement(){
        mode = SynthMode.Dynamic;
        foreach(Operator op in operators){
            op.RemoveAllWaves();
        }
    }
    public void NoteOn()
    {
        noteOn = true;
    }
    public void NoteOn(float duration){
        noteOn = true;
        StartCoroutine(NoteOffAfterDuration(duration));
    }
    private IEnumerator NoteOffAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        NoteOff();
    }

    public void NoteOff()
    {
        noteOn = false;
    }

    // Function to play a MIDI note
    public void PlayNote(float frequency, NoteEventInfo noteEvent)
    {   
        List<WavePair> pairs = new List<WavePair>();
        foreach (Operator op in operators)
        {
            // Add a WaveGenerator for the given MIDI note frequency
            pairs.Add(op.AddWave(frequency));
        }
        StartCoroutine(AutoStopNote(noteEvent.length, pairs));
    }

    IEnumerator AutoStopNote(float duration, List<WavePair> pairs){
        yield return new WaitForSeconds(duration);  
        StopNote(pairs);
    }
    // Function to stop a MIDI note
    public void StopNote(List<WavePair>  wavePairs)
    {
        foreach (Operator op in operators)
        {
            // Remove all WaveGenerators
            op.RemoveWave(wavePairs[0]);
            wavePairs.RemoveAt(0);
            // Debug.Log("Removed wave");
        }
    }

}
