using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarStencil : MonoBehaviour
{
    [SerializeField] FOW_BaseCompose baseCompute;

    // Start is called before the first frame update
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
            RenderTexture.ReleaseTemporary(baseCompute.fogTexBuffer);
            baseCompute.fogTexBuffer = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(source, baseCompute.fogTexBuffer);
        }
    }
}
