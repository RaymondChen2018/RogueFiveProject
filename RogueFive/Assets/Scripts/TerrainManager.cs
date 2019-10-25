using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
public class TerrainManager : MonoBehaviour
{
    [Serializable]
    public class Count{
        public int maximum;
        public int minimum;
        public Count(int min, int max)
        {
            maximum = max;
            minimum = min;
        }

    }
    public int dungeonx = 20;
    public int dungeony = 200;
    public int waitingroomy = 10;
    public Count roomsize = new Count(20, 30);
    public Count numberOfFloors = new Count(50, 100);
    public GameObject[] floorTiles;
    public GameObject[] waterTiles;
    public GameObject[] bossRoomTiles;
    private Transform terrainHolder;
    private Transform roomHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialDungeon()
    {

        //Initial the grid list from the top to bottom.
        gridPositions.Clear();
        for(int i = 0; i < dungeonx; i++)
        {
            for (int j = 0; j < dungeony-waitingroomy; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }
    //initial the background and the boss room floors
    void TerrainSetup()
    {
        terrainHolder = new GameObject("terrain").transform;
        for(int i = 0;i< dungeonx; i++)
        {
            for(int j = 0; j< dungeony; j++)
            {
                GameObject toInstantiate = waterTiles[Random.Range(0, waterTiles.Length)];//initial the background watertile
                //last 3 tile to be boss room floor;
                if(j <=3)
                {
                    toInstantiate = bossRoomTiles[Random.Range(0, bossRoomTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(terrainHolder);
            }
        }
    }
    /*
     //room setup is going to creat different room with different theme and elements
    void roomSetup()
    {
        roomHolder = new GameObject("Room").transform;


    }
    //return a random position in certain range from gridlist
    Vector3 RandomPosition(int startIndex,int endIndex)
    {
        int randomIndex = Random.Range(startIndex, endIndex+1);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;

    }
    void LayOutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    */

    //return a random position from gridlist
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;

    }
    //choose randomly from tilearray, and place at random position within the range of maximum and minimum
    void LayOutObjectAtRandom(GameObject[] tileArray,int minimum, int maximum)
    {
        int objectCount =Random.Range(minimum, maximum + 1);
        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
  public void SetupScene()
    {
 
        TerrainSetup();
        InitialDungeon();
        LayOutObjectAtRandom(floorTiles, numberOfFloors.minimum, numberOfFloors.maximum);

    }
   
}
