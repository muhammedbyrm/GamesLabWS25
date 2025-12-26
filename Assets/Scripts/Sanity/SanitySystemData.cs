using UnityEngine;
[CreateAssetMenu(fileName = "SanitySystemData", menuName = "Scriptable Objects/SanitySystemData")]
public class SanitySystemData : ScriptableObject
{
    public SanityLevel[] sanityLevels;
    private int currentLevelIndex;

    public void ResetLevels()
    {
        currentLevelIndex = sanityLevels.Length - 1;
    }

    public SanityLevel GetLevel()
    {
        if (sanityLevels == null || sanityLevels.Length == 0) return null;

        SanityLevel levelToReturn = sanityLevels[currentLevelIndex];

        if (currentLevelIndex > 0)
        {
            currentLevelIndex--;
        }

        return levelToReturn;
    }
}