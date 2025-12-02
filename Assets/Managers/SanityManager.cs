using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public class SanityManager : MonoBehaviour

{
    private const float MIN_EFFECT_WAIT_TIME = 1f; // 8
    private const float MAX_EFFECT_WAIT_TIME = 1f; //12
    private const float RANDOM_EFFECT_CHANCE = 0.8f;
    private const int decrease_level = 3;
    private int time_travel_attempts = 0;
    
    
    public static SanityManager Instance { get; private set; }

    [SerializeField] private SanitySystemData sanityData;
    // changed to public so player script has access
    [SerializeField] public int currentSanity = 100;
    [SerializeField] public int sanityReduction = 10;
    [SerializeField] private Volume _volume;
    
    private SanityLevel currentLevel;
    
    [SerializeField]
    private CameraDistortiion cameraDistortion;
    [SerializeField] private CameraSmearPlusHueEffect hueEffect;
    private Coroutine effectLoopCoroutine;
    private VisualEffectType _current;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start() {
        //cameraDistortion = gameObject.AddComponent<CameraDistortiion>();
        cameraDistortion.SetVolume(_volume);
    }
    
    
    void Update() {
        
    }
    
  
    
    public void UpdateSanityEffects()
    {

        time_travel_attempts += 1;
        Debug.Log("Traveled " + time_travel_attempts);

        if (time_travel_attempts == decrease_level)
        {
            currentLevel = sanityData.GetLevel();
            time_travel_attempts = 0;
        }
        
        if (currentLevel != null)
        {
            Debug.Log(currentLevel.levelName);
            currentSanity = Mathf.Clamp(currentSanity - sanityReduction, 0, 100);
            Debug.Log("Sanity: " + currentSanity);
            ApplyEffect(currentLevel);
        }
        
    }   
    
    private void ApplyEffect(SanityLevel level)
    {
        if (effectLoopCoroutine != null) return; // Prevent stacking multiple coroutines
        if (level.sanitySound != null)
            AudioSource.PlayClipAtPoint(level.sanitySound, Camera.main.transform.position);
        StopAllCoroutines();
        Debug.Log("Before entering loop effects");
        effectLoopCoroutine = StartCoroutine(LoopSanityEffects(level.visualEffects));
        
    }

    private IEnumerator LoopSanityEffects(VisualEffectType[] effects)
    {
        Debug.Log("Entering loop effects");

        while (true)
        {
            Debug.Log("Looping Sanity Effects");
            float waitTime = Random.Range(MIN_EFFECT_WAIT_TIME, MAX_EFFECT_WAIT_TIME);
            float random = 0; //Random.value;
            Debug.Log("Random value: " + random + " Wait time: " + waitTime + " seconds");
            // 70% chance to apply effect
            if (random <= RANDOM_EFFECT_CHANCE)
            {
                Debug.Log("Inside!");
                var randomEffect = effects[Random.Range(0, effects.Length)];

                switch (randomEffect)
                {
                    case VisualEffectType.ScreenBlur:
                        Debug.Log("Apply Camera Shake");
                        cameraDistortion.TriggerEffect();
                        yield return new WaitForSeconds(waitTime + 2f); // Wait a bit longer for camera distortion to finish
                        break;
                    case VisualEffectType.BloodStain:
                        Debug.Log("Apply Blood Stain");
                        yield return new WaitForSeconds(waitTime);
                        break;
                    case VisualEffectType.CameraSmearPlusHue:
                        Debug.Log("Apply Hue");
                        hueEffect.TriggerEffect();
                        yield return new WaitForSeconds(waitTime);
                        break;
                }
            }
            else
            {
                Debug.Log("No effect applied this time");
                yield return new WaitForSeconds(waitTime - 4f);  
            }
        }

        // Stop coroutine when boost is active again
        effectLoopCoroutine = null;
    }
}