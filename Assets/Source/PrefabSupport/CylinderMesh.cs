using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMesh : SimpleMesh {

    public int rotation = 270;

    //Each x is one partial rotation away, y go vertically
    //  "worldWidth" is the starting diameter of the cylinder
    //  "worldHeight" is the starting height of the cylinder
    protected override Vector3 GetVertexStartingPosition(int x, int y)
    {
        Vector3 beforeRotate = new Vector3(worldWidth / 2, 0, 0);
        float degrees = (float)rotation / (float)(widthRes - 1) * (float)x;
        Quaternion q = Quaternion.AngleAxis(degrees, Vector3.up);
        Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        Vector3 afterRotate = r.MultiplyPoint(beforeRotate);
        return new Vector3(afterRotate.x, (float)y * worldHeight / (heightRes - 1), afterRotate.z);
    }

    //Used to handle the legwork of melding edge vertices in a completed cylinder
    protected override void VertexMeld()
    {
        if(rotation == 360)
        {
            //First loop through the triangles,
            //  any vertex reference on the right edge should be switched to the left edge
            int[] t = mesh.triangles;
            for (int i = 0; i < t.Length; i++)
            {
                //If the referenced vertex is on the right edge...
                if (t[i] % widthRes == widthRes - 1)
                {
                    GameObject anchorObj = vertexObjects[t[i]];
                    if (anchorObj != null) GameObject.Destroy(anchorObj);
                    GameObject normalObj = normalObjects[t[i]];
                    if (normalObj != null) GameObject.Destroy(normalObj);
                    t[i] = t[i] - widthRes + 1;
                }
            }
            mesh.triangles = t;

        }
    }

    protected override bool IsWrapped()
    {
        return rotation == 360;
    }

    //Hides edge vertices for cylinders
    protected override bool DisplayVertex(int id)
    {
        return (rotation < 360 || id % widthRes != widthRes - 1);
    }

    protected override void TrackAnchors()
    {
        //Set each vertex to the rotated position from its edge
        int index = 0;
        Vector3[] v = mesh.vertices;
        for (int y = 0; y < heightRes; y++)
        {
            Vector3 edgePos = vertexObjects[index].transform.localPosition;
            for (int x = 0; x < widthRes; x++)
            {
                if (DisplayVertex(index))
                {
                    Vector3 beforeRotate = edgePos;
                    float degrees = (float)rotation / (float)(widthRes - 1) * (float)x;
                    Quaternion q = Quaternion.AngleAxis(degrees, Vector3.up);
                    Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
                    Vector3 afterRotate = r.MultiplyPoint(beforeRotate);
                    v[index] = afterRotate;
                    vertexObjects[index].transform.localPosition = afterRotate;
                }
                index++;
            }
        }

        mesh.vertices = v;
    }

    protected override bool IsVertexEditable(int id)
    {
        int x = id % widthRes;
        return x == 0;
    }

}
