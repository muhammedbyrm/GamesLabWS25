using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Material uvMaterial;
    public Material uvMaterial_Object;
    public Light flashlightSpot;
    private bool isOn = false;

    private float toggle;
    private Vector3 lichtPosition;
    private Vector3 lichtDirection;
    private float toggle_Object;
    private Vector3 lichtPosition_Object;
    private Vector3 lichtDirection_Object;

    private void Update()
    {
        uvMaterial.SetVector("_LichtPosition", flashlightSpot.transform.position);
        uvMaterial.SetVector("_LichtDirection", flashlightSpot.transform.forward);
        //uvMaterial.SetFloat("_FlashlightAngle", flashlightSpot.spotAngle);
        uvMaterial_Object.SetVector("_LichtPosition", flashlightSpot.transform.position);
        uvMaterial_Object.SetVector("_LichtDirection", flashlightSpot.transform.forward);
    }

    public void toggleFlashlight()
    {
        isOn = !isOn;

        if (isOn)
        {
            uvMaterial.SetFloat("_isActivated", 1f);
            uvMaterial_Object.SetFloat("_isActivated", 1f);
        }
        else
        {
            uvMaterial.SetFloat("_isActivated", 0f);
            uvMaterial_Object.SetFloat("_isActivated", 1f);
        }
    }

    private void OnEnable()
    {
        toggle = uvMaterial.GetFloat("_isActivated");
        lichtDirection = uvMaterial.GetVector("_LichtDirection");
        lichtPosition = uvMaterial.GetVector("_LichtPosition");

        toggle_Object = uvMaterial_Object.GetFloat("_isActivated");
        lichtDirection_Object = uvMaterial_Object.GetVector("_LichtDirection");
        lichtPosition_Object = uvMaterial_Object.GetVector("_LichtPosition");
    }
    private void OnDisable()
    {
        uvMaterial.SetVector("_LichtPosition", lichtPosition);
        uvMaterial.SetVector("_LichtDirection", lichtDirection);
        uvMaterial.SetFloat("_isActivated", toggle);

        uvMaterial_Object.SetVector("_LichtPosition", lichtPosition_Object);
        uvMaterial_Object.SetVector("_LichtDirection", lichtDirection_Object);
        uvMaterial_Object.SetFloat("_isActivated", toggle_Object);
    }
}
