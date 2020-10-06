using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftShadowStencil : MonoBehaviour
{
    [SerializeField] FOW_BaseCompose baseCompute;

    void Start()
    {
        //if (enabled)
        //{
        //    Debug.LogWarning("This camera should be off and only for mannual render");
        //    enabled = false;
        //}
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (baseCompute != null)
        {
            RenderTexture.ReleaseTemporary(baseCompute.fogMaskBuffer);
            baseCompute.fogMaskBuffer = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(source, baseCompute.fogMaskBuffer);
        }
    }
}
