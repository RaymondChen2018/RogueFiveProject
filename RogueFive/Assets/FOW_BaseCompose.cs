using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOW_BaseCompose : MonoBehaviour
{
    [Header("Fog Mask")]
    [SerializeField] Camera fogMaskCam;
    public RenderTexture fogMaskBuffer;

    [Header("Fog Texture")]
    [SerializeField] Camera fogTextureCam;
    public RenderTexture fogTexBuffer;

    [Header("Composition")]
    [Tooltip("How to compose layers?")] [SerializeField] Shader composeShader;
    Material composeMaterial;

    /// <summary>
    /// Z depth of light overlay on fogofwar mask render layer, Eg. maglite, spotlight
    /// </summary>
    public const float LIGHT_Z = -0.1f;

    // Start is called before the first frame update
    void Start()
    {
        composeMaterial = new Material(composeShader);
        composeMaterial.hideFlags = HideFlags.DontSave;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Render mask cam to store mask texture in "fogMaskBuffer"
        fogMaskCam.Render();

        // Render fog cam to store masked fog texture in "fogTexBuffer"
        fogTextureCam.Render();

        if(fogTexBuffer != null && fogMaskBuffer != null)
        {
            // Pass alpha buffer
            composeMaterial.SetTexture("_MaskBuffer", fogMaskBuffer);

            // Pass masked fog buffer
            composeMaterial.SetTexture("_FogTexture", fogTexBuffer);

            // Use composeMaterial to process fogofwar effect
            Graphics.Blit(source, destination, composeMaterial);
        }
        else
        {
            Debug.LogError("Layers not completed");
            Graphics.Blit(source, destination);
        }
    }
}
