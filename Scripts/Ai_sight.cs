using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ai_sight : MonoBehaviour
{

    private GameObject player;
    public CircleCollider2D col;
    
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    
    public bool targetFound;
    public bool enemyInTrigger = false;
    public int rays;
    public string target;
    private GameObject[] test;
    
    

    Mesh viewMesh;
    List<Vector3> viewPoints = new List<Vector3>();
    Vector3 edgeMinPoint, edgeMaxPoint;
    public MeshFilter viewMeshFilter;
    public bool drawFoW;


    // Start is called before the first frame update
    void Start()
    {

        Physics2D.queriesStartInColliders = false;
        col.radius = viewRadius-1;
        
        
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        
        
    }
    
    private void Update()
    {
        
        if (enemyInTrigger)
        {
            FindVisibleTargets();
        }
    }

    private void LateUpdate()
    {
        if (drawFoW)
        {
            DrawFieldOfView();
        }
    }

    void FindVisibleTargets()
    {
        print("searchingtarget");
        targetFound = false;
        float stepAngleSize = viewAngle / rays;
        for (int i = 0; i <= rays; i++)
        {
            float angle = -transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            Vector3 dir = DirFromAngle(angle + 180f);
            RaycastHit2D hitTarget = Physics2D.Raycast(transform.position, dir, viewRadius);
            if (hitTarget.collider != null)
            {
                if (hitTarget.collider.CompareTag(target))
                {
                    Debug.DrawLine(transform.position, hitTarget.point, Color.red);
                    targetFound = true;
                    print("Found target, attacking");
                    break;
                }
            }
            
        }
        
    }
    
    public Vector3 DirFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
/*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(target))
        {
            print("target in trigger");
            enemyInTrigger = true;
        }
        

    }

*/
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(target))
        {
            enemyInTrigger = true;
        }
        else
        {
            enemyInTrigger = false;
        }
    }

/*
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(target))
        {
            enemyInTrigger = false;
            targetFound = false;
        }
    }
    
    */



// FOV DRAW


    private void DrawFieldOfView()
    {
        //draw field of view
        float stepAngleSize = viewAngle / rays;
        viewPoints.Clear();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= rays; i++)
        {
            float angle = -transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle + 180f);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > 0;
                if (oldViewCast.hit != newViewCast.hit ||
                    (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    if (edgeMinPoint != Vector3.zero)
                        viewPoints.Add(edgeMinPoint);
                    if (edgeMaxPoint != Vector3.zero)
                        viewPoints.Add(edgeMaxPoint);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.up;

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
    
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle);

        return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
    }
    

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

}

