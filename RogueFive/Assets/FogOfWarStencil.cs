using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarStencil : MonoBehaviour
{
    [SerializeField] FOW_BaseCompose baseCompute;
    private UnityStandardAssets.ImageEffects.BlurOptimized blurModule;

    // Start is called before the first frame update
    void Start()
    {
        //if (enabled)
        //{
        //    Debug.LogWarning("This camera should be off and only for mannual render");
        //    enabled = false;
        //}

        blurModule = GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (baseCompute != null)
        {
            // Reallocate buffer
            RenderTexture.ReleaseTemporary(baseCompute.fogTexBuffer);
            baseCompute.fogTexBuffer = RenderTexture.GetTemporary(source.width, source.height);

            // Blur pass
            if (blurModule != null && blurModule.enabled)
            {
                RenderTexture blurPass = RenderTexture.GetTemporary(source.width, source.height);
                blurModule.BlurItRaymond(source, blurPass);

                Graphics.Blit(blurPass, baseCompute.fogTexBuffer);

                RenderTexture.ReleaseTemporary(blurPass);
            }
            else
            {
                Graphics.Blit(source, baseCompute.fogTexBuffer);
            }
        }
    }
}
