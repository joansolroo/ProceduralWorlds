using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DebugMesh : MonoBehaviour
{

    [SerializeField] bool DrawVertices = true;
    [SerializeField] bool DrawNormals = true;
    [SerializeField] bool DrawTriangles = false;
    [SerializeField] bool DrawAntiTriangles = false;
    MeshFilter filter;


    public class Comp : IComparer<int>
    {
        public int point;
        public List<Vector3> IndexedList;
        public int Compare(int p1, int p2)
        {
            return (IndexedList[p1] - IndexedList[point]).sqrMagnitude.CompareTo((IndexedList[p2] - IndexedList[point]).sqrMagnitude);
        }
    }

    private void OnDrawGizmos()
    {
        if (filter == null)
        {
            filter = GetComponent<MeshFilter>();
        }
        Vector3[] vertices = filter.mesh.vertices;

        Vector3[] normals = filter.mesh.normals;
        for (int v = 0; v < vertices.Length; ++v)
        {
            vertices[v] = transform.TransformPoint(vertices[v]);
            normals[v] = transform.TransformDirection(normals[v]);
        }
        int[] triangles = filter.mesh.triangles;

        //Gizmos.DrawWireSphere(transform.TransformPoint(Vector3.zero), 0.03f);
        if (filter != null)
        {
            if (DrawVertices)
            {
                Gizmos.color = Color.green;
                for (int idx = 0; idx < filter.mesh.vertices.Length; ++idx)
                {
                    Gizmos.DrawSphere(vertices[idx], 0.02f);

                }
            }
            if (DrawNormals)
            {
                Gizmos.color = Color.blue;
                for (int idx = 0; idx < filter.mesh.vertices.Length; ++idx)
                {
                    Gizmos.DrawLine(vertices[idx], normals[idx] * 0.1f + vertices[idx]);
                }
            }
            if (DrawTriangles)
            {
                Gizmos.color = Color.black;
                for (int t = 0; t < triangles.Length; t += 3)
                {
                    Gizmos.DrawLine(vertices[triangles[t]], vertices[triangles[t + 1]]);
                    Gizmos.DrawLine(vertices[triangles[t + 1]], vertices[triangles[t + 2]]);
                    Gizmos.DrawLine(vertices[triangles[t]], vertices[triangles[t + 2]]);

                }
            }

            if (DrawAntiTriangles)
            {
                List<Vector3> centers = new List<Vector3>();
                List<int>[] polygons = new List<int>[vertices.Length];
                Gizmos.color = Color.white;
                int inv = 0;

                for (int t = 0; t < triangles.Length; t += 3)
                {
                    Vector3 center = (vertices[triangles[t]] + vertices[triangles[t + 1]] + vertices[triangles[t + 2]]) / 3;

                    centers.Add(center);
                    for (int d = 0; d < 3; ++d)
                    {
                        if (polygons[triangles[t + d]] == null)
                        {
                            polygons[triangles[t + d]] = new List<int>();
                        }
                        polygons[triangles[t + d]].Add(inv);
                    }
                    ++inv;

                   // Gizmos.DrawSphere(center, 0.1f);
                }

                for (int pol = 0; pol < polygons.Length; ++pol)
                {
                    
                    //if (pol > 3)
                    //    break;
                    //HashSet<int> links = new HashSet<int>();
                    List<int> shape = new List<int>();
                    List<int> unusedPoints = new List<int>(polygons[pol]);
                    shape.Add(unusedPoints[0]);
                    unusedPoints.RemoveAt(0);
                  
                    int p = 0;
                    while (unusedPoints.Count > 0)
                    {
                        int min = 0;
                        float d = (centers[shape[p]] - centers[unusedPoints[0]]).sqrMagnitude;
                        
                        for (int p2 = 1; p2 < unusedPoints.Count; ++p2)
                        {
                            float d2 = (centers[shape[p]] - centers[unusedPoints[p2]]).sqrMagnitude;
                            if (d2 < d)
                            {
                               
                                d = d2;
                                min = p2;
                                Gizmos.color = Color.white;
                            }
                        }
                        shape.Add(unusedPoints[min]);
                        unusedPoints.RemoveAt(min);
                        ++p;
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(centers[shape[p-1]], centers[shape[p]]);
                    }
                    Gizmos.DrawLine(centers[shape[0]], centers[shape[p]]);
                }
            }
        }

    }

}
