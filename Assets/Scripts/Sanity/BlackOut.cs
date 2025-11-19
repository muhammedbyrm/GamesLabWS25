using UnityEngine;

public class BlackOut : MonoBehaviour
{
    public GameObject blackoutPanel;

    void Start()
    {
        blackoutPanel.SetActive(false); // hide at start
    }

    public void ToggleBlackout(bool on)
    {
        blackoutPanel.SetActive(on);
    }

}
