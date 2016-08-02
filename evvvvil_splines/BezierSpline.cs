using UnityEngine;
using System;
using System.Collections.Generic;

public class BezierSpline : MonoBehaviour
{

    [SerializeField]
    private List<Vector3> points;

    [SerializeField]
    private List<BezierControlPointMode> modes;

    public BezierShowHandlesMode ShowHandlesMode;

    public List<float> pointAnimArray;
    public float GizmoSize= 1f;
    //public bool ShowDir;
    public int ControlPointCount
    {
        get
        {
            return points.Count;
        }
    }

    public int numberOfClicks = 0;
    public int sarcasticCount = 0;
    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }
    public void SetControlPoint(int index, Vector3 point)
    {
        //Debug.Log("inside setcontrolpoint");
        if (index % 3 == 0 && index!=3)
        {
           // Debug.Log("inside setcontrolpoint first if");
            Vector3 delta = point - points[index];
            if (index > 0)
            {
                points[index - 1] += delta;
            }
            if (index + 1 < points.Count)
            {
          //      Debug.Log("inside setcontrolpoint second if");

                points[index + 1] += delta;
            }
        }
        points[index] = point;
      //  Debug.Log("before enforcing");


        EnforceMode(index);
     //   Debug.Log("After enforcing");

    }

    public void RemoveControlPoint(int index)
    {
        if (numberOfClicks <= 3)
        {
            Reset();
        }
        else
        {

            if (index == 0)
            {
                index = 1;
                modes.RemoveAt(0);
            }
            else
            {
                modes.RemoveAt((index + 2) / 3);
            }

            points.RemoveAt(index - 1);
            points.RemoveAt(index - 1);
            points.RemoveAt(index - 1);
            pointAnimArray.RemoveAt(index - 1);
            pointAnimArray.RemoveAt(index - 1);
            pointAnimArray.RemoveAt(index - 1);
            numberOfClicks -= 3;
        }
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
       // Debug.Log("inside setcontrolpointmode");
        modes[(index + 1) / 3] = mode;
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Count - 1)
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }
        Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    public int CurveCount
    {
        get
        {
            return (points.Count - 1) / 3;
        }
    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Count - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(
            points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Count - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(
            points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void Reset()
    {
        points = new List<Vector3> {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        pointAnimArray = new List<float> {
            0f,
            0f,
            0f,
            0f
        };
        modes = new List<BezierControlPointMode> {
            BezierControlPointMode.Mirrored,
            BezierControlPointMode.Mirrored
        };
        numberOfClicks = 0;
    }
    public void AddCurve()
    {
        Vector3 point = points[points.Count - 1];
        //Array.Resize(ref points, points.Count + 3);
        point.x += 1f;
        points.Add(point);
        point.x += 1f;
        points.Add(point);
        point.x += 1f;
        points.Add(point);

        //Array.Resize(ref modes, modes.Count + 1);
        modes.Add(modes[modes.Count - 2]);
        EnforceMode(points.Count - 4);
        pointAnimArray.Add(0f);
        pointAnimArray.Add(0f);
        pointAnimArray.Add(0f);
       
    }
    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    

}