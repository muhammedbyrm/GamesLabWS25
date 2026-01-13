using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 0.3f;
    public float magnitude = 0.2f;
    public float damping = 5f;

    Vector3 startPos;
    float timer;

    void Awake()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (timer > 0)
        {
            float fade = timer / duration;
            Vector3 offset = Random.insideUnitSphere * magnitude * fade;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos + offset,
                Time.deltaTime * damping
            );
            timer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos,
                Time.deltaTime * damping
            );
        }
    }

    public void Shake(float customDuration, float customMagnitude)
    {
        duration = customDuration;
        magnitude = customMagnitude;
        timer = duration;
    }
}
