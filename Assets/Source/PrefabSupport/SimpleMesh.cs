using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMesh : MonoBehaviour {

    protected Mesh mesh;
    public int widthRes = 4; //Number of vertexes horizontally
    public int heightRes = 4; //Number of vertexes vertically
    public float worldWidth = 4;
    public float worldHeight = 4;
    protected GameObject[] vertexObjects;
    protected GameObject[] normalObjects;
    private Vector2[] uv_list;
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    // Use this for initialization
    void Start () {
        Initialize();

	}

    private int[] adjacentSequence = { -5, -4, -3, 1, 5, 4, 3, -1, -5 };
     
    private void Update()
    {
        TrackAnchors();
        UpdateNormals();
    }

    protected virtual void TrackAnchors()
    {
        //Set vertex positions to their anchors
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            vertices[i] = vertexObjects[i].transform.localPosition;
        }
        mesh.vertices = vertices;
    }

    private void UpdateNormals()
    {
        Vector3[] meshNormals = mesh.normals;

        //Compute normals for each triangle
        Vector3[] triangleNormals = new Vector3[mesh.triangles.Length / 3];
        int triNormIndex = 0;
        for (int tri = 0; tri < mesh.triangles.Length; tri += 3)
        {
            Vector3 v0 = mesh.vertices[mesh.triangles[tri]];
            Vector3 v1 = mesh.vertices[mesh.triangles[tri + 1]];
            Vector3 v2 = mesh.vertices[mesh.triangles[tri + 2]];
            triangleNormals[triNormIndex] = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            triNormIndex++;
        }

        //Update their normals
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (!DisplayVertex(i)) continue;
            //There can be up to 6 adjacent triangles...
            Vector3[] normals = new Vector3[6];
            int adjacent = 0;

            //First find what edges we're on
            bool isTop = i < widthRes;
            bool isBottom = i >= mesh.vertices.Length - widthRes;
            bool isLeft = i % widthRes == 0;
            bool isRight = i % widthRes == widthRes - 1;

            if (!isTop && !isLeft)
            {
                //upperLeft square
                int squareIndex = i - widthRes - 1;
                int x = squareIndex % widthRes;
                int y = (int)Mathf.Floor(squareIndex / widthRes);
                int triangle1 = y * (widthRes - 1) * 2 + x * 2;
                int triangle2 = triangle1 + 1;
                normals[adjacent] = triangleNormals[triangle1];
                normals[adjacent + 1] = triangleNormals[triangle2];
                adjacent += 2;
            }
            if (!isBottom && !isRight)
            {
                //lower right square
                int squareIndex = i;
                int x = squareIndex % widthRes;
                int y = (int)Mathf.Floor(squareIndex / widthRes);
                int triangle1 = y * (widthRes - 1) * 2 + x * 2;
                int triangle2 = triangle1 + 1;
                normals[adjacent] = triangleNormals[triangle1];
                normals[adjacent + 1] = triangleNormals[triangle2];
                adjacent += 2;
            }
            if (!isRight && !isTop)
            {
                //upper right triangle
                int squareIndex = i - widthRes;
                int x = squareIndex % widthRes;
                int y = (int)Mathf.Floor(squareIndex / widthRes);
                int triangle1 = y * (widthRes - 1) * 2 + x * 2;
                normals[adjacent] = triangleNormals[triangle1];
                adjacent += 1;
            }
            if (!isLeft && !isBottom)
            {
                //lower left triangle
                int squareIndex = i - 1;
                int x = squareIndex % widthRes;
                int y = (int)Mathf.Floor(squareIndex / widthRes);
                int triangle1 = y * (widthRes - 1) * 2 + x * 2 + 1;
                normals[adjacent] = triangleNormals[triangle1];
                adjacent += 1;
            }

            //If this is a cylinder that's wrapping, we may be able to use triangles on the other side
            if(isLeft && IsWrapped())
            {
                if(!isTop)
                {
                    //Upper left wrapped
                    int squareIndex = i - 2;
                    int x = squareIndex % widthRes;
                    int y = (int)Mathf.Floor(squareIndex / widthRes);
                    int triangle1 = y * (widthRes - 1) * 2 + x * 2;
                    int triangle2 = triangle1 + 1;
                    normals[adjacent] = triangleNormals[triangle1];
                    normals[adjacent + 1] = triangleNormals[triangle2];
                    adjacent += 2;
                }
                if(!isBottom)
                {
                    //lower left triangle
                    int squareIndex = i + widthRes - 2;
                    int x = squareIndex % widthRes;
                    int y = (int)Mathf.Floor(squareIndex / widthRes);
                    int triangle1 = y * (widthRes - 1) * 2 + x * 2 + 1;
                    normals[adjacent] = triangleNormals[triangle1];
                    adjacent += 1;
                }
            }


            Vector3 resultNormal = Vector3.zero;
            for (int k = 0; k < adjacent; k++)
            {
                resultNormal += normals[k];
            }
            resultNormal = resultNormal.normalized;
            meshNormals[i] = resultNormal;

            //Update the cylinder
            normalObjects[i].transform.localRotation = transform.localRotation * Quaternion.FromToRotation(Vector3.up, resultNormal);
            normalObjects[i].transform.localPosition = vertexObjects[i].transform.position + normalObjects[i].transform.up * normalObjects[i].transform.localScale.y;

            //Update the display information
            normalObjects[i].GetComponent<NormalInfo>().adjacentTriangles = adjacent;
            normalObjects[i].GetComponent<NormalInfo>().adjacentNormals = normals;
            normalObjects[i].GetComponent<NormalInfo>().resultNormal = resultNormal;
            normalObjects[i].GetComponent<NormalInfo>().vertexID = i;
            normalObjects[i].GetComponent<NormalInfo>().isLeft = isLeft;
            normalObjects[i].GetComponent<NormalInfo>().isRight = isRight;
            normalObjects[i].GetComponent<NormalInfo>().isTop = isTop;
            normalObjects[i].GetComponent<NormalInfo>().isBottom = isBottom;
        }

        mesh.normals = meshNormals;
    }

    protected virtual Vector3 GetVertexStartingPosition(int x, int y) 
    {
        return new Vector3((float)x * worldWidth / (widthRes - 1), 0, (float)y * worldHeight / (heightRes - 1) * -1);
    }

    protected virtual bool IsVertexEditable(int id)
    {
        int x = id % widthRes;
        int y = (int)Mathf.Floor(id / widthRes);
        return true;
    }

    protected virtual bool IsWrapped()
    {
        return false;
    }

    private void SetupVertices()
    {
        //Let's say 0,0 is top left of our mesh, easier to handle with variable size meshes
        //  x,y will be the vertex
        Vector3[] v = new Vector3[(widthRes) * (heightRes)]; //Number of vertices
        Color[] c = new Color[v.Length];
        Vector3[] n = new Vector3[v.Length]; //Number of normals (same as number of vertices)
        vertexObjects = new GameObject[v.Length];
        normalObjects = new GameObject[v.Length];

        int index = 0;
        for (int y = 0; y < heightRes; y++)
        {
            for (int x = 0; x < widthRes; x++)
            {
                v[index] = GetVertexStartingPosition(x, y);

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(transform);
                sphere.transform.localScale = new Vector3(.2f, .2f, .2f);
                sphere.transform.localPosition = v[index];
                
                sphere.AddComponent<MultiColor>();
                if(!IsVertexEditable(index))
                {
                    sphere.GetComponent<MultiColor>().SetColor(Color.black);
                } else
                {
                    sphere.GetComponent<MultiColor>().SetColor(Color.white);
                }
                vertexObjects[index] = sphere;

                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.transform.localScale = new Vector3(.02f, .5f, .02f);
                cylinder.AddComponent<NormalInfo>();
                normalObjects[index] = cylinder;

                c[index] = (((index % widthRes) + (int)Mathf.Floor(index / widthRes)) % 2 == 0) ? Color.white : Color.white;
                index++;
            }
        }
        mesh.vertices = v;
        mesh.colors = c;
        mesh.normals = n;
    }

    private void CreateTriangles()
    {
        int[] t = new int[(widthRes - 1) * (heightRes - 1) * 2 * 3]; //Number of triangles

        int triangle = 0;
        int triNumber = 1;
        //Then draw each square, where a triangle is two squares
        for (int squareY = 0; squareY < heightRes - 1; squareY++)
        {
            for (int squareX = 0; squareX < widthRes - 1; squareX++)
            {
                //Vertex in the top left of this square
                int topLeftIndex = squareY * (widthRes) + squareX;

                //Lower left triangle
                t[triangle] = topLeftIndex + widthRes;
                t[triangle + 1] = topLeftIndex;
                t[triangle + 2] = topLeftIndex + widthRes + 1;

                //Upper right triangle
                t[triangle + 3] = topLeftIndex + widthRes + 1;
                t[triangle + 4] = topLeftIndex;
                t[triangle + 5] = topLeftIndex + 1;

                triNumber += 2;
                triangle += 6;
            }
        }

        mesh.triangles = t;
    }

    //Recreates the mesh with the current resolution
    private void Initialize()
    {
        mesh.Clear(); //Clear existing mesh
        SetupVertices();
        CreateTriangles();
        VertexMeld();       
        uv_list = new Vector2[widthRes*heightRes]; //need a UV for each vertex
        mapUV();
        mesh.uv = uv_list;
        GetComponent<TexturePlacement>().SaveInitUV(uv_list);
    }

    private void mapUV()
    {
        float x_step = 1 / ((float)widthRes - 1);
        float y_step = 1 / ((float)heightRes - 1);
        int uv_index = 0;
        for (float y_v = 0; y_v < heightRes; y_v++)
        {
            float y_pos = y_v * y_step;
            for (float x_v = 0; x_v < widthRes; x_v++)
            {
                float x_pos = x_v * x_step;
                uv_list[uv_index] = new Vector2(x_pos, y_pos);
                uv_index++;
            }
        }        
    }

    //Updates triangles on the seem of a cylinder to use the opposite seem vertexes
    protected virtual void VertexMeld()
    {

    }

    //Hides edge vertices for full cylinders
    protected virtual bool DisplayVertex(int id)
    {
        return true;
    }

    private void ClearSetup()
    {
        foreach(GameObject o in normalObjects)
        {
            GameObject.Destroy(o);
        }
        foreach(GameObject o in vertexObjects)
        {
            GameObject.Destroy(o);
        }
        mesh.Clear();
    }

    private void OnEnable()
    {
        if (normalObjects != null)
        {
            foreach (GameObject go in normalObjects)
            {
                if (go != null)
                    go.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        if (normalObjects != null)
        {
            foreach (GameObject go in normalObjects)
            {
                if(go != null)
                go.SetActive(false);
            }
        }
    }

    public void Reset()
    {
        ClearSetup();
        Initialize();
    }

}
