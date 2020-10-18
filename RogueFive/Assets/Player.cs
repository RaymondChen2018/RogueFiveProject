using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
[System.Serializable]
public class UnityEventFloat : UnityEvent<float>
{

}

public class Player : MonoBehaviour
{
    [Header("Controller")]
    public KeyCode up;
    public KeyCode down;
    public KeyCode right;
    public KeyCode left;

    [Header("Swim Water")]
    public float swimSpeed = 300.0f;

    [Header("Land Walk (Only when touch ground)")]
    [SerializeField] private float walkSpeed = 3000.0f;
    [SerializeField] private float maxLandWalkSpeed = 3.0f;
    [SerializeField] private UnityEventFloat OnMoveTowardAngle = new UnityEventFloat();
    [SerializeField] private Transform playerDirectionTransform;

    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 150.0f;
    private float jumpCoolDown = 0.1f;
    private float timeLastJump = 0.0f;
    [SerializeField] private KeyCode keyJump;
    [SerializeField] LayerMask TerrainMask;
    private bool touchGround = false;

    [Header("Oxygen settings")]
    [SerializeField] private float oxygen = 100.0f;
    [SerializeField] private float maxOxygen = 100.0f;
    [SerializeField] private float oxygenDepletionRate = 5.0f;
    private SubmergePhysics submergePhysic;

    [Header("Health")]
    [SerializeField] private Health health;

    [Header("Animation")]
    [SerializeField] Animator anim;
    [SerializeField] private float pitchMaxAngle = 70.0f;
    [SerializeField] private float pitchMinAngle = -70.0f;

    Rigidbody2D RB;
    CircleCollider2D circleCollider;
    public static Player player;

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = this;
        }
        else
        {
            Debug.LogError("Two players in scene");
        }

        RB = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        submergePhysic = GetComponent<SubmergePhysics>();
        health.OnDeath.AddListener(die);
    }

    // Update is called once per frame
    void Update()
    {
        if(health.hasDied())
        {
            return;
        }

        Vector2 thisPos = transform.position;

        //////////////////////////////////////// Oxygen //////////////////////////////////////////////////
        if (submergePhysic.getDiveState() == DiveState.fullyInWater)
        {
            oxygen -= Time.deltaTime * oxygenDepletionRate;
        }
        else
        {
            oxygen = maxOxygen;
        }
        if(oxygen < 0)
        {
            health.damage(health.getHealth());
        }

        //////////////////////////////////////// Movement ///////////////////////////////////////////////

        // Is player touching ground?
        touchGround = Physics2D.CircleCast(thisPos, circleCollider.radius, Vector2.down, 0.05f, TerrainMask).collider != null;
        anim.SetBool("touchground", touchGround);

        Vector2 moveVec = Vector2.zero;
        // Land Movement
        Vector2 landMoveVec = Vector2.zero;
        if (touchGround)
        {
            if (Input.GetKey(left))
            {
                landMoveVec += Vector2.left;
            }
            if (Input.GetKey(right))
            {
                landMoveVec += Vector2.right;
            }
            if (Input.GetKeyDown(keyJump) && Time.time > timeLastJump + jumpCoolDown)
            {
                timeLastJump = Time.time;
                RB.AddForce(Vector2.up * jumpSpeed);
            }
        }

        // Aqua Control
        Vector2 aquaMoveVec = Vector2.zero;
        if (Input.GetKey(up))
        {
            aquaMoveVec += Vector2.up;
        }
        if (Input.GetKey(down))
        {
            aquaMoveVec += Vector2.down;
        }
        if (Input.GetKey(left))
        {
            aquaMoveVec += Vector2.left;
        }
        if (Input.GetKey(right))
        {
            aquaMoveVec += Vector2.right;
        }
        float submergeRatio = Mathf.Clamp(submergePhysic.getSubmergeRatio(), 0.0f, 1.0f);

        // Sum up move force
        moveVec = ((aquaMoveVec.normalized * submergeRatio).normalized * swimSpeed + landMoveVec.normalized * walkSpeed);

        // Move body
        RB.AddForce(moveVec * Time.deltaTime);

        // Limit land walk speed
        float currMovementSpeed = RB.velocity.magnitude;
        
        if (touchGround && currMovementSpeed > maxLandWalkSpeed)
        {
            RB.velocity *= maxLandWalkSpeed / currMovementSpeed;
        }

        //////////////////////////////////// Animation ////////////////////////////////////////////////////////
        Vector2 animMoveDir;
        if (submergePhysic.getDiveState() != DiveState.fullyInWater)
        {
            animMoveDir = moveVec;
        }
        else
        {
            animMoveDir = RB.velocity;
        }
        
        // Update moveDirection reference gameobject
        if (animMoveDir.sqrMagnitude != 0.0f)
        {
            float moveAngle = Mathf.Atan2(animMoveDir.y, animMoveDir.x) * 180.0f / 3.14f;
            playerDirectionTransform.rotation = Quaternion.Euler(0, 0, moveAngle);
            OnMoveTowardAngle.Invoke(moveAngle);
        }

        // Facing left or right?
        Vector3 flipScale = anim.transform.localScale;
        if (animMoveDir.x > 0.0f)
        {
            flipScale.x = Mathf.Abs(flipScale.x);
            anim.transform.localScale = flipScale;
        }
        else if (animMoveDir.x < 0.0f)
        {
            flipScale.x = Mathf.Abs(flipScale.x) * -1;
            anim.transform.localScale = flipScale;
        }

        // Rotate via swim direction under water
        if (submergePhysic.getDiveState() == DiveState.fullyInWater)
        {
            // Change rotation only when swimming
            if(animMoveDir != Vector2.zero)
            {
                float movementRightAngle = Mathf.Atan2(animMoveDir.y, Mathf.Abs(animMoveDir.x)) * 180.0f / 3.14f;
                float movementRightAngleClamped = Mathf.Clamp(movementRightAngle, pitchMinAngle, pitchMaxAngle);

                if (flipScale.x < 0.0f)
                {
                    movementRightAngleClamped *= -1;
                }

                anim.transform.rotation = Quaternion.Euler(0, 0, movementRightAngleClamped);
            }
        }
        // Resume upright rotation
        else
        {
            anim.transform.rotation = Quaternion.identity;
        }

        anim.SetBool("fullyinwater", (submergePhysic.getDiveState() == DiveState.fullyInWater));
        anim.SetFloat("speed", RB.velocity.magnitude / 5.0f);
    }

    public void setMaxOxygen(float newMax)
    {
        maxOxygen = newMax;
    }

    public float getOxygenRatio()
    {
        return oxygen / maxOxygen;
    }

    public float getHealthRatio()
    {
        return health.getHealth() / health.getMaxHealth();
    }

    private void die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Use collisionstay because multiple collision could trigger exit and turn off touchground while other are colliding
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    touchGround = true;
    //    
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    touchGround = false;
    //    anim.SetBool("touchground", false);
    //}
}
