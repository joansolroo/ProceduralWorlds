using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralCapsule2D : MonoBehaviour {


    [SerializeField] float radius = 1f;
    [SerializeField] float roundiness = 1f;

    [SerializeField] Vector3[] line;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnValidate()
    {
       // line = new Vector3[2];line[0] = new Vector3(0,1,1); line[line.Length-1] = new Vector3(0, -1, 1);
        /*for(int idx = 0; idx < line.Length; ++idx)
        {
            line[idx] = Vector3.Lerp(line[0], line[line.Length - 1],((float)idx)/line.Length);
        }*/
        Generate();
    }
    MeshFilter filter;
    void Generate()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = mesh = new Mesh();

        filter.sharedMesh = mesh;
        mesh.name = "Capsule2D";
       
        mesh.Clear();

        #region Vertices
        int cap = 10;
        Vector3[] vertices = new Vector3[(cap *2) + line.Length];
        Vector3[] normales = new Vector3[vertices.Length];

        //vertices[0] = line[0];
        for(int l =0; l < line.Length; ++l)
        {
            vertices[l] = line[l];
        }
        int idx = 1;
        for(idx = 0; idx < cap; ++idx)
        {
            float a = idx / ((cap * 2f)-2) * 360 * Mathf.Deg2Rad;
            float x = Mathf.Cos(a);
            float y = Mathf.Sin(a);
            vertices[idx+line.Length] = new Vector3(x, y, 0)* roundiness + line[0];
            normales[idx + line.Length] = new Vector3(0, 0, 1);
        }
        for (idx = cap; idx < 2*cap; ++idx)
        {
            float a = (idx-1) / ((cap * 2f)-2) * 360 * Mathf.Deg2Rad;
            float x = Mathf.Cos(a);
            float y = Mathf.Sin(a);
            vertices[idx + line.Length] = new Vector3(x, y, 0)*roundiness + line[line.Length-1];
            normales[idx + line.Length] = new Vector3(0, 0, 1);
        }

        for (idx = 0; idx < vertices.Length; ++idx)
        {
            normales[idx] = vertices[idx].normalized;
            vertices[idx] = normales[idx]*radius;
            
        }

        #endregion
        /*float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        vertices[0] = Vector3.up * radius;
        for (int lat = 0; lat < nbLat; lat++)
        {
            float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++)
            {
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
            }
        }
        vertices[vertices.Length - 1] = Vector3.up * -radius;?/
        #endregion
        /*
        #region Normales		
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < vertices.Length; n++)
            normales[n] = vertices[n].normalized;
        #endregion
    */
        /*
            #region UVs
            Vector2[] uvs = new Vector2[vertices.Length];
            uvs[0] = Vector2.up;
            uvs[uvs.Length - 1] = Vector2.zero;
            for (int lat = 0; lat < nbLat; lat++)
                for (int lon = 0; lon <= nbLong; lon++)
                    uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
            #endregion
        */

        #region Triangles
        int nbFaces = vertices.Length;
        int nbTriangles = cap*2+4;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap
        int i = 0;
        for (idx = line.Length; idx < cap+ line.Length-1; idx++)
        {
            triangles[i++] = 0;
            triangles[i++] = idx;
            triangles[i++] = idx+1;
            //break;
        }
        for (idx = line.Length+cap; idx < cap*2 + line.Length - 1; idx++)
        {
            triangles[i++] = 1;
            triangles[i++] = idx;
            triangles[i++] = idx + 1;
            //break;
        }
        triangles[i++] = 1;
        triangles[i++] = vertices.Length-1;
        triangles[i++] = 0;

        triangles[i++] = 0;
        triangles[i++] = line.Length+cap-1;
        triangles[i++] = 1;

        triangles[i++] = line.Length + cap;
        triangles[i++] = 1;
        triangles[i++] = line.Length + cap - 1;

        triangles[i++] = line.Length;
        triangles[i++] = 0;
        triangles[i++] = vertices.Length - 1;

        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        //mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(Vector3.zero), 0.03f);
        if (filter != null) {
            for (int idx = 0; idx < filter.mesh.vertices.Length; ++idx)
            {
                Gizmos.DrawSphere(transform.TransformPoint(filter.mesh.vertices[idx]), 0.02f);
                Gizmos.DrawLine(transform.TransformPoint(filter.mesh.vertices[idx]), transform.TransformPoint(filter.mesh.normals[idx]*0.1f+filter.mesh.vertices[idx]));
            }
        }
    }*/
}

