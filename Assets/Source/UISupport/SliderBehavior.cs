using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBehavior : MonoBehaviour {


    public SliderMode mode;
    public Text echo;
    public SliderPanel panel;
    private bool updating = false;
    public enum SliderMode {
        width, height, widthRes, heightRes, angle
    }

    public void Awake()
    {
        GetComponent<Slider>().onValueChanged.AddListener(OnSliderChanged);
        echo.text = GetComponent<Slider>().value.ToString();
        panel.AddListener(OnViewChanged);
    }

    public void OnViewChanged(SimpleMesh sm)
    {
        updating = true;
        switch (mode)
        {
            case SliderMode.width:
                {
                    GetComponent<Slider>().value = sm.worldWidth;
                    break;
                }
            case SliderMode.height:
                {
                    GetComponent<Slider>().value = sm.worldHeight;
                    break;
                }
            case SliderMode.widthRes:
                {
                    GetComponent<Slider>().value = sm.widthRes;
                    break;
                }
            case SliderMode.heightRes:
                {
                    GetComponent<Slider>().value = sm.heightRes;
                    break;
                }
            case SliderMode.angle:
                {
                    CylinderMesh cm = sm.gameObject.GetComponent<CylinderMesh>();
                    if(cm != null)
                        GetComponent<Slider>().value = cm.rotation;
                    break;
                }
        }
        updating = false;
    }

    private void OnSliderChanged(float f)
    {
        echo.text = f.ToString();
        if (!updating)
        {
            switch (mode)
            {
                case SliderMode.width:
                    {
                        panel.UpdateWidth(f);
                        break;
                    }
                case SliderMode.height:
                    {
                        panel.UpdateHeight(f);
                        break;
                    }
                case SliderMode.widthRes:
                    {
                        panel.UpdateResWidth((int)f);
                        break;
                    }
                case SliderMode.heightRes:
                    {
                        panel.UpdateResHeight((int)f);
                        break;
                    }
                case SliderMode.angle:
                    {
                        panel.UpdateAngle((int)f);
                        break;
                    }
            }
        }
    }
}
