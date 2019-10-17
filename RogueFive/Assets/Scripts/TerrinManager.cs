using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
public class TerrinManager : MonoBehaviour
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
    public int initialRoomy = 100;
    public Count numberOfFloors = new Count(50, 100);
    public GameObject[] floorTiles;
    public GameObject[] waterTiles;
    public GameObject[] bossRoomTiles;
    private Transform terrinHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialDungeon()
    {

        //Initial the grid list from the top to bottom.
        gridPositions.Clear();
        for(int i = 0; i < dungeonx; i++)
        {
            for (int j = 0; j < dungeony; j++)
            {
                gridPositions.Add(new Vector3(dungeonx, dungeony, 0f));
            }
        }
    }
    //initial the background and the boss room floors
    void TerrinSetup()
    {
        terrinHolder = new GameObject("Board").transform;
        for(int i = 0;i< dungeonx; i++)
        {
            for(int j = 0; j< dungeony; j++)
            {
                GameObject toInstantiate = waterTiles[Random.Range(0, floorTiles.Length)];
                if(j <=5)
                {
                    toInstantiate = bossRoomTiles[Random.Range(0, bossRoomTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(terrinHolder);
            }
        }
    }
    //return a random position from gridlist
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        Debug.Log(randomIndex);
        Debug.Log(randomPosition);
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
        
        TerrinSetup();
        InitialDungeon();
        LayOutObjectAtRandom(floorTiles, numberOfFloors.minimum, numberOfFloors.maximum);


    }
   
}
