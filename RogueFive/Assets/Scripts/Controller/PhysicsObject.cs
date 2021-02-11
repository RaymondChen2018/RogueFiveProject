using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//------------------------
//PhysicsObject类：
//
//作为一个物理对象的基类，具备碰撞检测和重力加速度
//
//------------------------
public class PhysicsObject : MonoBehaviour
{
    [Tooltip("调整重力的比例")]
    public float gravityModifier = 0.1f;
    
    [Tooltip("地面法向量（x,y） y的最小值， y大于该值时被认为是地面（这个目前没什么用）")]
    public float minGroundNormalY = .85f;
    
    [Tooltip("速度向量，供Debug时观测）")]
    [SerializeField]
    protected Vector2 velocity;
    
    protected Vector2 targetVelocity;//加速度向量
    
    protected bool grounded = false;//是否着地
    protected Vector2 groundNormal;//地面法向量


  
    protected Rigidbody2D rb2d;//刚体
    
    //碰撞检测相关
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;//最小碰撞距离
    protected const float shellRadius = 0.01f; //碰撞检测外壳，防止穿模

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
       targetVelocity = Vector2.zero;
       ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {
    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround;
        
        if (grounded)
        {
            moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        }
        else
        {
            moveAlongGround = new Vector2(1f, 0f);
        }

        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move,false);
        
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    
    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;
        
        //碰撞检测
        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal; 
                        //currentNormal = Vector2.up;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity -= projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }
}
