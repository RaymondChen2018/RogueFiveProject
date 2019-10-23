using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private TerrinManager terrinManager;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(gameObject);
        }
        terrinManager = GetComponent<TerrinManager>();
        terrinManager.SetupScene();
    }
}
