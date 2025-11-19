using UnityEngine;


public enum VisualEffectType
{
    None,
    ScreenBlur,
    BloodStain,
    CameraSmearPlusHue
}

[CreateAssetMenu(fileName = "SanityEffect", menuName = "Scriptable Objects/SanityEffect")]
public class SanityLevel : ScriptableObject
{
    
    public string levelName;
    public int sanityExtraction; // How much sanity must be extracted
    public AudioClip sanitySound;
    public VisualEffectType[] visualEffects;
    public bool actviated;

}
