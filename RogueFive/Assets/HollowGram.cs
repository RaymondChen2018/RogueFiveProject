using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowGram : MonoBehaviour
{
    [SerializeField] private GameObject prefabStreak;
    [SerializeField] private Transform projector;
    [SerializeField] private Transform[] streakEndPoints;

    [Header("Debug")]
    [SerializeField] private bool debugOn = true;
    [SerializeField] private Color debugColor_streak = Color.cyan;

    // Start is called before the first frame update
    void Start()
    {
        // Create streaks in place of streak end points
        for(int i=0;i<streakEndPoints.Length;i++)
        {
            Transform streak = Instantiate(prefabStreak, projector.position, Quaternion.identity).transform;
            streak.GetComponent<LineRenderer>().SetPosition(1, streakEndPoints[i].position - projector.position);
            Destroy(streakEndPoints[i].gameObject);
            streakEndPoints[i] = streak;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(debugOn && !Application.isPlaying && streakEndPoints != null && projector != null)
        {
            foreach(Transform streakEndPoint in streakEndPoints)
            {
                if(streakEndPoint == null)
                {
                    continue;
                }

                Debug.DrawLine(projector.position, streakEndPoint.position, debugColor_streak);
            }
        }
    }
}
