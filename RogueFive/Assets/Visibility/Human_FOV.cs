using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FOV_MesbGenerator : MonoBehaviour {
    public float angle_check;
    public float meshResolution;
    public int edgeResolveiteration;
    public float edgeDstThreshold;
    public float maskCutAwayDst;
    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public bool createShadowFin = true;
    public GameObject shadow_fin_template;
    public List<GameObject> shadow_fin_pool = new List<GameObject>();
    public bool fullview;
    public bool reverse;

    [SerializeField] private Transform sprite_orient;
    GameObject fade_view;
    public List<Transform> visibleTargets = new List<Transform>();
    [SerializeField] float eyeGap = 1.0f;
    [SerializeField] float viewDist = 30.0f;
    [SerializeField] float viewAngle = 90.0f;
    float shadowThredshold = 1.0f;
    float shadowZ = -5.0f;
    GameObject Darkness;
    Transform fade_view_left;
    Transform fade_view_right;
    // Use this for initialization
    void Start()
    {
        Darkness = GameObject.Find("Darkness");
        fade_view = GameObject.Find("Fade_view");
        fade_view_left = fade_view.transform.Find("Fade_view_leaf_left");
        fade_view_right = fade_view.transform.Find("Fade_view_leaf_right");

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        fade_view.transform.localScale = new Vector2(viewDist, viewDist);
        fade_view_left.localRotation = Quaternion.Euler(0, 0, viewAngle / 2);
        fade_view_right.localRotation = Quaternion.Euler(0, 0, -viewAngle / 2);
        //StartCoroutine("FindTargetsWithDelay", 0.2f);

    }
    //IEnumerator FindTargetsWithDelay(float delay)
    //{

    //    while (true)
    //    {
    //        yield return new WaitForSeconds(delay);
    //        FindVisibleTargets();
    //    }
    //}
    
    //void FindVisibleTargets()
    //{
    //    visibleTargets.Clear();
    //    Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(sprite_orient.position, viewDist, targetMask);
        

    //    for (int i = 0; i < targetsInViewRadius.Length; i++)
    //    {
    //        Transform target = targetsInViewRadius[i].transform;
    //        Vector2 dirToTarget = (target.position - sprite_orient.position).normalized;
    //        if(Vector2.Angle(transform.right, dirToTarget) < viewAngle / 2)
    //        {
    //            float dstToTarget = Vector2.Distance(sprite_orient.position, target.position);
    //            if(!Physics2D.Raycast(sprite_orient.position,dirToTarget, dstToTarget, obstacleMask))
    //            {
    //                visibleTargets.Add(target);
    //                //Debug.Log("in: ");
    //            }
    //        }
    //    }
    //}
    void DrawFieldOfView()
    {
        float orientation = sprite_orient.eulerAngles.z;
        fade_view.transform.rotation =  Quaternion.Euler(0, 0, orientation);
        float FOV = viewAngle;
        if (reverse)
        {
            orientation -= 180;
            FOV = 360 - FOV;
        }
        int stepCount = Mathf.RoundToInt(FOV * meshResolution);
        float stepAngleSize = FOV / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        //Casting rays to detect corners
        int shadow_fins_count = 0;
        for (int i = 0; i <= stepCount; i++)
        {
            float angle;
            if (fullview)
            {
                angle = 180 - stepAngleSize * i;
            }
            else
            {
                angle = orientation + FOV / 2 - stepAngleSize * i;
            }
            
            ViewCastInfo newViewCast = ViewCast(angle);
            
            if (i > 0)
            {
                //bool edgeDsThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                float edge_dist = Mathf.Abs(oldViewCast.dst - newViewCast.dst);
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edge_dist > edgeDstThreshold))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                        //vec_shadow = edge.pointA;
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                        //vec_clear = edge.pointB;
                    }

                    

                    

                    if(QualitySettings.GetQualityLevel() > 2 && edge_dist > shadowThredshold && createShadowFin)
                    {
                        bool fade_in = true;//This boolean is gonna be very confusing
                        Vector3 vec_shadow = newViewCast.point;
                        Vector3 vec_clear = oldViewCast.point;
                        if (oldViewCast.dst < newViewCast.dst)
                        {
                            fade_in = false;
                            vec_shadow = oldViewCast.point;
                            vec_clear = newViewCast.point;
                        }
                        //Shadow fin
                        GameObject fin;
                        if (shadow_fins_count >= shadow_fin_pool.Count)
                        {
                            fin = Instantiate(shadow_fin_template, vec_shadow, Quaternion.identity);
                            shadow_fin_pool.Add(fin);
                        }
                        else
                        {
                            fin = shadow_fin_pool[shadow_fins_count];
                            fin.SetActive(true);
                        }
                        shadow_fins_count++;
                        Vector2 base_vec = vec_shadow - sprite_orient.position;
                        float base_vec_dist = base_vec.magnitude;
                        float base_angle = Mathf.Atan2(base_vec.y, base_vec.x) * 180 / 3.14f - 90;
                        
                        //Extend the soft side of the shadow
                        float offset = Mathf.Atan2(eyeGap, base_vec_dist) * 180 / 3.14f;
                        float soft_angle = 0;
                        if (fade_in)
                        {
                            soft_angle = base_angle - offset;
                        }
                        else
                        {
                            soft_angle = base_angle + offset;
                        }
                        
                        /*
                        if (fade_left)
                        {
                            base_angle += offset;
                        }
                        else
                        {
                            base_angle -= offset;
                        }
                        */

                        //
                        vec_shadow.z = shadowZ;
                        fin.transform.position = vec_shadow;
                        fin.transform.rotation = Quaternion.Euler(0, 0, base_angle);
                        //bool face_left = fin.transform.localScale.x >= 0;
                        Vector3 flip = fin.transform.localScale;
                        flip.y = Mathf.Max(edge_dist, viewDist - base_vec_dist) * 1.2f;
                        flip.x = (viewDist - base_vec_dist) * (eyeGap / base_vec_dist) * 1.2f;
                        if (!fade_in)
                        {
                            flip.x *= -1;
                        }
                        fin.transform.localScale = flip;
                        
                    }
                    
                }
            }

            
            //
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        
        //Clean redundant fins
        for (int j = shadow_fins_count; j < shadow_fin_pool.Count; j++)
        {
            shadow_fin_pool[j].SetActive(false);
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;

        //Analyzing points
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.right * maskCutAwayDst;
            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += sprite_orient.eulerAngles.z;
        }
        return new Vector2(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad),Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }

    //Refine two vertices that form a shadow edge
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        for (int i = 0; i < edgeResolveiteration; i++){
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDsThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && (!edgeDsThresholdExceeded))
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit = Physics2D.Raycast(sprite_orient.position, dir, viewDist, obstacleMask);
        if(hit)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }else
        {
            return new ViewCastInfo(false, sprite_orient.position + dir * viewDist, viewDist, globalAngle);
        }
    }
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
    }
}





