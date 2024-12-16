using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSynthAbility", menuName = "Synth/Ability", order = 1)]
public class SynthAbility : ScriptableObject
{
    [Header("Ability Properties")]
    public ESynthType synthType;      // Type of synth (Bass, Lead, etc.)
    public float damage;             // Damage dealt by this ability
    public float hitstop = 0.1f;
    public GameObject owner;         // Reference to the owner of the ability

    [Header("Synth Properties")]
    public FMSynth synthPrefab;      // Prefab for the synth to play
    public float abilityDuration;    // Duration for which the ability lasts

    [Header("Attack Properties")]
    public GameObject projectilePrefab; // Prefab for the projectile
    public bool useProjectile;          // Whether to use a projectile for the attack
    public bool isTracking;             // Whether the projectile tracks a target
    public string targetTag;            // The tag of the target (e.g., "Player" or "Enemy")
    public float projectileSpeed = 5f;  // Speed of the projectile
    public float lifetime = 4f;
    private FMSynth activeSynth;        // Instance of the synth currently playing
    private Coroutine endAbilityCoroutine; // Reference to the EndAbility coroutine

    public Animator animator;
    public AnimationClip animationClip;
    public string animationName;
    public int animationLayer;

    public void TryAbility()
    {
        if (owner == null)
        {
            Debug.LogError("Owner is not assigned for this ability!");
            return;
        }

        if (useProjectile)
        {
            SpawnProjectile();
        }
        else
        {
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
            Debug.Log("anim "+ animationName);
            animator.Play(animationName, animationLayer, 0);
            // Play the synth
            activeSynth.NoteOn();

            // Start the ability duration coroutine
            if (abilityDuration > 0)
            {
                MonoBehaviour ownerMono = owner.GetComponent<MonoBehaviour>();
                if (ownerMono != null)
                {
                    endAbilityCoroutine = ownerMono.StartCoroutine(EndAbilityAfterDuration(abilityDuration));
                }
                else
                {
                    Debug.LogError("Owner does not have a MonoBehaviour component to start coroutines!");
                }
            }
        }
    }

    private void SpawnProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned for this ability!");
            return;
        }

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, owner.transform.position, Quaternion.identity);

        // Set the projectile's direction
        Vector3 direction = owner.transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        // If tracking is enabled, find the target and set the tracking behavior
        if (isTracking && !string.IsNullOrEmpty(targetTag))
        {
            GameObject target = FindClosestTargetWithTag(targetTag);
            if (target != null)
            {
                SynthAttack synthAttack = projectile.GetComponent<SynthAttack>();
                if (synthAttack != null)
                {
                    synthAttack.synthAbility = this;
                    synthAttack.TrackTarget(target);
                }
            }
        }
    }

    private GameObject FindClosestTargetWithTag(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(owner.transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
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
                if (activeSynth.operators != null && activeSynth.operators.Length > 0)
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
        }
    }

    private IEnumerator EndAbilityAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndAbility();
    }
}
