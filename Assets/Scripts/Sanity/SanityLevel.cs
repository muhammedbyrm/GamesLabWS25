using UnityEngine;


public enum VisualEffectType
{
    None,
    CameraDistorion,
    BloodStain,
    CameraSmearPlusHue,
    FOVPulse,
    BlurEffect
}

[CreateAssetMenu(fileName = "SanityEffect", menuName = "Scriptable Objects/SanityEffect")]
public class SanityLevel : ScriptableObject
{
    
    public string levelName;
    public AudioClip sanitySound;
    public VisualEffectType[] visualEffects;
    public bool actviated;

}
