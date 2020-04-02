using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
[CustomEditor(typeof(SliderControl))]
//[CustomEditor(typeof(move))]
[ExecuteInEditMode]
[InitializeOnLoadAttribute]
[CanEditMultipleObjects]

public  class SliderControl : Editor
{
   
    static bool s_all;
    static float multi = 0.1f;
   // static int multiint;
    static float posrotx;
    static float posroty;
    static float posrotz;
    Vector3 posrot;
    static SliderControl() //INIT
    {
        Debug.Log(Tools.current);
       
    }

   
   
   
    [MenuItem("Controller/IncMulti _=")]
    static void IncMulti()
    {
        SliderControl.multi *= 10f;
        Debug.Log(SliderControl.multi);
       
    }
    [MenuItem("Controller/DecMulti _-")]
    static void DecMulti()
    {
        SliderControl.multi *= 0.1f;
        Debug.Log(SliderControl.multi);
        
    }
    
    static void MovePosition(int axis, float scaleFactor)
    {
        foreach (Transform obj in Selection.transforms)
        {
            Vector3 objTransform = obj.transform.position;
            Undo.RecordObject(obj.transform, "Zero Transform Position");
            switch (axis)
            {
                case 1:
                    objTransform.x += (multi*scaleFactor);
                    break;
                case 2:
                    objTransform.x -= (multi * scaleFactor);
                    break;
                case 3:
                    objTransform.y -= (multi * scaleFactor);
                    break;
                case 4:
                    objTransform.y += (multi * scaleFactor);
                    break;
                case 5:
                    objTransform.z -= (multi * scaleFactor);
                    break;
                case 6:
                    objTransform.z += (multi * scaleFactor);
                    break;
            }
           
            obj.transform.position = objTransform;
        }
    }
    static void RotateObjects(int axis, float scaleFactor)
    {
        foreach (Transform obj in Selection.transforms)
        {
            posrotx = obj.transform.eulerAngles.x;
            posroty = obj.transform.eulerAngles.y;
            posrotz = obj.transform.eulerAngles.z;
            Undo.RecordObject(obj.transform, "Zero Transform Position");
           // posrotz -= multi; //x and z swapped
            switch (axis)
            {
                case 1:
                    posrotz -= (multi * scaleFactor);
                    break;
                case 2:
                    posrotz += (multi * scaleFactor);
                    break;
                case 3:
                    posroty -= (multi * scaleFactor);
                    break;
                case 4:
                    posroty += (multi * scaleFactor);
                    break;
                case 5:
                    posrotx -= (multi * scaleFactor);
                    break;
                case 6:
                    posrotx += (multi * scaleFactor);
                    break;
            }
            obj.transform.eulerAngles = new Vector3(posrotx, posroty, posrotz);
        }
    }
    static void ScaleObjects(int axis, float scaleFactor) // scale one axis
    {
        foreach (Transform obj in Selection.transforms)
        {
            Vector3 objScale = obj.transform.localScale;
            Undo.RecordObject(obj.transform, "Zero Transform Position");
            switch (axis)
            {
                case 1:
                    objScale.x += (multi * scaleFactor);
                    break;
                case 2:
                    objScale.x -= (multi * scaleFactor);
                    break;
                case 3:
                    objScale.y -= (multi * scaleFactor);
                    break;
                case 4:
                    objScale.y += (multi * scaleFactor);
                    break;
                case 5:
                    objScale.z -= (multi * scaleFactor);
                    break;
                case 6:
                    objScale.z += (multi * scaleFactor);
                    break;
            }
           
            obj.transform.localScale = objScale;
        }
    }
    static void ScaleObjects(bool up, float scaleFactor) //scale all axis
    {
            foreach (Transform obj in Selection.transforms)
            {
                Vector3 posscaleall = obj.transform.localScale;
                Undo.RecordObject(obj.transform, "Zero Transform Position");
            if (up)
            {
                posscaleall.x += (multi * scaleFactor);
                posscaleall.y += (multi * scaleFactor);
                posscaleall.z += (multi * scaleFactor);
            }
            else
            {
                posscaleall.x -= (multi * scaleFactor);
                posscaleall.y -= (multi * scaleFactor);
                posscaleall.z -= (multi * scaleFactor);
            }
                obj.transform.localScale = posscaleall;
            }
    }
    static void CallFunctions(int axis, bool up, float scaleFactor)
    {
        
        if (Tools.current.ToString() == "Move")
        {
            MovePosition(axis, scaleFactor);
        }
        if (Tools.current.ToString() == "Rotate")
        {
            RotateObjects(axis, scaleFactor);
        }
        if (Tools.current.ToString() == "Scale")
        {
            ScaleObjects(axis, scaleFactor);
        }
        if (Tools.current.ToString() == "Rect")
        {
            ScaleObjects(up, scaleFactor);
        }
    }

    [MenuItem("Controller/Movex %#, ")] // ctl shift ,
    static void Movex()
    {
        CallFunctions(1, true, 1);
    }
    [MenuItem("Controller/MovexFine %, ")] // ctl  ,
    static void MovexFine()
    {
        CallFunctions(1, true, 0.1f);
    }

    [MenuItem("Controller/Movexneg  %#;")]// ctl shift ;
    static void Movexneg()
    {
        CallFunctions(2, false, 1);
    }
    [MenuItem("Controller/MovexnegFine  %;")]// ctl  ;
    static void MovexnegFine()
    {
        CallFunctions(2, false, 0.1f);
    }
    [MenuItem("Controller/Movey  %#j")] // ctl shift j
    static void Movey()
    {
        CallFunctions(3, false, 1);
    }
    [MenuItem("Controller/MoveyFine  %j")] // ctl  j
    static void MoveyFine()
    {
        CallFunctions(3, false, 0.1f);
    }

    [MenuItem("Controller/Movenegy  %#i")] // ctl shift i
    static void Movenegy()
    {
        CallFunctions(4, true, 1);
    }
    [MenuItem("Controller/MovenegyFine  %i")] // ctl i
    static void MovenegyFine()
    {
        CallFunctions(4, true, 0.1f);
    }

    [MenuItem("Controller/Movez  %#l")]  // ctl shift g
    static void Movez()
    {
        CallFunctions(5, false, 1 );
    }
    [MenuItem("Controller/MovezFine  %&l")]  // ctl alt l
    static void MovezFine()
    {
        CallFunctions(5, false, 0.1f);
    }
    [MenuItem("Controller/Movenegz  %#h")] //ctl shift h
    static void Movenegz()
    {
        CallFunctions(6, true, 1);
    }
    [MenuItem("Controller/MovenegzFine  %&h")] //ctl alt  h
    
    static void MovenegzFine()
    {
        CallFunctions(6, true, 0.1f);
    }

}