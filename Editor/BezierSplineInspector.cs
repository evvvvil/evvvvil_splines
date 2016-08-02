using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;


[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{

    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    //private int numberOfclicks=0;
    private Vector3 MouseToXYZ;
    int maxAxisIndex = 5;
    private int lastMaxAxisIndex = 4;
    private float offset = 0f;
    private int createdTime;
    private int currentLife;
    private bool toggleAdd = false;
    private bool toggleRemove = false;
    private int doTheTwist = -1;
    private string sarcasticString = "'A great Bézier tool with a shit sense of humour'";
    public static System.Random _random = new System.Random();
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private GUISkin mySkin;
    private int selectedIndex = -1;
    private string[] beginningSarcasm = new string[] {
        "Impressive curve skills! Mummy made you join the dots as a child?",
        "Yeah bro building that line with confidence, love and a mouse pointer",
        "Does your boss know you downloaded this for free? Slacker",
        "One small step for a spline, one meaningless wiggle for mankind",
        "Feel the curve, touch the Bézier... Fondle the handle",
        "The decaying cabbage in the office fridge believes in you",
        "Is this the path to righteousness?... Looks like penis to me",
        "Yeah well, still pretty small, my spline is much bigger.",
        "Still not long enough for the half-eaten moth to even care" 
        };

    private string[] endSarcasm = new string[] {
        "So far so good: your parents did indeed give you enough attention as a child",
        "There you go: closer to lunch time now, you sloth",
        "Teenage machine guns shouldn't drink contrived milkshakes!",
        "Remember MySpace?... Still more memorable than this spline",
        "3 blind nuns drew a better spline, even blindfolded",
        "You can impress chicks with this tool, and hairy men too",
        "Leaders don't follow defined paths, so you are wrong",
        "Splines don't make for you lack of Quinoa knowledge",
        "You could be watching a cooking show, but you do this instead?",
        "Meanwhile in north-east of France they're making Champagne. You can still change",
        "This is taking a while... You better cancel your nitting class",
        "There are more important things in life than drawing points, like owning a cinderblock"
        };

    //private List<float> pointCreatedArray;
    //private List<int> listOfControlIds;
    public override void OnInspectorGUI()
    {
        if (spline != null) { 
        EditorGUI.BeginChangeCheck();

        float GizmoSize = EditorGUILayout.Slider("Gizmo Size",spline.GizmoSize,0f, 10f);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Bezier Gizmo Size Change");
            EditorUtility.SetDirty(spline);
            spline.GizmoSize = GizmoSize;
        }
        /*EditorGUI.BeginChangeCheck();
		bool showDir = EditorGUILayout.Toggle("Show Direction", spline.ShowDir);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Toggle Show Bezier Direction");
			EditorUtility.SetDirty(spline);
			spline.ShowDir = showDir;
		}*/
        EditorGUI.BeginChangeCheck();
        BezierShowHandlesMode ShowHandlesMode = (BezierShowHandlesMode)
        EditorGUILayout.EnumPopup("Show Handles", spline.ShowHandlesMode);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Bezier Show Handles Mode");
            spline.ShowHandlesMode=ShowHandlesMode;
            EditorUtility.SetDirty(spline);
        }
        spline = target as BezierSpline;
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount && !toggleRemove)
        {
            DrawSelectedPointInspector();
        }
            /*if (GUILayout.Button("Add Curve"))
            {
                Undo.RecordObject(spline, "Add Curve");
                EditorUtility.SetDirty(spline);
                spline.AddCurve(); 

            }*/
        }
    }
    private static void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + (int)(_random.NextDouble() * (n - i));
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }
    private string GetSarcasm()
    {
        //Returns a piece of sarcasm when each bezier spline point is created      
        int middlefinger = beginningSarcasm.Length;
        string thong=sarcasticString;
     
        if (spline.sarcasticCount < middlefinger)
        {
            if (spline.sarcasticCount == 0)
            {
                //Randomly shuffles arrays of sarcasms with "FISHER-YATES" method
                Shuffle(beginningSarcasm); Shuffle(endSarcasm);
            }
            thong = beginningSarcasm[spline.sarcasticCount];
        }
        else if (spline.sarcasticCount >= middlefinger)
        {
            thong = endSarcasm[(spline.sarcasticCount-middlefinger) % endSarcasm.Length];
        }
        spline.sarcasticCount++;
        return thong;
    }

    private void SetAdd(bool state)
    {
        if (state)
        {
            SetRemove(false);
        }
        toggleAdd = state;
    }


    private void SetRemove(bool state)
    {
        if (state)
        {
            SetAdd(false);
            if (selectedIndex > 0)
            {
                selectedIndex = -1;
            }
        }
        toggleRemove = state;
        

    }

    private void DrawSelectedPointInspector()
    {
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field(("Point "+ selectedIndex.ToString() + " Position"), spline.GetControlPoint(selectedIndex));

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Bezier Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)
            EditorGUILayout.EnumPopup(("Point " + selectedIndex.ToString() + " Mode"), spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Bezier Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
        
    }

    private void DrawBox(Rect position, Color color)
    {
        Color oldColor = GUI.color;

        GUI.color = color;
        GUI.Box(position, "");

        GUI.color = oldColor;
    }

    private void OnSceneGUI()
    {

        //float currentTime = (float)EditorApplication.timeSinceStartup;

        
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
        handleTransform.rotation : Quaternion.identity;
    /*    if (GUIUtility.hotControl > 0 && spline.numberOfClicks>0)
        {
            Debug.Log("clicked on some shit"+ GUIUtility.hotControl+" sdfjsdjfjh ");
           // GUIUtility.GetControlID();
            //Vector3 currentPos = Handles
           // GUIUtility.GetStateObject
           // Debug.Log("list of point space"+listOfControlIds.IndexOf(GUIUtility.hotControl));

            //GUIUtility.GetStateObject(GUIUtility.hotControl);
        };*/
        Vector3 p0 = ShowPoint(0);

        if (toggleAdd)
        {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.ArrowPlus);
        }
        else
        {
            if (toggleRemove)
            {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.ArrowMinus);
            }
        }

        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(0,5, 220, 200));


        
        
        GUILayout.Label((Texture)Resources.Load("evvvvil-3d-splines"));
        //GUI.skin.button.fontSize = 12;
        GUILayout.BeginHorizontal();
        //GUI.skin.button.padding = new RectOffset(0,0,0,0);
        GUILayout.Space(14);

        GUISkin mySkin = (GUISkin)Resources.Load("evvvvil-spline-skin");

        SetAdd(GUILayout.Toggle(toggleAdd,"",mySkin.customStyles[0]));

        GUILayout.Space(11);

        GUI.Box(new Rect(51, 63, 100, 100),"", mySkin.customStyles[3]);
           
        SetRemove(GUILayout.Toggle(toggleRemove, "", mySkin.customStyles[1]));
        /*
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                Debug.Log("Mouse over!");
            }

            else
            {
                Debug.Log("Mouse somewhere else");
            }
        */



    GUILayout.EndHorizontal();
        //GUI.skin.button.fontSize = 8;
        //GUI.skin.button.alignment = TextAnchor.MiddleCenter;
       bool yobro = GUILayout.Button("", mySkin.customStyles[2]);
        if (yobro)
        {
            spline.Reset();
        }
        
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(14, Screen.height-70, 500, 25));
        GUILayout.Label(sarcasticString);
        GUILayout.EndArea();
        Handles.EndGUI();
        
        

            
    
        

        // Debug.Log("before main gui loop number of points is: "+spline.ControlPointCount);
        for (int i = 1; i < spline.ControlPointCount; i += 3)
        {
       //    Debug.Log("loop index is: " + i);
            Vector3 p1 = ShowPoint(i);

            Vector3 p2 = ShowPoint(i + 1);
            // Debug.Log("loop index is i+1: " + (i+1));

            Vector3 p3 = ShowPoint(i + 2);
            // Debug.Log("loop index is i+2: " + (i+2));


            if ((spline.ShowHandlesMode == BezierShowHandlesMode.All || (spline.ShowHandlesMode == BezierShowHandlesMode.OnlySelected && (selectedIndex>-1&&(selectedIndex == i - 1|| selectedIndex == i - 2 || selectedIndex == i)||(spline.numberOfClicks-i<3&&toggleAdd)))) && !toggleRemove)
            {
                
                if (i - 2 > 0)
                {
                    Vector3 previousP =ShowPoint(i - 2);
                    Handles.color = Color.gray;
                    Handles.DrawLine(previousP, p0);
                }

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                

            }
                if (i + 2 < spline.numberOfClicks)
                {
                    Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                }

            p0 = p3;
        }
        /*if (spline.ShowDir)
        {
            ShowDirections();
        }*/

        /* code tkane from:

       http://forum.unity3d.com/threads/solved-custom-editor-onscenegui-scripting.34137/

   */
        
        // int controlID = GUIUtility.GetControlID(spline, FocusType.Passive);
       
            Event current = Event.current;

            if (current.type == EventType.layout)
            {
            //int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
                HandleUtility.AddDefaultControl(controlID);
           
            if (doTheTwist >= 0)
                {
                if (spline.numberOfClicks % 3 == 1)
                {
                    Debug.Log("throw bug?");
                    spline.numberOfClicks += 2;
                }
                spline.RemoveControlPoint(doTheTwist);
                    doTheTwist = -1;
                }
            
            }
            else if (current.type == EventType.MouseDown && toggleAdd)
            {
                if (current.button == 0)
                {
                    current.Use();
                
                sarcasticString = GetSarcasm();
                if (Camera.current != null)
                    {
                        SetPointWithMouse(current);
                        if (spline.numberOfClicks % 3 == 0 && spline.numberOfClicks > 0)
                        {
                            spline.AddCurve();
                        }
                        spline.numberOfClicks++;
                    }
                }



            }
            else if (current.type == EventType.MouseDrag && toggleAdd)
            {
                currentLife++;
                if (current.button == 0)
                {
                    current.Use();

                    if (Camera.current != null)
                    {
                        SetPointWithMouse(current);
                    }
                }
            }
            else if (current.type == EventType.MouseUp && toggleAdd)
            {
                //Debug.Log("mouseup ");         
                if (current.button == 0)
                    {
                        current.Use();  
                        spline.numberOfClicks += 2;
                    }
            }else if (current.type == EventType.MouseMove && toggleAdd)
            {
            if (spline.numberOfClicks % 3 == 1)
            {
                //Debug.Log("throw bug?");
                spline.numberOfClicks += 2;
            }
        }
                /* end of code */       
    }
    /*private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;

        for (int i = 1; i <= steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }*/

    /*
    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };*/

    private void animatePointArray(int index)
    {
            if (spline.pointAnimArray[index] < 1f)
            {
                spline.pointAnimArray[index] += 0.02f;
            }
    }
    private void SetPointWithMouse(Event current)
    {
        float[] forwardoXYZ = new float[] { Camera.current.transform.forward.x, Camera.current.transform.forward.y, Camera.current.transform.forward.z };
        float[] forwardoXYZAbs = new float[] { Mathf.Abs(Camera.current.transform.forward.x), Mathf.Abs(Camera.current.transform.forward.y), Mathf.Abs(Camera.current.transform.forward.z) };

        float[] positioXYZ = new float[] { Camera.current.transform.position.x, Camera.current.transform.position.y, Camera.current.transform.position.z };
        float maxAxis = forwardoXYZAbs.Max();
        int inverter = 1;

        maxAxisIndex = forwardoXYZAbs.ToList().IndexOf(maxAxis);
        if (forwardoXYZ[maxAxisIndex]<0)
        {
            inverter = -1;
        }

        if (lastMaxAxisIndex!=maxAxisIndex)
        {
            if(spline.numberOfClicks > 0)
            {
                Vector3 previousPos = spline.GetControlPoint(spline.numberOfClicks - 3);
                float[] previousPosArray = new float[] { previousPos.x, previousPos.y, previousPos.z };
                offset = previousPosArray[maxAxisIndex];
            }          
            lastMaxAxisIndex = maxAxisIndex;
        }

        MouseToXYZ = new Vector3(current.mousePosition.x, Camera.current.pixelHeight - current.mousePosition.y, (-positioXYZ[maxAxisIndex] * forwardoXYZ[maxAxisIndex])+(offset* inverter));
        //MouseToXYZ = handleTransform.TransformPoint(MouseToXYZ);
       // Debug.Log("bla "+ Camera.current.WorldToScreenPoint(handleTransform.transform.position));
      //  Debug.Log("bla " + Camera.current.ScreenToWorldPoint(MouseToXYZ));
        
        spline.SetControlPoint(spline.numberOfClicks, handleTransform.InverseTransformPoint(Camera.current.ScreenToWorldPoint(MouseToXYZ)));
    }

    private Vector3 ShowPoint(int index)
    {
        //Debug.Log("begin of show point");
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.black;


        if (index > spline.numberOfClicks-1 || ((index-2) % 3 == 0 && index == spline.numberOfClicks-1))
        {
            size = 0;
        }
        else
        {
            if (index % 3 == 0)
            {
                float sizo = 0f;
                Quaternion rota = Camera.current.transform.rotation;
                if (spline.pointAnimArray.Count > index)
                {
                    animatePointArray(index);
                    sizo = spline.GizmoSize*spline.pointAnimArray[index];
                }

                if (spline.numberOfClicks - index <= 3)
                {
                    Handles.color = Color.white;
                }
                


                if (spline.numberOfClicks > 0)
                {
                    Vector3 rot=Camera.current.transform.right;
                    if (index == 0)
                    {
                        rot = (spline.GetControlPoint(index + 1) - spline.GetControlPoint(index)).normalized;
                    }
                    else
                    {
                        rot = (spline.GetControlPoint(index + 1) - spline.GetControlPoint(index - 1)).normalized;

                    }
                    rota = Quaternion.LookRotation(rot);
                }

                if (spline.GetControlPointMode(index)== BezierControlPointMode.Mirrored)
                {
                    
                    
                    
                    
                    
                    Handles.CircleCap(0,
                        point,
                        rota,sizo
                        );
                    

                }
                else if (spline.GetControlPointMode(index) == BezierControlPointMode.Aligned)
                    {
                    Handles.RectangleCap(0,
                        point,
                        rota,
                        sizo);
                }
                else
                {

                    Vector3 point1 = point + Camera.current.transform.up* sizo;
                    Vector3 point2 = point - Camera.current.transform.up * sizo + Camera.current.transform.right * sizo;
                    Vector3 point3 = point - Camera.current.transform.up * sizo - Camera.current.transform.right * sizo;
                    Handles.DrawLine(point1,point2);
                    Handles.DrawLine(point2, point3);
                    Handles.DrawLine(point3, point1);
                    
                }
                
                
                //size *= 0.5f;
           

            }
            
        }
        /* following logic isn't great but it works... bit too drunk when I coded it I suppose*/
        //Debug.Log("selected index % 3 " + selectedIndex%3);
        //Debug.Log("spline.numberofclicks " + spline.numberOfClicks);
        if (index%3==0) { 
            if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
            {

                if (toggleRemove)
                {

                    // point = Handles.DoPositionHandle(point, handleRotation);
                    doTheTwist = index;
                    //   Undo.RecordObject(spline, "Remove Point");
                    //   EditorUtility.SetDirty(spline);
                    //spline.RemoveControlPoint(index);
                    // return null;
                }
                else
                {
                    selectedIndex = index;
                }
                Repaint();
            }
        }else if (index==selectedIndex || (index%3!=0 && !toggleRemove && (spline.ShowHandlesMode == BezierShowHandlesMode.All||(spline.ShowHandlesMode == BezierShowHandlesMode.OnlySelected && ((toggleAdd && ((spline.numberOfClicks - index < 3&&index%3!=2)|| (spline.numberOfClicks-index<5&&index%3==2))) || (selectedIndex%3==0 && Mathf.Max(selectedIndex,index) - Mathf.Min(selectedIndex, index) < 2)||(selectedIndex % 3 == 1 &&  index==selectedIndex-2) || (selectedIndex % 3 == 2 && index==selectedIndex+2))))))
        {
            if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
            {
                selectedIndex = index;
                Repaint();
            }
        }
		if (selectedIndex == index) {
           // Debug.Log("inside selectedindex=index");
          
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                /* if (toggleAdd)
                 {
                     spline.SetControlPoint(index, handleTransform.TransformPoint(point));
                 }
                 else
                 {
                     spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                 }*/
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
            }
            
		
		return point;
	}

}