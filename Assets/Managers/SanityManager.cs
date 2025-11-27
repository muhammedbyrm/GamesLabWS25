using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public class SanityManager : MonoBehaviour

{
    private const float MIN_EFFECT_WAIT_TIME = 8f;
    private const float MAX_EFFECT_WAIT_TIME = 12f;
    private const float RANDOM_EFFECT_CHANCE = 0.8f;
    private bool DONT_SET_TO_TRUE = false;
    
    public static SanityManager Instance { get; private set; }

    [SerializeField] private SanitySystemData sanityData;
    // changed to public so player script has access
    [SerializeField] public int currentSanity = 100;
    [SerializeField] public int sanityReduction = 10;
    [SerializeField] private Volume _volume;
    
    private SanityLevel currentEffect;
    
    private bool prevent = true;     // check to allow one injection at a time
    [SerializeField]
    private CameraDistortiion cameraDistortion;
    private CameraSmearPlusHueEffect hueEffect;
    private Coroutine effectLoopCoroutine;
    private bool boosted = false;
    private VisualEffectType _current;

    private bool noSerum = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start() {
        //cameraDistortion = gameObject.AddComponent<CameraDistortiion>();
        cameraDistortion.SetVolume(_volume);
        hueEffect = gameObject.AddComponent<CameraSmearPlusHueEffect>();
    }
    
    
    void Update() {
        
    }
    
  
    
    public void UpdateSanityEffects()  {
        
        
        // TODO REquires rework, it made use of boost manager which we dont use in this project 
        currentEffect = sanityData.GetLevel();
        
        if (currentEffect != null)
        {
            Debug.Log(currentEffect.levelName);
            // changed here so that its minus
            currentSanity = Mathf.Clamp(currentSanity - sanityReduction, 0, 100);
            Debug.Log("Sanity: " + currentSanity);
            ApplyEffect(currentEffect);
        }
        
    }   
    
    private void ApplyEffect(SanityLevel level)
    {
        if (effectLoopCoroutine != null) return; // Prevent stacking multiple coroutines
        if (level.sanitySound != null)
            AudioSource.PlayClipAtPoint(level.sanitySound, Camera.main.transform.position);
        StopAllCoroutines();
        effectLoopCoroutine = StartCoroutine(LoopSanityEffects(level.visualEffects));
        
    }

    private IEnumerator LoopSanityEffects(VisualEffectType[] effects)
    {
        while (DONT_SET_TO_TRUE)
        {
            Debug.Log("Looping Sanity Effects");
            float waitTime = Random.Range(MIN_EFFECT_WAIT_TIME, MAX_EFFECT_WAIT_TIME);
            float random = Random.value;
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