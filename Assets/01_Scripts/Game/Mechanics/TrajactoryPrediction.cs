using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajactoryPrediction : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private float distance;
    
    [SerializeField] private LineRenderer lineRenderer1;
    [SerializeField] private LineRenderer lineRenderer2;
    
    public void Predict(Vector2 origin, Vector2 dir)
    {
        Ray2D ray = new Ray2D(origin, dir);
        
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, mask);
        
        Debug.DrawRay(origin, dir * distance, Color.red);
        SetLine(lineRenderer1, origin, ray.GetPoint(distance));
        
        if (hit.collider != null)
        {
            Vector2 inDirection = dir;
            Vector2 normal = hit.normal;
            Vector2 reflectDirection = Vector2.Reflect(inDirection, normal);

            Ray2D reflectedRay = new Ray2D(hit.point, reflectDirection);

            RaycastHit2D hit2 = Physics2D.Raycast(reflectedRay.origin, reflectedRay.direction, distance, mask);
            
            Debug.DrawRay(reflectedRay.origin, reflectedRay.direction * distance, Color.green);

            SetLine(lineRenderer2, reflectedRay.origin, reflectedRay.GetPoint(distance));
        }
    }
    
    void SetLine(LineRenderer lr, Vector2 or, Vector2 endPos)
    {
        lr.transform.localPosition = -transform.position;
        lr.positionCount = 2;

        lr.SetPosition(0, or);
        lr.SetPosition(1, endPos);
    }

    public void DisableTrajectory()
    {
        lineRenderer1.enabled = false;
        lineRenderer2.enabled = false;
    }
    
    public void EnableTrajectory()
    {
        lineRenderer1.enabled = true;
        lineRenderer2.enabled = true;
    }
}