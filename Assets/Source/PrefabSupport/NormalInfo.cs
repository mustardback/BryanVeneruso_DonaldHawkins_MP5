using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalInfo : MonoBehaviour {

    public int adjacentTriangles;
    public Vector3[] adjacentNormals = new Vector3[6];
    public Vector3 resultNormal;
    public int vertexID;
    public bool isLeft;
    public bool isRight;
    public bool isTop;
    public bool isBottom;
}
