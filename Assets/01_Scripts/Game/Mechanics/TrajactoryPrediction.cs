using _01_Scripts.Game.Core;
using UnityEngine;

public class TrajactoryPrediction : MonoBehaviour
{
    [SerializeField] private LayerMask rayMask;
    [SerializeField] private LayerMask rayMask2;
    [SerializeField] private float distance;
    
    [SerializeField] private LineRenderer lineRenderer1;
    [SerializeField] private LineRenderer lineRenderer2;
    
    private Cell targetCell;

    public void Predict(Vector2 origin, Vector2 dir)
    {
        Ray2D ray = new Ray2D(origin, dir);
        
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, rayMask);
        
        Debug.DrawRay(origin, dir * distance, Color.red);

        SetLine(lineRenderer1, origin, ray.GetPoint(distance));

        if (PredictItemPlace(hit))
        {
            lineRenderer2.enabled = false;
            return;
        }
        
        if (hit.collider != null)
        {
            lineRenderer2.enabled = true;

            Vector2 inDirection = dir;
            Vector2 normal = hit.normal;
            Vector2 reflectDirection = Vector2.Reflect(inDirection, normal);

            Ray2D reflectedRay = new Ray2D(hit.point, reflectDirection);

            RaycastHit2D hit2 = Physics2D.Raycast(reflectedRay.origin, reflectedRay.direction, distance, rayMask2);
            
            Debug.DrawRay(reflectedRay.origin, reflectedRay.direction * distance, Color.green);

            SetLine(lineRenderer2, reflectedRay.origin, reflectedRay.GetPoint(distance));
            
            PredictItemPlace(hit2);
        }
        else
        {
            lineRenderer2.enabled = false;
        }
    }

    Item PredictItemPlace(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out Item item))
            {
                Cell temp = targetCell;
                targetCell = item.GetClosestCell(item.GetEmptyNeighbours(), hit.point);

                if (targetCell)
                    targetCell.PredictItem();

                if (temp != null && temp != targetCell)
                    temp.StopPredictingItem();
                
                return item;
            }
        }
        

        return null;
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

        if (targetCell)
            targetCell.StopPredictingItem();
    }
    
    public void EnableTrajectory()
    {
        lineRenderer1.enabled = true;
        lineRenderer2.enabled = true;
    }
}