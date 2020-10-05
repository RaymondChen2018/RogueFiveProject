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
    public float swimSpeed = 300.0f;
    public float walkSpeed = 600.0f;
    [SerializeField] private UnityEventFloat OnMoveTowardAngle = new UnityEventFloat();
    [SerializeField] private Transform playerDirectionTransform;

    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 200.0f;
    [SerializeField] private KeyCode keyJump;
    private bool touchGround = false;

    [Header("Oxygen settings")]
    [SerializeField] private float oxygen = 100.0f;
    [SerializeField] private float maxOxygen = 100.0f;
    [SerializeField] private float oxygenDepletionRate = 5.0f;
    private SubmergePhysics submergePhysic;

    [Header("Health")]
    [SerializeField] private float health = 10.0f;
    [SerializeField] private float maxHealth = 10.0f;
    private bool dead = false;

    [Header("Animation")]
    [SerializeField] Animator anim;
    [SerializeField] private float pitchMaxAngle = 70.0f;
    [SerializeField] private float pitchMinAngle = -70.0f;

    Rigidbody2D RB;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        submergePhysic = GetComponent<SubmergePhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        if(dead)
        {
            return;
        }

        //////////////////////////////////////// Oxygen //////////////////////////////////////////////////
        if(submergePhysic.getDiveState() == DiveState.fullyInWater)
        {
            oxygen -= Time.deltaTime * oxygenDepletionRate;
        }
        else
        {
            oxygen = maxOxygen;
        }
        if(oxygen < 0)
        {
            die();
        }

        //////////////////////////////////////// Movement ///////////////////////////////////////////////
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
        RB.AddForce(moveVec * Time.deltaTime);
        if(moveVec.sqrMagnitude != 0.0f)
        {
            float moveAngle = Mathf.Atan2(moveVec.y, moveVec.x) * 180.0f / 3.14f;
            playerDirectionTransform.rotation = Quaternion.Euler(0,0,moveAngle);
            OnMoveTowardAngle.Invoke(moveAngle);
        }

        // Health
        if (health <= 0)
        {
            die();
        }

        //////////////////////////////////// Animation ////////////////////////////////////////////////////////
        // Facing left or right?
        Vector3 flipScale = anim.transform.localScale;
        if (moveVec.x > 0.0f)
        {
            flipScale.x = Mathf.Abs(flipScale.x);
            anim.transform.localScale = flipScale;
        }
        else if (moveVec.x < 0.0f)
        {
            flipScale.x = Mathf.Abs(flipScale.x) * -1;
            anim.transform.localScale = flipScale;
        }

        // Rotate via swim direction under water
        if (submergePhysic.getDiveState() == DiveState.fullyInWater)
        {
            // Change rotation only when swimming
            if(moveVec != Vector2.zero)
            {
                float movementRightAngle = Mathf.Atan2(moveVec.y, Mathf.Abs(moveVec.x)) * 180.0f / 3.14f;
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

    private void die()
    {
        dead = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        touchGround = true;
        anim.SetBool("touchground", true);
        if (Input.GetKeyDown(keyJump))
        {
            RB.AddForce(Vector2.up * jumpSpeed);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        touchGround = false;
        anim.SetBool("touchground", false);
    }
}
