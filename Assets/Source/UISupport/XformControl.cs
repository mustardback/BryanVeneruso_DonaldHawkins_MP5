using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XformControl : MonoBehaviour {
    public Toggle T, R, S;
    public SliderWithEcho X, Y, Z;
    public GameObject pMesh;
    public GameObject cMesh;
    private TexturePlacement pTexture;
    private TexturePlacement cTexture;



    // Use this for initialization
    void Start () {
        pTexture = pMesh.GetComponent<TexturePlacement>();
        cTexture = cMesh.GetComponent<TexturePlacement>();
        T.onValueChanged.AddListener(SetToTranslation);
        R.onValueChanged.AddListener(SetToRotation);
        S.onValueChanged.AddListener(SetToScaling);
        X.SetSliderListener(XValueChanged);
        Y.SetSliderListener(YValueChanged);
        Z.SetSliderListener(ZValueChanged);

        T.isOn = true;
        R.isOn = false;
        S.isOn = false;
        SetToTranslation(true);
	}  
	  
    //---------------------------------------------------------------------------------
    // Initialize slider bars to specific function
    void SetToTranslation(bool v) 
    {
        X.InitSliderRange(-4, 4,0);
        Y.InitSliderRange(-4, 4, 0);
        Z.InitSliderRange(0, 0, 0);
    }

    void SetToScaling(bool v)
    {
        X.InitSliderRange(0.1f, 20, 1);
        Y.InitSliderRange(0.1f, 20, 1);
        Z.InitSliderRange(1, 1, 1);
    }

    void SetToRotation(bool v)
    {
        X.InitSliderRange(0, 0, 0);
        Y.InitSliderRange(0, 0, 0);
        Z.InitSliderRange(-180, 180, 0);
    }
    //---------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------
    // resopond to sldier bar value changes
    void XValueChanged(float v)
    {
        if (T.isOn)
        {
            pTexture.Offset.x = v;
            cTexture.Offset.x = v;
        }
        else if (S.isOn)
        {
            pTexture.Scale.x = v;
            cTexture.Scale.x = v;
        }
    }
    
    void YValueChanged(float v)
    {
        if (T.isOn)
        {
            pTexture.Offset.y = v;
            cTexture.Offset.y = v;
        }
        else if (S.isOn)
        {
            pTexture.Scale.y = v;
            cTexture.Scale.y = v;
        }
    }

    void ZValueChanged(float v)
    {
        if (R.isOn)
        {
            pTexture.Rotation = v;
            cTexture.Rotation = v;
        }
    }
}