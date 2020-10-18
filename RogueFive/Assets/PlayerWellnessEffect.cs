using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class PlayerWellnessEffect : MonoBehaviour
{
    [Header("Oxygen Low Effect")]
    [Tooltip("Oxygen high -> low mapped from left to right; Alpha 0 means no effect, 1 means full effect")][SerializeField] private Gradient O2MeterFxMap;
    [Tooltip("Amount of blur when oxygen runs out")] [SerializeField] float maxBlurSize = 8.0f;
    [SerializeField] private BlurOptimized blur;

    [Header("Health Low Effect")]
    [Tooltip("Health high -> low mapped from left to right; Alpha 0 means no effect, 1 means full effect")] [SerializeField] private Gradient HealthMeterFxMap;
    [SerializeField] private Image bleedScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Player.player == null)
        {
            return;
        }

        // Map oxygen effect
        float playerOxygenReverseRatio = 1.0f - Player.player.getOxygenRatio();
        float blurSizeTime = O2MeterFxMap.getAlpha(playerOxygenReverseRatio);
        if(blurSizeTime <= 0.0f)
        {
            blur.enabled = false;
        }
        else
        {
            blur.enabled = true;
            blur.blurSize = Mathf.Lerp(0.0f, maxBlurSize, blurSizeTime);
        }

        // Health effect
        if(bleedScreen != null)
        {
            float playerHealthReverseRatio = 1.0f - Player.player.getHealthRatio();
            float bleenAlpha = HealthMeterFxMap.getAlpha(playerHealthReverseRatio);
            Color bleedScreenColor = bleedScreen.color;
            bleedScreenColor.a = bleenAlpha;
            bleedScreen.color = bleedScreenColor;
        }
        else
        {
            Debug.LogWarning("No bleed screen assigned!");
        }
    }
}
