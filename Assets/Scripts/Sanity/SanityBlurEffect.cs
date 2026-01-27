using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanityBlurEffect : MonoBehaviour, ISanityEffect
{
    private Volume volume;
    private DepthOfField depthOfField;

    [Header("Blur Settings")]
    [SerializeField] private float holdDuration = 2f;       
    [SerializeField] private float transitionTime = 2f;    
    [SerializeField] private float blurFocusDistance = 0.1f; 
    [SerializeField] private float intensityFocalLength = 100f; 

    private float originalFocusDist;
    private float originalFocalLength;

    void Start()
    {
        volume = GetComponent<Volume>();
    
        if (volume != null && volume.profile.TryGet(out depthOfField))
        {
            originalFocusDist = depthOfField.focusDistance.value;
            originalFocalLength = depthOfField.focalLength.value;
        }
    }

    public void TriggerEffect()
    {
        if (depthOfField == null) return;
        StopAllCoroutines();
        StartCoroutine(BlurRoutine());
    }

    private IEnumerator BlurRoutine()
    {
        // 1. Fade Into Blur
        float timer = 0f;
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float progress = timer / transitionTime; 
            
            depthOfField.focusDistance.value = Mathf.Lerp(originalFocusDist, blurFocusDistance, progress);
            depthOfField.focalLength.value = Mathf.Lerp(originalFocalLength, intensityFocalLength, progress);
            yield return null;
        }

        // 2. Hold the Blur
        yield return new WaitForSeconds(holdDuration);

        // 3. Smooth Reset
        timer = 0f;
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float progress = timer / transitionTime;
            
            depthOfField.focusDistance.value = Mathf.Lerp(blurFocusDistance, originalFocusDist, progress);
            depthOfField.focalLength.value = Mathf.Lerp(intensityFocalLength, originalFocalLength, progress);
            yield return null;
        }

        depthOfField.focusDistance.value = originalFocusDist;
        depthOfField.focalLength.value = originalFocalLength;
    }
}