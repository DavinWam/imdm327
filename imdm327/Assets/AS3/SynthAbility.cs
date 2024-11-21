using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSynthAbility", menuName = "Synth/Ability", order = 1)]
public class SynthAbility : ScriptableObject
{
    [Header("Ability Properties")]
    public ESynthType synthType;      // Type of synth (Bass, Lead, etc.)
    public float damage;             // Damage dealt by this ability
    public GameObject owner;         // Reference to the owner of the ability

    [Header("Synth Properties")]
    public FMSynth synthPrefab;      // Prefab for the synth to play
    public float abilityDuration;    // Duration for which the ability lasts

    private FMSynth activeSynth;     // Instance of the synth currently playing
    private Coroutine endAbilityCoroutine; // Reference to the EndAbility coroutine
    public Animator animator;
    public AnimationClip animationClip;
    public String animation;

    public void TryAbility()
    {
        if (owner == null)
        {
            Debug.LogError("Owner is not assigned for this ability!");
            return;
        }

        if (synthPrefab == null)
        {
            Debug.LogError("Synth Prefab is not assigned for this ability!");
            return;
        }

        // End any currently active synth to avoid overlapping abilities
        if (activeSynth != null)
        {
            EndAbility();
        }

        // Spawn the synth
        activeSynth = Instantiate(synthPrefab, owner.transform.position, Quaternion.identity);
        activeSynth.transform.SetParent(owner.transform);

        // Register the synth with the sound manager
        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.RegisterSynth(activeSynth, true);
            activeSynth.soundManager = soundManager;
        }

        animator.Play("BassAbility",0,0);
        // Play the synth
        activeSynth.NoteOn();

        // Start the ability duration coroutine
        if (abilityDuration > 0)
        {
            MonoBehaviour ownerMono = owner.GetComponent<MonoBehaviour>();
            if (ownerMono != null)
            {
                //  Debug.Log($"{this.name} start end {abilityDuration}");
                endAbilityCoroutine = ownerMono.StartCoroutine(EndAbilityAfterDuration(abilityDuration));
            }
            else
            {
                Debug.LogError("Owner does not have a MonoBehaviour component to start coroutines!");
            }
        }

        // Debug.Log($"{synthType} ability activated by {owner.name}!");
    }

    public void EndAbility()
    {
        if (activeSynth != null)
        {
            // Stop the active coroutine if it's running
            if (endAbilityCoroutine != null)
            {
                owner.GetComponent<MonoBehaviour>().StopCoroutine(endAbilityCoroutine);
                endAbilityCoroutine = null;
            }

            if (activeSynth.noteOn)
            {
                activeSynth.NoteOff();
                if (activeSynth.operators != null && activeSynth.operators.Length > 0 )
                {
                    Destroy(activeSynth.gameObject, activeSynth.operators[0].envelope.releaseTime);
                }
                else
                {
                    Destroy(activeSynth.gameObject);
                }
            }
            else
            {
                Destroy(activeSynth.gameObject);
            }

            activeSynth = null;
            // Debug.Log($"{synthType} ability ended.");
        }
    }

    private IEnumerator EndAbilityAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndAbility();
    }
}
