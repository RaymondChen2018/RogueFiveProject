using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //摄像机
    [Tooltip("摄像机对象")]
    public Camera mCamera;
    //玩家对象
    [Tooltip("玩家对象（追踪对象）")]
    public GameObject target;

    //最大偏移量，当玩家与摄像机的相对位置超过该值时
    //摄像机进行跟随
    [Tooltip("X轴最大相对偏移")]
    public float MaxOffsetX = 2f;
    [Tooltip("Y轴最大相对偏移")]
    public float MaxOffsetY = 2f;
    //摄像机跟随速度
    [Tooltip("摄像机跟随速度")]
    public float followingSpeed = 2f;

    private bool isFollowing = false;
    // Start is called before the first frame update
    void Start()
    {
        //摄像机对准玩家
        mCamera.transform.position = new Vector3(target.transform.position.x,
                                                 target.transform.position.y,
                                                 mCamera.transform.position.z);
        
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = target.transform.position.x - mCamera.transform.position.x;
        float offsetY = target.transform.position.y - mCamera.transform.position.y;

        if (Mathf.Abs(offsetX) > MaxOffsetX || Mathf.Abs(offsetY) > MaxOffsetY)
        {
            isFollowing = true;
        }

        if (isFollowing)
        {
            Vector3 targetPos;
            
            targetPos.z = mCamera.transform.position.z;
            targetPos.x = mCamera.transform.position.x + offsetX * followingSpeed * Time.deltaTime;
            targetPos.y = mCamera.transform.position.y + offsetY * followingSpeed * Time.deltaTime;
            
            mCamera.transform.position = targetPos;
            
            //当再次锁定玩家时，停止跟随
            if (offsetX == 0 && offsetY == 0)
            {
                isFollowing = false;
            }
        }
    }
}
