using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshRenderer))]
public class FollowSpline : MonoBehaviour
{

    public BezierSpline spline;

    public float duration = 5f;
    private float progressLimit=1f;
    private float progress;
    public bool alignToSpline;
    public FollowSplineMode mode;
    public float Delay=0f;
    private bool goingForward = true;
    private bool follow = false;
    public bool followOnStart = true;
    public bool alwaysShow = false;
    public float endPrecision = 1f;

    private void Reset()
    {
        goingForward = true;
        progress = 0f;       
    }
    private void MoveOnSpline(float progress)
    {
        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (alignToSpline)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }

    private void Go()
    {
        follow = true;
 
        if(this.GetComponent<MeshRenderer>().enabled == false) {
            MoveOnSpline(0f);
            this.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void Go(float delay)
    {

        StartCoroutine(DelayThenGo(delay));
    }

    private IEnumerator DelayThenGo(float delay)
    {
      //  Debug.Log("beg corroutine"); 
        yield return new WaitForSeconds(delay);

      //  Debug.Log("after yield");
        Go();
    }

    private void Start()
    {
       if (!alwaysShow)
        {
            this.GetComponent<MeshRenderer>().enabled = false;
        }
        
    }
    private void OnEnable()
    {
        if (!alwaysShow)
        {
            
            this.GetComponent<MeshRenderer>().enabled = false;
        }


    }
    private void Stop()
    {
        follow = false;
    }
    private void Update()
    {
        if (followOnStart)
        {
            if (Delay > 0f)
            {
                StartCoroutine(DelayThenGo(Delay));
            }
            else
            {
                Go();
            }            
        }
        if (follow && spline!=null && duration >0f && spline.numberOfClicks>3)
        {           
            float distance = Vector3.Distance(spline.GetControlPoint(spline.numberOfClicks - 3), spline.GetPoint(progress)- spline.transform.localPosition);
            
            if (goingForward)
            {
                if (distance < endPrecision)
                {
                    progressLimit = progress;
                    progress = 2f;
                }
                if (progress == 2f)
                {
                    if (mode == FollowSplineMode.Once)
                    {
                        progress = progressLimit;
                        follow = false;
                    } else if (mode == FollowSplineMode.Loop)
                    {
                        progress = 0;
                    }
                    else
                    {
                        progress = 2f * progressLimit - progressLimit;
                        goingForward = false;
                    }
                }
                else
                {

                    progress += Time.deltaTime / duration;
                }

            }
            else
            {
                distance = Vector3.Distance(spline.GetControlPoint(0), spline.GetPoint(progress) - spline.transform.localPosition);

                if (distance < endPrecision)
                {
                    progressLimit = progress;
                    progress = 2f;
                }
                if (progress == 2f)
                {
                    if (mode == FollowSplineMode.PingPong)
                    {
                        progress = 0f;
                        follow = false;
                    }else if (mode == FollowSplineMode.LoopPingPong)
                    {
                        progress = 0f;
                        goingForward = true;
                    }
                    else
                    {
                        progress = -progressLimit;
                        goingForward = true;
                    }

                }
                else
                {
                    progress -= Time.deltaTime / duration;
                }
            }
            MoveOnSpline(progress);
        }

    }
}