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
    [SerializeField]
    float lerpSpeed = 5f;
    [SerializeField]
    float grindSpeed = 5f;
    [SerializeField]
    SplineContainer relatedSpline;
    [SerializeField]
    Transform exitPoint = null;
    Coroutine playAnimationCoroutine = null;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && playAnimationCoroutine == null)
        {
            if (!other.GetComponent<SkateMovement>().isGrinding)
            {
                playAnimationCoroutine = StartCoroutine(PlayAnimation_CO(other.GetComponent<SkateMovement>(),other.GetComponent<SplineAnimate>()));
            }
        }
    }

    [SerializeField] private float distance = 50f;
    
    private Quaternion startGrindRotation;
    private IEnumerator PlayAnimation_CO(SkateMovement playerMovement, SplineAnimate splineAnimate)
    {
        splineAnimate.Container = relatedSpline;
        //splineAnimate.Restart(false);
        startGrindRotation = splineAnimate.transform.rotation;
        BezierKnot point;
        float offset = FindClosestPointOnSpline(splineAnimate.transform.position,splineAnimate.Container,splineAnimate.Container.Spline,out point);
        playerMovement.PauseForGrind(true);
        playerMovement.isGrinding = true;
        splineAnimate.MaxSpeed = grindSpeed;
        splineAnimate.StartOffset = offset;
        Vector3 realWorldKnotPos =
            splineAnimate.Container.transform.TransformPoint(splineAnimate.Container.Spline.EvaluatePosition(offset));
        Vector3 realWorldCenterPos = splineAnimate.Container.transform.TransformPoint(splineAnimate.Container.Spline.EvaluateCurvatureCenter(offset));
        while (Vector3.Distance(playerMovement.transform.position, realWorldKnotPos) > distance)
        {
            playerMovement.transform.position = Vector3.Lerp(playerMovement.transform.position, realWorldKnotPos, Time.deltaTime * lerpSpeed);
            Vector3 direction =  realWorldCenterPos - playerMovement.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);
            
            playerMovement.transform.rotation = Quaternion.Lerp(playerMovement.transform.rotation, targetRotation, lerpSpeed * Time.time);
            yield return null;
        }
        splineAnimate.Play();
        yield return new WaitForSeconds(grindTime);
        splineAnimate.Pause();
        // yield return new WaitForSeconds(2f);
        Vector3 lastSplinePos = splineAnimate.Container.transform.TransformPoint(splineAnimate.Container.Spline.EvaluatePosition(splineAnimate.NormalizedTime + offset));

        splineAnimate.Restart(false);
        playerMovement.ExitGrind(lastSplinePos, exitPoint ?exitPoint.position:realWorldCenterPos);
        playerMovement.PauseForGrind(false);
        yield return new WaitForSeconds(grindCooldown);
        playerMovement.isGrinding = false;
        playAnimationCoroutine = null;
    }
    
    private float FindClosestPointOnSpline(Vector3 worldPosition,SplineContainer container,Spline spline,out BezierKnot knotPoint)
    {
        float closestT = 0f;
        float minDistance = float.MaxValue;

        // Sample points along the spline
        int i = 0;
        knotPoint = new BezierKnot();
        foreach (BezierKnot knot in spline.Knots)
        {
            float t = i / (float)spline.Knots.Count(); // Normalized parameter
            knotPoint = knot;
            Vector3 point = container.transform.TransformPoint(spline.EvaluatePosition(t)); // Get spline position at t

            float _distance = Vector3.Distance(worldPosition, point);
            if (_distance < minDistance)
            {
                minDistance = _distance;
                closestT = t;
            }

            ++i;
        }
        return closestT; // Return the t value of the closest point
    }
}
