using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//------------------------
//玩家操控类：
//
//当前仅支持左右移动和空格键“跳跃”
//
//------------------------
public class PlayerController : PhysicsObject
{
    [Tooltip("向上的加速度向量")]
    public float jumpTakeOffSpeed = 4f;
    [Tooltip("最大垂直向上速度")]
    public float maxJumpSpeed = 3f;
    [Tooltip("最大水平速度")]
    public float maxSpeed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        
        if (Input.GetButton("Jump"))
        {
            velocity.y += (jumpTakeOffSpeed - gravityModifier * Physics2D.gravity.y) * Time.deltaTime;
            velocity.y = velocity.y > maxJumpSpeed ? maxJumpSpeed : velocity.y;
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        }

        targetVelocity = move * maxSpeed;
    }
}
