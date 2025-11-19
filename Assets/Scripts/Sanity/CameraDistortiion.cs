using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ChromaticAberration = UnityEngine.Rendering.Universal.ChromaticAberration;
using Vignette = UnityEngine.Rendering.Universal.Vignette;

public class CameraDistortiion : MonoBehaviour
{
    private Volume volume;
    private UnityEngine.Rendering.Universal.LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;
    private float originalIntensity;
    private UnityEngine.Rendering.Universal.ChromaticAberration chromaticAberration;
    private UnityEngine.Rendering.Universal.Vignette vignette;
    
    private float originalDistortion;
    private Color originalColorFilter;
    private float originalAberrationIntensity;
    private float originalVignetteIntensity;
    
    [SerializeField]
    private Transform camTransform;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    
    private float originalSaturation;
    

   void Start()
    {
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out vignette);

        if (lensDistortion != null) originalDistortion = lensDistortion.intensity.value;
        if (colorAdjustments != null) originalColorFilter = colorAdjustments.colorFilter.value;
        if (chromaticAberration != null) originalAberrationIntensity = chromaticAberration.intensity.value;
        if (vignette != null) originalVignetteIntensity = vignette.intensity.value;
        
        if (colorAdjustments != null)
            originalSaturation = colorAdjustments.saturation.value;
        
        originalCamPos = camTransform.localPosition;
        originalCamRot = camTransform.localRotation;
    }

   
    public void TriggerEffect()
    {
        StopAllCoroutines();
        StartCoroutine(ApplyEffect());
    }

    private IEnumerator ApplyEffect()
    {
        float duration = 8f;
        float timer = 0f;

        if (lensDistortion != null)
        {
            lensDistortion.active = true;
            lensDistortion.intensity.value = -0.7f;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.active = true;
            //colorAdjustments.colorFilter.value = Color.red;
        }
        

        if (vignette != null)
        {
            vignette.active = true;
            vignette.intensity.value = 0.45f;
        }

        // Shake effect
        while (timer < duration)
        {
            timer += Time.deltaTime;
            
            if (colorAdjustments != null)
            {
                float flicker = Mathf.PerlinNoise(Time.time * 4f, 0f); // Smooth randomness
    
                // Flicker between normal saturation and black & white (-100)
                // Default is usually 0, meaning no change
                colorAdjustments.saturation.value = Mathf.Lerp(0f, -100f, flicker * 0.5f); // up to -50% saturation
            }

            // Randomized camera movement and rotation
            camTransform.localPosition = originalCamPos + Random.insideUnitSphere * 0.05f;
            camTransform.localRotation = originalCamRot * Quaternion.Euler(
                Mathf.Sin(Time.time * 10f) * 1.5f,
                Mathf.Cos(Time.time * 10f) * 1.5f,
                Mathf.Sin(Time.time * 8f) * 2f
            );

            yield return null;
        }

        // Phase 2: Smooth Reset
        float resetDuration = 2f;
        float resetTimer = 0f;

        // Cache starting values
        float startDistortion = lensDistortion != null ? lensDistortion.intensity.value : 0f;
        float startSaturation = colorAdjustments != null ? colorAdjustments.saturation.value : 0f;
        float startVignette = vignette != null ? vignette.intensity.value : 0f;

        while (resetTimer < resetDuration)
        {
            resetTimer += Time.deltaTime;
            float t = resetTimer / resetDuration;

            if (lensDistortion != null)
                lensDistortion.intensity.value = Mathf.Lerp(startDistortion, originalDistortion, t);

            if (colorAdjustments != null)
                colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, originalSaturation, t);

            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(1f, originalAberrationIntensity, t);

            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(startVignette, originalVignetteIntensity, t);

            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalCamPos, t);
            camTransform.localRotation = Quaternion.Slerp(camTransform.localRotation, originalCamRot, t);

            yield return null;
        }

        // Ensure exact reset
        if (lensDistortion != null) lensDistortion.intensity.value = originalDistortion;
        if (colorAdjustments != null) colorAdjustments.saturation.value = originalSaturation;
        if (chromaticAberration != null) chromaticAberration.intensity.value = originalAberrationIntensity;
        if (vignette != null) vignette.intensity.value = originalVignetteIntensity;

        camTransform.localPosition = originalCamPos;
        camTransform.localRotation = originalCamRot;
    }

    public void SetVolume(Volume newVolume)
    {
        this.volume = newVolume;
    }

    public void ForceResetVolume()
    {
        if (lensDistortion != null) lensDistortion.intensity.value = originalDistortion;
        if (colorAdjustments != null) colorAdjustments.saturation.value = originalSaturation;
        if (chromaticAberration != null) chromaticAberration.intensity.value = originalAberrationIntensity;
        if (vignette != null) vignette.intensity.value = originalVignetteIntensity;

        camTransform.localPosition = originalCamPos;
        camTransform.localRotation = originalCamRot;   
    }
    
}
