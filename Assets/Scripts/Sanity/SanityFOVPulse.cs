using System.Collections;
using UnityEngine;

public class SanityFOVPulse : MonoBehaviour, ISanityEffect
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [Header("Pulse Settings")]
    [SerializeField] private float duration = 4f;
    [SerializeField] private float pulseSpeed = 2.5f;
    [SerializeField] private float pulseAmount = 15f;
    
    private float originalFOV;
    private Coroutine pulseCoroutine;

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera != null) originalFOV = targetCamera.fieldOfView;
    }

    public void TriggerEffect()
    {
        if (targetCamera == null) return;
        
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            
            float wave = Mathf.Sin(Time.time * pulseSpeed);
            
            // Apply the offset to the base FOV
            targetCamera.fieldOfView = originalFOV + (wave * pulseAmount);

            yield return null;
        }

        // Smoothly return to original FOV
        float resetTimer = 0f;
        float resetDuration = 1.5f;
        float currentFOV = targetCamera.fieldOfView;

        while (resetTimer < resetDuration)
        {
            resetTimer += Time.deltaTime;
            targetCamera.fieldOfView = Mathf.Lerp(currentFOV, originalFOV, resetTimer / resetDuration);
            yield return null;
        }

        targetCamera.fieldOfView = originalFOV;
    }
}