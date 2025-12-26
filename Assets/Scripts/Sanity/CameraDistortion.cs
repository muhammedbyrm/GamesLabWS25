using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraDistortion : MonoBehaviour, ISanityEffect
{
    private Volume volume;
    
    // Explicitly use URP versions to avoid ambiguity
    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    
    private float originalDistortion;
    private float originalVignetteIntensity;
    private float originalSaturation;

    [SerializeField] private Transform camTransform;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    void Start()
    {
        // 1. Try to find the volume on this object if it wasn't set manually
        if (volume == null) volume = GetComponent<Volume>();

        if (volume == null)
        {
            Debug.LogError("No Volume component found on " + gameObject.name);
            return;
        }

        // 3. Get the specific effects from the URP profile
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out vignette);

        // 4. Capture defaults
        if (lensDistortion != null) originalDistortion = lensDistortion.intensity.value;
        if (colorAdjustments != null) 
        {
            originalSaturation = colorAdjustments.saturation.value;
        }
        if (vignette != null) originalVignetteIntensity = vignette.intensity.value;
        
        // 5. Capture camera defaults
        if (camTransform != null)
        {
            originalCamPos = camTransform.localPosition;
            originalCamRot = camTransform.localRotation;
        }
    }

    public void TriggerEffect()
    {
        if (camTransform == null) return;
        StopAllCoroutines();
        StartCoroutine(ApplyEffect());
    }

    private IEnumerator ApplyEffect()
    {
        float duration = 8f;
        float timer = 0f;

        // Set initial "insanity" values
        if (lensDistortion != null) lensDistortion.intensity.value = -0.7f;
        if (vignette != null) vignette.intensity.value = 0.45f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            
            if (colorAdjustments != null)
            {
                float flicker = Mathf.PerlinNoise(Time.time * 4f, 0f);
                colorAdjustments.saturation.value = Mathf.Lerp(0f, -100f, flicker * 0.5f);
            }

            // Smoother "Perlin" Drift
            float noiseX = (Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(0f, Time.time * 0.5f) - 0.5f) * 2f;
            
            
            // driftPos is the next position the camera moves to
            Vector3 driftPos = originalCamPos + new Vector3(noiseX, noiseY, 0) * 0.2f;
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, driftPos, Time.deltaTime * 2f);
            
            camTransform.localRotation = originalCamRot * Quaternion.Euler(
                Mathf.Sin(Time.time * 3f) * 0.7f,
                Mathf.Cos(Time.time * 3f) * 0.7f,
                Mathf.Sin(Time.time * 1.8f) * 1f
            );

            yield return null;
        }

        // Smooth Reset
        float resetDuration = 2f;
        float resetTimer = 0f;
        float startDistortion = lensDistortion != null ? lensDistortion.intensity.value : 0f;
        float startSaturation = colorAdjustments != null ? colorAdjustments.saturation.value : 0f;

        while (resetTimer < resetDuration)
        {
            resetTimer += Time.deltaTime;
            float t = resetTimer / resetDuration;

            if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(startDistortion, originalDistortion, t);
            if (colorAdjustments != null) colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, originalSaturation, t);
            if (vignette != null) vignette.intensity.value = Mathf.Lerp(0.45f, originalVignetteIntensity, t);

            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalCamPos, t);
            camTransform.localRotation = Quaternion.Slerp(camTransform.localRotation, originalCamRot, t);

            yield return null;
        }

        ForceResetVolume();
    }

    public void ForceResetVolume()
    {
        if (lensDistortion != null) lensDistortion.intensity.value = originalDistortion;
        if (colorAdjustments != null) colorAdjustments.saturation.value = originalSaturation;
        if (vignette != null) vignette.intensity.value = originalVignetteIntensity;
        if (camTransform != null)
        {
            camTransform.localPosition = originalCamPos;
            camTransform.localRotation = originalCamRot;
        }
    }

}