using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class GrindTrigger : MonoBehaviour
{
    [SerializeField]
    float grindTime = 3f;
    [SerializeField]
    float grindCooldown = 2f;
    Coroutine playAnimationCoroutine = null;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && playAnimationCoroutine == null)
        {
            playAnimationCoroutine = StartCoroutine(PlayAnimation_CO(other.GetComponent<SkateMovement>(),other.GetComponent<SplineAnimate>()));
        }
    }

    [SerializeField] private float distance = 50f;
    private IEnumerator PlayAnimation_CO(SkateMovement playerMovement, SplineAnimate splineAnimate)
    {
        Vector3 point;
        float offset = FindClosestPointOnSpline(splineAnimate.transform.position,splineAnimate.Container.Spline,out point);
        playerMovement.Pause(true);
        splineAnimate.StartOffset = offset;
        // while (Vector3.Distance(playerMovement.transform.position, splineAnimate.Container.transform.TransformPoint(point)) > distance)
        // {
        //     playerMovement.transform.position = Vector3.Lerp(playerMovement.transform.position, splineAnimate.Container.transform.TransformPoint(point), Time.deltaTime);
        //     playerMovement.transform.rotation = Quaternion.Lerp(playerMovement.transform.rotation, splineAnimate.Container.transform.rotation, Time.deltaTime);
        //     yield return null;
        // }
        splineAnimate.Play();
        yield return new WaitForSeconds(3f);
        splineAnimate.Pause();
        splineAnimate.Restart(false);
        // yield return new WaitForSeconds(2f);
        playerMovement.Pause(false);
        playerMovement.ExitGrind();
        yield return new WaitForSeconds(2f);
        playAnimationCoroutine = null;
    }
    
    private float FindClosestPointOnSpline(Vector3 worldPosition,Spline spline,out Vector3 knotPoint)
    {
        float closestT = 0f;
        float minDistance = float.MaxValue;

        // Sample points along the spline
        int i = 0;
        knotPoint = worldPosition;
        foreach (BezierKnot knot in spline.Knots)
        {
            float t = i / (float)spline.Knots.Count(); // Normalized parameter
            knotPoint = knot.Position;
            Vector3 point = spline.EvaluatePosition(t); // Get spline position at t

            float distance = Vector3.Distance(worldPosition, point);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestT = t;
            }

            ++i;
        }

        return closestT; // Return the t value of the closest point
    }
}
