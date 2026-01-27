using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [Header("Sanity Settings")]
    [SerializeField] private SanitySystemData sanityData;
    [SerializeField] public int currentSanity = 100;
    [SerializeField] public int sanityReduction = 10;
    [SerializeField] private int decreaseLevel = 3; 
    
    [Header("Effect Timing")]
    private const float MIN_EFFECT_WAIT_TIME = 4f; 
    private const float MAX_EFFECT_WAIT_TIME = 4f;
    private const float RANDOM_EFFECT_CHANCE = 0.8f;

    private int time_travel_attempts = 0;
    private SanityLevel currentLevel;
    private Coroutine effectLoopCoroutine;

    private Dictionary<VisualEffectType, ISanityEffect> effectLookup = new Dictionary<VisualEffectType, ISanityEffect>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (sanityData != null)
        {
            sanityData.ResetLevels();
        }

        RegisterEffects();
    }
    
    private void RegisterEffects()
    {
        // Add new effects HERE
        effectLookup[VisualEffectType.CameraDistorion] = GetComponent<CameraDistortion>();
        effectLookup[VisualEffectType.CameraSmearPlusHue] = GetComponent<CameraSmearPlusHueEffect>();
        effectLookup[VisualEffectType.FOVPulse] = GetComponent<SanityFOVPulse>();
        effectLookup[VisualEffectType.BlurEffect] = GetComponent<SanityBlurEffect>();

    }
    
    
    public void UpdateSanityEffects()
    {
        currentSanity = Mathf.Clamp(currentSanity - sanityReduction, 0, 100);
    
        time_travel_attempts += 1;
        Debug.Log($"Sanity: {currentSanity} | Attempt: {time_travel_attempts}/{decreaseLevel}");

        if (time_travel_attempts >= decreaseLevel)
        {
            time_travel_attempts = 0; 
            SanityLevel newLevel = sanityData.GetLevel(); 

            if (newLevel != currentLevel)
            {
                currentLevel = newLevel;
                Debug.Log($"--- LEVEL SHIFTED TO: {currentLevel.levelName} ---");
                ApplyLevelChanges(currentLevel);
            }
        }
    }
    
    private void ApplyLevelChanges(SanityLevel level)
    {
        if (level == null) return;

        // Play the one-time sound for entering a new sanity state
        if (level.sanitySound != null)
            AudioSource.PlayClipAtPoint(level.sanitySound, Camera.main.transform.position);

        // Restart the loop with the new list of possible effects
        if (effectLoopCoroutine != null) StopCoroutine(effectLoopCoroutine);
        effectLoopCoroutine = StartCoroutine(LoopSanityEffects(level.visualEffects));
    }

    private IEnumerator LoopSanityEffects(VisualEffectType[] allowedEffects)
    {
        if (allowedEffects == null || allowedEffects.Length == 0) yield break;

        while (true)
        {
            float waitTime = Random.Range(MIN_EFFECT_WAIT_TIME, MAX_EFFECT_WAIT_TIME);
            yield return new WaitForSeconds(waitTime);

            if (Random.value <= RANDOM_EFFECT_CHANCE)
            {
                // Pick a random effect from the current level's allowed list
                //VisualEffectType randomType = allowedEffects[Random.Range(0, allowedEffects.Length)];
                // testing the camera distortion
                VisualEffectType randomType = VisualEffectType.BlurEffect;
                if (effectLookup.TryGetValue(randomType, out ISanityEffect effect))
                {
                    Debug.Log("Applied effect: " + randomType.ToString());
                    effect?.TriggerEffect();
                }
            }
        }
    }
}