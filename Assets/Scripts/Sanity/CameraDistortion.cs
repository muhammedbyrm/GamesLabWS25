using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraDistortion : MonoBehaviour, ISanityEffect
{
    private Volume volume;
    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    
    private float originalDistortion;
    private float originalVignetteIntensity;
    private float originalSaturation;

    [SerializeField] private Transform camTransform;
    [SerializeField] private float shakeAmount = 0.05f; // Decreased range (was 0.2f)

    private Vector3 currentShakeOffset;
    private Quaternion currentRotationOffset;
    private bool isEffectActive = false;
    private float effectIntensity = 0f;

    void Start()
    {
        if (volume == null) volume = GetComponent<Volume>();
        if (volume == null) return;

        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out vignette);

        if (colorAdjustments != null) originalSaturation = colorAdjustments.saturation.value;
        if (vignette != null) originalVignetteIntensity = vignette.intensity.value;
        if (lensDistortion != null) originalDistortion = lensDistortion.intensity.value;
    }

    void LateUpdate()
    {
        // If the game is paused (Time.timeScale == 0), don't update offsets
        if (Time.timeScale == 0f) return;

        if (!isEffectActive && effectIntensity <= 0) return;

        // Use Time.unscaledTime so the noise keeps "rolling" or Time.time if you want noise to freeze
        float noiseX = (Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f) * 2f;
        float noiseY = (Mathf.PerlinNoise(0f, Time.time * 0.5f) - 0.5f) * 2f;
        
        Vector3 targetShake = new Vector3(noiseX, noiseY, 0) * shakeAmount;
        
        currentShakeOffset = Vector3.Lerp(currentShakeOffset, targetShake * effectIntensity, Time.deltaTime * 2f);
        
        currentRotationOffset = Quaternion.Euler(
            Mathf.Sin(Time.time * 3f) * (0.5f * effectIntensity),
            Mathf.Cos(Time.time * 3f) * (0.5f * effectIntensity),
            Mathf.Sin(Time.time * 1.8f) * (0.8f * effectIntensity)
        );

        camTransform.localPosition += currentShakeOffset;
        camTransform.localRotation *= currentRotationOffset;
    }

    public void TriggerEffect()
    {
        StopAllCoroutines();
        StartCoroutine(RunEffectRoutine());
    }

    private IEnumerator RunEffectRoutine()
    {
        isEffectActive = true;
        float duration = 6f;
        float timer = 0f;

        if (lensDistortion != null) lensDistortion.intensity.value = -0.7f;
        if (vignette != null) vignette.intensity.value = 0.45f;

        while (timer < duration)
        {
            // Only progress the timer if the game isn't paused
            if (Time.timeScale > 0)
            {
                timer += Time.deltaTime;
                effectIntensity = 1f;

                if (colorAdjustments != null)
                {
                    float flicker = Mathf.PerlinNoise(Time.time * 4f, 0f);
                    colorAdjustments.saturation.value = Mathf.Lerp(0f, -100f, flicker * 0.5f);
                }
            }
            yield return null; 
        }

        isEffectActive = false;
        float resetDuration = 2f;
        float resetTimer = 0f;
        
        float startDistortion = lensDistortion != null ? lensDistortion.intensity.value : 0f;
        float startSaturation = colorAdjustments != null ? colorAdjustments.saturation.value : 0f;

        while (resetTimer < resetDuration)
        {
            if (Time.timeScale > 0)
            {
                resetTimer += Time.deltaTime;
                float t = resetTimer / resetDuration;

                effectIntensity = Mathf.Lerp(1f, 0f, t);

                if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(startDistortion, originalDistortion, t);
                if (colorAdjustments != null) colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, originalSaturation, t);
                if (vignette != null) vignette.intensity.value = Mathf.Lerp(0.45f, originalVignetteIntensity, t);
            }
            yield return null;
        }

        ForceResetVolume();
    }

    public void ForceResetVolume()
    {
        effectIntensity = 0f;
        currentShakeOffset = Vector3.zero;
        currentRotationOffset = Quaternion.identity;
        if (lensDistortion != null) lensDistortion.intensity.value = originalDistortion;
        if (colorAdjustments != null) colorAdjustments.saturation.value = originalSaturation;
        if (vignette != null) vignette.intensity.value = originalVignetteIntensity;
        
        // We DON'T set camTransform position here anymore, 
        // because effectIntensity = 0 handles the removal of the shake.
    }
}