using UnityEngine;

public class DeadPoseInstant : MonoBehaviour
{
    public float poseTime = 0.86f;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Animator kapalıyken state ayarla
        anim.enabled = true;
        anim.Play(0, 0, poseTime);
        anim.Update(0f);   // 🔥 ÇOK KRİTİK
        anim.speed = 0f;
    }
}

