using UnityEngine;

[System.Serializable]
public struct ADSREnvelope
{


    public float attackTime, decayTime, sustainLevel, releaseTime;
    
    public bool canInterrupt; //allows interuption of attack and decay

    private float time;
    private bool wasOnLastUpdate;
    private float currentAmplitude;
    private float ReleaseStartingAmplitude;
    private EnvelopeState previousState; // Stores the state before muting
    public EnvelopeState State { get; private set; }

    public float Time => time;

    public static ADSREnvelope Default()
    {
        return new ADSREnvelope()
        {
            attackTime = 1,
            decayTime = 1,
            sustainLevel = 0.5f,
            releaseTime = 1,
            State = EnvelopeState.Idle
        };
    }

    public void Mute()
    {
        if(State != EnvelopeState.Muted){
            // Store the current state and switch to Muted
            previousState = State;
            State = EnvelopeState.Muted;
            currentAmplitude = 0f; // Ensure silence while muted
        }

    }

    public void Unmute()
    {
        
        // Resume from the last state instead of resetting
        if (previousState != EnvelopeState.Muted && State == EnvelopeState.Muted)
        {
            State = previousState;
        }
    }

    public float Update(bool isOn, float dt)
    {
        if (State == EnvelopeState.Muted)
        {
            // Maintain silence if muted
            return 0f;
        }
        
        if (isOn != wasOnLastUpdate)
        {
            time = 0;
            if (isOn)
            {
                State = EnvelopeState.Attack;
            }
            else if (canInterrupt && State != EnvelopeState.Idle)
            {
                State = EnvelopeState.Release;
                ReleaseStartingAmplitude = currentAmplitude;  

            }
        }


        switch (State)
        {
            case EnvelopeState.Idle:
                currentAmplitude = 0f;
                break;

            case EnvelopeState.Attack:
                currentAmplitude = time / attackTime;
                if (time >= attackTime)
                {
                    State = EnvelopeState.Decay;
                    time = 0;
                }
                break;

            case EnvelopeState.Decay:
                currentAmplitude = Mathf.Lerp(1f, sustainLevel, time / decayTime);
                if (time >= decayTime)
                {
                    State = EnvelopeState.Sustain;
                    time = 0;
                }
                break;

            case EnvelopeState.Sustain:
                currentAmplitude = sustainLevel;
                break;

            case EnvelopeState.Release:

                // Use an exponential decay for a more natural amplitude reduction
                currentAmplitude = Mathf.Lerp(ReleaseStartingAmplitude, 0f, time / releaseTime);

                if (time >= releaseTime)
                {
                    State = EnvelopeState.Idle; // Transition to idle state
                    time = 0f;
                    currentAmplitude = 0f; // Ensure amplitude is fully zeroed out
                }
                break;

        }
        if (State != EnvelopeState.Idle)
        {
            time += dt;
        }

        wasOnLastUpdate = isOn;
        return Mathf.Min(currentAmplitude, 1f);
    }
    public bool IsComplete()
    {
        // Check if the envelope is in the Idle state, indicating completion
        return State == EnvelopeState.Idle;
    }
}
