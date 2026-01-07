using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log("Screenshot gespeichert: " + filename);
        }
    }
}
