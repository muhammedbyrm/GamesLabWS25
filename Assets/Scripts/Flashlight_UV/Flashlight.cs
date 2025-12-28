using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Material uvMaterial;
    public Material uvMaterial_Object;
    public Material uvMaterial_Text;
    public Material uvMaterial_Seven;
    public Light flashlightSpot;
    private bool isOn = false;

    private float toggle;
    private Vector3 lichtPosition;
    private Vector3 lichtDirection;
    private float toggle_Object;
    private Vector3 lichtPosition_Object;
    private Vector3 lichtDirection_Object;
    private float toggle_Text;
    private Vector3 lichtPosition_Text;
    private Vector3 lichtDirection_Text;
    private float toggle_Seven;
    private Vector3 lichtPosition_Seven;
    private Vector3 lichtDirection_Seven;

    private void Update()
    {
        uvMaterial.SetVector("_LichtPosition", flashlightSpot.transform.position);
        uvMaterial.SetVector("_LichtDirection", flashlightSpot.transform.forward);
        //uvMaterial.SetFloat("_FlashlightAngle", flashlightSpot.spotAngle);
        uvMaterial_Object.SetVector("_LichtPosition", flashlightSpot.transform.position);
        uvMaterial_Object.SetVector("_LichtDirection", flashlightSpot.transform.forward);

        uvMaterial_Text.SetVector("_LichtPosition", flashlightSpot.transform.position);
        uvMaterial_Text.SetVector("_LichtDirection", flashlightSpot.transform.forward);

        uvMaterial_Seven.SetVector("_LichtPosition", flashlightSpot.transform.position);
        uvMaterial_Seven.SetVector("_LichtDirection", flashlightSpot.transform.forward);
    }

    public void toggleFlashlight()
    {
        isOn = !isOn;

        if (isOn)
        {
            uvMaterial.SetFloat("_isActivated", 1f);
            uvMaterial_Object.SetFloat("_isActivated", 1f);
            uvMaterial_Text.SetFloat("_isActivated", 1f);
            uvMaterial_Seven.SetFloat("_isActivated", 1f);
        }
        else
        {
            uvMaterial.SetFloat("_isActivated", 0f);
            uvMaterial_Object.SetFloat("_isActivated", 0f);
            uvMaterial_Text.SetFloat("_isActivated", 0f);
            uvMaterial_Seven.SetFloat("_isActivated", 0f);
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

        toggle_Text = uvMaterial_Text.GetFloat("_isActivated");
        lichtDirection_Text = uvMaterial_Text.GetVector("_LichtDirection");
        lichtPosition_Text = uvMaterial_Text.GetVector("_LichtPosition");

        toggle_Seven = uvMaterial_Seven.GetFloat("_isActivated");
        lichtDirection_Seven = uvMaterial_Seven.GetVector("_LichtDirection");
        lichtPosition_Seven = uvMaterial_Seven.GetVector("_LichtPosition");
    }
    private void OnDisable()
    {
        uvMaterial.SetVector("_LichtPosition", lichtPosition);
        uvMaterial.SetVector("_LichtDirection", lichtDirection);
        uvMaterial.SetFloat("_isActivated", toggle);

        uvMaterial_Object.SetVector("_LichtPosition", lichtPosition_Object);
        uvMaterial_Object.SetVector("_LichtDirection", lichtDirection_Object);
        uvMaterial_Object.SetFloat("_isActivated", toggle_Object);

        uvMaterial_Text.SetVector("_LichtPosition", lichtPosition_Text);
        uvMaterial_Text.SetVector("_LichtDirection", lichtDirection_Text);
        uvMaterial_Text.SetFloat("_isActivated", toggle_Text);

        uvMaterial_Seven.SetVector("_LichtPosition", lichtPosition_Seven);
        uvMaterial_Seven.SetVector("_LichtDirection", lichtDirection_Seven);
        uvMaterial_Seven.SetFloat("_isActivated", toggle_Seven);
    }
}
