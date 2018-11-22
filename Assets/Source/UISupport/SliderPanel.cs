using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPanel : MonoBehaviour {

    public SimpleMesh sm;
    public GameObject cylinder;
    public GameObject plane;
    public delegate void ViewListener(SimpleMesh sm);
    List<ViewListener> listeners = new List<ViewListener>();

    public void AddListener(ViewListener vl)
    {
        listeners.Add(vl);
    }

    public void SwitchViewingShape(int index)
    {
        if(index == 0)
        {
            //Cylinder
            plane.SetActive(false);
            cylinder.SetActive(true);
            sm = cylinder.GetComponent<SimpleMesh>();
            foreach(ViewListener vl in listeners)
            {
                vl.Invoke(sm);
            }
        } else
        {
            //Plane
            cylinder.SetActive(false);
            plane.SetActive(true);
            sm = plane.GetComponent<SimpleMesh>();
            foreach (ViewListener vl in listeners)
            {
                vl.Invoke(sm);
            }
        }
    }

    public void Start()
    {
        SwitchViewingShape(0);
    }

    public void UpdateWidth(float val)
    {
        sm.worldWidth = val;
        sm.Reset();
    }
    public void UpdateHeight(float val)
    {
        sm.worldHeight = val;
        sm.Reset();
    }
    public void UpdateResWidth(int val)
    {
        sm.widthRes = val;
        sm.Reset();
    }
    public void UpdateResHeight(int val)
    {
        sm.heightRes = val;
        sm.Reset();
    }
    public void UpdateAngle(int angle)
    {
        CylinderMesh cm = sm.gameObject.GetComponent<CylinderMesh>();
        if(cm != null)
        {
            cm.rotation = angle;
            sm.Reset();
        }
    }
}
