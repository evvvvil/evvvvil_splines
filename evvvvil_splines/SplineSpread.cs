using UnityEngine;

public class SplineSpread : MonoBehaviour
{

    public BezierSpline spline;

    public int frequency;

    public bool lookForward;
    public float elementSize = 1f;

    public float endPrecision = 1f;

    public Transform[] items;

    private void Awake()
    {
        if (frequency <= 0 || items == null || items.Length == 0)
        {
            return;
        }
        float stepSize = frequency * items.Length;

        // WATCH OUT!!! IF YOU RE INTRODUCE LOOPS IN SPLINES THEN:
        //REPLACE THE NEXT LINE WITH: if (spline.Loop || stepSize == 1) 
        //WATCH OUT BRO FUCK SAKE
        //WATCH OUT MENG
        //ARE YOU WATCHING OUT OR WUT?????
        if (stepSize == 1)
        {
            stepSize = 1f / stepSize;
        }
        else
        {
            stepSize = 1f / (stepSize - 1);
        }
        for (int p = 0, f = 0; f < frequency; f++)
        {
            for (int i = 0; i < items.Length; i++, p++)
            {
                Transform item = Instantiate(items[i]) as Transform;
                float distance = Vector3.Distance(spline.GetControlPoint(spline.numberOfClicks - 3), spline.GetPoint(p * stepSize));
              //  Debug.Log("distance"+distance);
                if (distance < endPrecision)
                {
                    break;
                }
                else
                {
                    Vector3 position = spline.GetPoint(p * stepSize);
                    item.transform.localPosition = position;
                    if (lookForward)
                    {
                        item.transform.LookAt(position + spline.GetDirection(p * stepSize));
                    }
                    item.transform.localScale = new Vector3 (elementSize, elementSize, elementSize);
                    item.transform.parent = transform;
                }
                
            }
        }
    }
}