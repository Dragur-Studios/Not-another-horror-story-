using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class DynamicDepthOfField : MonoBehaviour
{
    [Header("General Settings")]
    public Volume volume;
    public float smoothSpeed = 8f;
    public float maxGaussianRadius = 1.5f;
    public float gaussianThreshold = 6f;

    [Header("Bokeh Controls")]
    [Range(1f, 16f)] public float bokehAperture = 3.5f; // lower = stronger blur
    [Range(1f, 300f)] public float bokehFocalLength = 50f;
    [Range(3, 11)] public int bokehBladeCount = 5;
    [Range(0f, 1f)] public float bokehBladeCurvature = 0.5f;
    [Range(0f, 360f)] public float bokehBladeRotation = 0f;
    public float focusOffset = 0f; // bias focus distance forward/back
    public float snapSpeed = 12f;  // how quickly focus reacts

    private Camera cam;
    private DepthOfField dof;
    private float currentFocusDist;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (!volume.profile.TryGet(out dof))
            Debug.LogError("DepthOfField override not found in Volume profile.");

        cam.depthTextureMode |= DepthTextureMode.Depth;
    }

    void Update()
    {
        if (dof == null) return;

        float depth01 = SampleCenterDepth();
        if (depth01 <= 0f) return;

        float dist = DepthToWorldDistance(depth01) + focusOffset;

        // Snap quickly toward target, then smooth settle
        currentFocusDist = Mathf.Lerp(currentFocusDist, dist, Time.deltaTime * snapSpeed);
        float smoothedFocus = Mathf.Lerp(dof.focusDistance.value, currentFocusDist, Time.deltaTime * smoothSpeed);

        if (smoothedFocus <= gaussianThreshold)
        {
            // --- Bokeh Mode ---
            dof.mode.value = DepthOfFieldMode.Bokeh;
            dof.focusDistance.value = smoothedFocus;
            dof.aperture.value = bokehAperture;
            dof.focalLength.value = bokehFocalLength;
            dof.bladeCount.value = bokehBladeCount;
            dof.bladeCurvature.value = bokehBladeCurvature;
            dof.bladeRotation.value = bokehBladeRotation;
        }
        else
        {
            // --- Gaussian Mode ---
            dof.mode.value = DepthOfFieldMode.Gaussian;
            float t = Mathf.InverseLerp(gaussianThreshold, 20f, smoothedFocus);
            float targetRadius = Mathf.Lerp(maxGaussianRadius, 0.1f, t);

            dof.gaussianStart.value = Mathf.Lerp(dof.gaussianStart.value, smoothedFocus - 1f, Time.deltaTime * smoothSpeed);
            dof.gaussianEnd.value = Mathf.Lerp(dof.gaussianEnd.value, smoothedFocus + 1f, Time.deltaTime * smoothSpeed);
            dof.gaussianMaxRadius.value = Mathf.Lerp(dof.gaussianMaxRadius.value, targetRadius, Time.deltaTime * smoothSpeed);
        }
    }

    float SampleCenterDepth()
    {
        RenderTexture rt = RenderTexture.GetTemporary(1, 1, 24, RenderTextureFormat.Depth);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(1, 1, TextureFormat.RFloat, false);
        tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
        tex.Apply();

        float depth = tex.GetPixel(0, 0).r;

        RenderTexture.active = null;
        cam.targetTexture = null;
        RenderTexture.ReleaseTemporary(rt);
        Destroy(tex);

        return depth;
    }

    float DepthToWorldDistance(float depth01)
    {
        return cam.farClipPlane * cam.nearClipPlane /
               (cam.farClipPlane - depth01 * (cam.farClipPlane - cam.nearClipPlane));
    }
}
