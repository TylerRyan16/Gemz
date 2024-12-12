using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("References")]
    public OreSpawner oreSpawner;

    [Header("Ore Tracking")]
    private int maxOreCount;
    private int currentOreCount;

    [Header("Skybox Settings")]
    private Material skyboxInstance; // Runtime instance of the material
    private Color originalSkyboxColor;
    public Color yellowHue = new Color(1f, 0.8f, 0.3f);
    public float startExposure = 1.3f;
    public float endExposure = 0.5f;

    private void Start()
    {
        // Create a runtime instance of the skybox material
        skyboxInstance = Instantiate(RenderSettings.skybox);
        RenderSettings.skybox = skyboxInstance;

        if (skyboxInstance == null)
        {
            Debug.LogError("Skybox Material is not assigned!");
            return;
        }

        // Store the original color if the material has a '_Tint' property
        if (skyboxInstance.HasProperty("_Tint"))
        {
            originalSkyboxColor = skyboxInstance.GetColor("_Tint");
        }
        else
        {
            Debug.LogError("Skybox material does not have a '_Tint' property.");
        }

        // Initialize ore counts
        if (oreSpawner != null)
        {
            maxOreCount = oreSpawner.GetTotalOresOnMap();
            currentOreCount = maxOreCount;
        }
    }

    private void Update()
    {
        // Update the skybox properties dynamically
        UpdateSkybox();
        
        if (Input.GetKey(KeyCode.L))
        {
            DecrementCurrentOresOnMap();
        }

        if (Input.GetKey(KeyCode.P))
        {
            IncrementCurrentOresOnMap();
        }

      
    }

    public void UpdateOreCount(int newOreCount)
    {
        currentOreCount = Mathf.Clamp(newOreCount, 0, maxOreCount);
        UpdateSkybox();
    }

    private void UpdateSkybox()
    {
        if (skyboxInstance == null) return;

        // Calculate interpolation factor based on ore count
        float t = 1f - (float)currentOreCount / maxOreCount;

        // Interpolate between original and yellow hues
        Color currentColor = Color.Lerp(originalSkyboxColor, yellowHue, t);
        float currentExposure = Mathf.Lerp(startExposure, endExposure, t);

        // Apply the changes to the skybox instance (not the original material)
        skyboxInstance.SetColor("_Tint", currentColor);
        skyboxInstance.SetFloat("_Exposure", currentExposure);
    }

    public void DecrementCurrentOresOnMap()
    {
        currentOreCount--;
    }

    public void IncrementCurrentOresOnMap()
    {
        currentOreCount++;
    }

    public void SetMaxOreCount(int numOres)
    {
        maxOreCount = numOres;
    }

    public void SetCurrentOreCount(int current)
    {
        currentOreCount = current;
    }
}