using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SanitySystemData", menuName = "Scriptable Objects/SanitySystemData")]
public class SanitySystemData : ScriptableObject

{

public SanityLevel[] sanityLevels;


// will point to the next level to hand out
private int currentLevelIndex;


private void OnEnable()

{
    // reset index whenever the asset is (re)loaded
    currentLevelIndex = sanityLevels.Length - 1;
    
}


public SanityLevel GetLevel()
{
    // if we've run out of levels, return null
    if (currentLevelIndex < 0)
        return null;
    return sanityLevels[currentLevelIndex--];;
}

}
