using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Ai_move : MonoBehaviour
{
  [SerializeField]  public float speed;
  [SerializeField] public bool moving;
  private Pathfinding pathfinding;
    
    private Vector2[] path;
    private int targetIndex;
    public float rotateSpeed;

    void Start()
    {
        path = null;
        targetIndex = 0;
        pathfinding = FindObjectOfType<Pathfinding>();

    }
    
    public bool AIcheckReachTarget(Vector3 pos)
    {
        return pathfinding.CanReachPathTarget(pos);
    }

    public void StopAIMovement()
    {
        StopCoroutine("FollowPath");
    }
    

    public void moveTo(Vector3 pos)
    {
        StopCoroutine("FollowPath");
        Vector3 targetPositionOld = pos + Vector3.up; // ensure != to target.position initially
        if (targetPositionOld != pos)
        {
            path = pathfinding.RequestPath(transform.position, pos);
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        if (path.Length > 0)
        {
            targetIndex = 0;
            Vector3 currentWaypoint = path[0];

            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        yield break;
                    }

                    currentWaypoint = path[targetIndex];
                }

                Vector3 lookDirection = (currentWaypoint - transform.position).normalized;
                float anlge = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg + 90f;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.AngleAxis(anlge, Vector3.forward), 
                    Time.deltaTime * rotateSpeed
                    );
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }


    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.yellow;

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}