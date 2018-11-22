using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiColor : MonoBehaviour {

    public Color c;

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().material.SetColor("_Color",c);
	}

    public void SetColor(Color newColor)
    {
        c = newColor;
        GetComponent<Renderer>().material.SetColor("_Color", c);
    }
}
