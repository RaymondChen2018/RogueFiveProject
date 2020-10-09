using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class AirRoom : MonoBehaviour
{
    [Header("Water Background")]
    [SerializeField] private Material material_airRoomStencil;
    private MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    [Header("Vision")]
    [SerializeField] bool generateVision = false;
    [SerializeField] LayerMask maskLayer;
    [SerializeField] private Material material_spriteDefault;
    private GameObject visionObject;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material_airRoomStencil;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = GetComponent<PolygonCollider2D>().polyCollider2DToMesh();

        // Generate vision
        if (generateVision)
        {
            visionObject = new GameObject("Vision");
            visionObject.layer = maskLayer.LayerMaskToLayer();
            if(LayerMask.LayerToName(visionObject.layer) != "SoftShadow")
            {
                Debug.LogWarning("Air room vision should be on softshadow layer!");
            }
            visionObject.transform.parent = transform;
            visionObject.transform.localPosition = new Vector3(0,0,FOW_BaseCompose.LIGHT_Z);

            MeshRenderer meshRendererVision = visionObject.AddComponent<MeshRenderer>();
            meshRendererVision.sharedMaterial = material_spriteDefault;
            MeshFilter meshFilterVision = visionObject.AddComponent<MeshFilter>();
            meshFilterVision.mesh = meshFilter.mesh.copyMesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
