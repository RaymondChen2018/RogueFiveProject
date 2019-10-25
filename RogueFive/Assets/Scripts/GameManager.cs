using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [SerializeField]private TerrainManager terrainManager;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Debug.LogError("Duplicate GameManager");
        }
        terrainManager.SetupScene();
    }
}
