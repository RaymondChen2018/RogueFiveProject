using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FishFlip : MonoBehaviour
{
    [Header("Flip Sprite")]
    [Tooltip("When this fish switch left to right or vice versa, it flip sprite to face that way")][SerializeField] private bool flipSpriteOtherWay = true;
    [Tooltip("Sprite separated to perform sprite flip effect alone")][SerializeField] private SpriteRenderer spriteRenderer;

    Vector2 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        Assert.IsTrue(spriteRenderer.gameObject != gameObject, "sprite renderer must be separated for sprite flip & 180 rotation");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get movement delta
        Vector2 thisPos = transform.position;
        Vector2 deltaPos = thisPos - prevPos;
        bool flipRight = deltaPos.x > 0.0f;
        bool flipLeft = deltaPos.x < 0.0f;
        bool dontFlip = deltaPos.x == 0.0f;
        prevPos = thisPos;

        // Flip sprite
        if(flipSpriteOtherWay)
        {
            if (flipLeft)
            {
                // flip sprite
                spriteRenderer.flipX = true;

                // flip rotation
                spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 180.0f);
            }
            else if(flipRight)
            {
                // reset sprite
                spriteRenderer.flipX = false;

                // reset rotation
                spriteRenderer.transform.localRotation = Quaternion.identity;
            }
            // dont flip
            else{
            
            }
        }
    }
}
