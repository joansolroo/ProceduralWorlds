using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProceduralIcosphere : MonoBehaviour
{
    [SerializeField] bool update = false;

    [SerializeField] [Range(0, 4)] int recursionLevel = 3;
    [SerializeField] float radius = 1f;
    [SerializeField] [Range(0, 6)] int bumpiness = 6;
    [SerializeField] [Range(0.01f, 10f)] float noiseScale = 1f;
    [SerializeField] [Range(0.01f, 10f)] float steepness = 0.25f;
    [SerializeField] Vector3 offsetPerlin = Vector3.zero;
    [SerializeField] bool Coesphere = false;
    [SerializeField] bool CreateTiles = false;

    [SerializeField] Gradient colors;

    private void Start()
    {
        
    }
    private void OnValidate()
    {
        if (update)
        {
            Generate();
        }
    }

    private struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    // return index of point in the middle of p1 and p2
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);

        // store it, return index
        cache.Add(key, i);

        return i;
    }
    void Generate()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filter.sharedMesh = mesh;
        mesh.Clear();

        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

        vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

        vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();

        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));


        // refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        mesh.vertices = vertList.ToArray();

        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++)
        {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }
        mesh.triangles = triList.ToArray();
        mesh.uv = new Vector2[mesh.vertices.Length];

        Vector3[] normales = new Vector3[vertList.Count];
        for (int i = 0; i < normales.Length; i++)
            normales[i] = vertList[i].normalized;

        mesh.normals = normales;
        if (Coesphere)
        {

            Flip(mesh);

        }

        mesh.RecalculateBounds();

    }

    void Flip(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        List<Vector3> centers = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<int>[] polygons = new List<int>[vertices.Length];

        int inv = 0;

        for (int t = 0; t < triangles.Length; t += 3)
        {
            Vector3 center = (vertices[triangles[t]] + vertices[triangles[t + 1]] + vertices[triangles[t + 2]]) / 3;
            centers.Add(center);
            Vector3 normal = (normals[triangles[t]] + normals[triangles[t + 1]] + normals[triangles[t + 2]]).normalized;
            newNormals.Add(normal);

            for (int d = 0; d < 3; ++d)
            {
                if (polygons[triangles[t + d]] == null)
                {
                    polygons[triangles[t + d]] = new List<int>();
                }
                polygons[triangles[t + d]].Add(inv);
            }
            ++inv;
        }

        for (int pol = 0; pol < polygons.Length; ++pol)
        {
            // SORT THE POLYGON CONTOUR
            List<int> shape = new List<int>();
            List<int> unusedPoints = new List<int>(polygons[pol]);
            shape.Add(unusedPoints[0]);
            unusedPoints.RemoveAt(0);
            {
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
                        }
                    }
                    shape.Add(unusedPoints[min]);
                    unusedPoints.RemoveAt(min);
                    ++p;
                }
                polygons[pol] = shape;
            }
            //Make it clockwise
            {
                Vector3 center = Vector3.zero;
                foreach (int p in polygons[pol])
                {
                    center += centers[p];
                }
                center /= polygons[pol].Count;

                Vector3 a = center;
                Vector3 b = centers[polygons[pol][0]];
                Vector3 c = centers[polygons[pol][1]];
                Vector3 side1 = b - a;
                Vector3 side2 = c - a;
                Vector3 perp = Vector3.Cross(side1, side2);
                if ((a + perp).sqrMagnitude <= (a).sqrMagnitude)
                {
                    polygons[pol].Reverse();
                }
            }


        }
        // Compute neighbours        
        List<int>[] sharedVertices = new List<int>[centers.Count];
        for (int pol = 0; pol < polygons.Length; ++pol)
        {
            foreach (int v in polygons[pol])
            {
                if (v >= sharedVertices.Length) Debug.LogWarning("problem in size: " + v + " >= " + sharedVertices.Length);
                if (sharedVertices[v] == null)
                {
                    sharedVertices[v] = new List<int>();
                }
                sharedVertices[v].Add(pol);
            }
        }
        List<int>[] neighbours = new List<int>[polygons.Length];
        List<Vector3>[] neighboursJoints = new List<Vector3>[polygons.Length];
        for ( int v = 0; v < sharedVertices.Length; ++v)
        {
            for (int pol = 0; pol < sharedVertices[v].Count; ++pol)
            {
                if (neighbours[sharedVertices[v][pol]] == null)
                {
                    neighbours[sharedVertices[v][pol]] = new List<int>();
                    neighboursJoints[sharedVertices[v][pol]] = new List<Vector3>();
                }
                for (int pol2 = 0; pol2 < sharedVertices[v].Count; ++pol2)
                {
                    if(pol == pol2)
                    {
                        continue;
                    }
                    neighbours[sharedVertices[v][pol]].Add(sharedVertices[v][pol2]);
                    neighboursJoints[sharedVertices[v][pol]].Add(centers[v]);
                }
            }
        }

        #region basemesh
        /*
        //THIS IS THE SPHERE WITHOUT DISPLACEMENT
        {
            List<int> newTriangles = new List<int>();
            foreach (List<int> pol in polygons)
            {
                Vector3 center = Vector3.zero;
                Vector3 normal = Vector3.zero;
                foreach (int p in pol)
                {
                    center += centers[p];
                    normal += newNormals[p];
                }
                center /= pol.Count;
                centers.Add(center);

                normal.Normalize();
                newNormals.Add(normal);

                int centerIdx = centers.Count - 1;
                {

                    for (int idxp = 0; idxp < pol.Count - 1; ++idxp)
                    {
                        newTriangles.Add(centerIdx);
                        newTriangles.Add(pol[idxp]);
                        newTriangles.Add(pol[idxp + 1]);
                    }
                    newTriangles.Add(centerIdx);
                    newTriangles.Add(pol[pol.Count - 1]);
                    newTriangles.Add(pol[0]);


                }
            }
            mesh.vertices = centers.ToArray();
            mesh.normals = newNormals.ToArray();
            mesh.triangles = newTriangles.ToArray();
            mesh.RecalculateNormals();
        }
        */
        #endregion

        #region Tiles
        float[] offset = new float[polygons.Length];
        SphereMapCell[] cells = new SphereMapCell[polygons.Length];
        SphereMap map = GetComponent<SphereMap>();
        map.cells = cells;
        {
            Clear();
            for (int pol = 0; pol < polygons.Length; ++pol)
            {
                List<int> polygon = polygons[pol];
                Mesh mesh2 = new Mesh();
                Vector3[] vertices2 = new Vector3[polygon.Count + 1];
                Vector3[] normals2 = new Vector3[polygon.Count + 1];

                Vector3 center = Vector3.zero;
                Vector3 normal = Vector3.zero;

                int idx = 0;
                foreach (int p in polygon)
                {
                    center += centers[p];
                    normal += newNormals[p];

                    vertices2[idx] = centers[p];
                    normals2[idx] = newNormals[p];

                    ++idx;
                }
                center /= polygon.Count;
                int centerIdx = vertices2.Length - 1;
                vertices2[centerIdx] = center;

                normal.Normalize();
                normals2[centerIdx] = normal;



                List<int> triangles2 = new List<int>();

                {

                    for (int idxp = 0; idxp < polygon.Count - 1; ++idxp)
                    {
                        triangles2.Add(centerIdx);
                        triangles2.Add(idxp);
                        triangles2.Add(idxp + 1);
                    }
                    triangles2.Add(centerIdx);
                    triangles2.Add(polygon.Count - 1);
                    triangles2.Add(0);

                }



                for (int v = 0; v < vertices2.Length; ++v)
                {
                    vertices2[v] -= center;
                }
                offset[pol] = Mathf.Max(0, (Perlin.Fbm(center * noiseScale + offsetPerlin, bumpiness))) * steepness;
                /*for (int v = 0; v < vertices2.Length; ++v)
                {

                    vertices2[v] += normal* 0.0001f;

                }*/
                center  += normal * (offset[pol] + 0.01f);
                Color color = colors.Evaluate(offset[pol] / steepness);

                mesh2.vertices = vertices2;
                mesh2.normals = normals2;
                mesh2.triangles = triangles2.ToArray();

                StartCoroutine(CreateTile(pol, mesh2, center, normal, color,offset[pol],map));
                //CreateTile(pol, mesh2, center, normal, color);

            }
            StartCoroutine(CreateJumpLinks(map, neighbours,neighboursJoints));
        }
        #endregion
        #region TilesWithBase
        if (CreateTiles)
        {
            Material material = this.GetComponent<MeshRenderer>().material;// new Material(Shader.Find("Diffuse"));
            for (int c = transform.childCount - 1; c >= 0; --c)
            {
                Destroy(transform.GetChild(c).gameObject);
            }
            for (int pol = 0; pol < polygons.Length; ++pol)
            {
                List<int> polygon = polygons[pol];

                GameObject go2 = new GameObject();
                MeshFilter mf2 = go2.AddComponent<MeshFilter>();
                MeshRenderer mr2 = go2.AddComponent<MeshRenderer>();
                mr2.material = material;

                Mesh mesh2 = new Mesh();
                Vector3[] vertices2 = new Vector3[2 * polygon.Count + 1];
                Vector3[] normals2 = new Vector3[2 * polygon.Count + 1];

                Vector3 center = Vector3.zero;
                Vector3 normal = Vector3.zero;

                go2.transform.parent = this.transform;
                go2.transform.localPosition = Vector3.zero;
                go2.transform.localScale = Vector3.one;


                int idx = 0;
                foreach (int p in polygon)
                {
                    center += centers[p];
                    normal += newNormals[p];

                    vertices2[idx] = centers[p];
                    normals2[idx] = newNormals[p];

                    vertices2[idx + polygon.Count] = centers[p];
                    normals2[idx + polygon.Count] = newNormals[p];

                    ++idx;
                }
                center /= polygon.Count;
                int centerIdx = vertices2.Length - 1;
                vertices2[centerIdx] = center;

                normal.Normalize();
                normals2[centerIdx] = normal;



                List<int> triangles2 = new List<int>();

                {
                    for (int idxp = 0; idxp < polygon.Count - 1; ++idxp)
                    {
                        triangles2.Add(centerIdx);
                        triangles2.Add(idxp);
                        triangles2.Add(idxp + 1);
                    }
                    triangles2.Add(centerIdx);
                    triangles2.Add(polygon.Count - 1);
                    triangles2.Add(0);

                    {

                        for (int idxp = 0; idxp < polygon.Count - 1; idxp++)
                        {
                            triangles2.Add(idxp);
                            triangles2.Add(idxp + polygon.Count);
                            triangles2.Add(idxp + 1);



                            triangles2.Add(idxp + polygon.Count);
                            triangles2.Add(idxp + polygon.Count + 1);
                            triangles2.Add(idxp + 1);

                        }
                        triangles2.Add(polygon.Count - 1);
                        triangles2.Add(polygon.Count + polygon.Count - 1);
                        triangles2.Add(0);

                        triangles2.Add(polygon.Count + polygon.Count - 1);
                        triangles2.Add(polygon.Count);
                        triangles2.Add(0);
                    }

                }
                go2.transform.localPosition = center;
                // go2.transform.LookAt(normal);


                for (int v = 0; v < vertices2.Length; ++v)
                {
                    vertices2[v] -= center;
                }
                float height = offset[pol];
                for (int v = 0; v < polygon.Count; ++v)
                {

                    vertices2[v] += normal * height;

                }
                vertices2[centerIdx] += normal * height;
                mr2.material.color = Color.Lerp(Random.ColorHSV(0.3f, 0.35f, 0.5f, 1, 0.8f, 0.9f), Random.ColorHSV(0.1f, 0.15f, 0.5f, 1, 0.8f, 0.9f), height * 5);
                mesh2.vertices = vertices2;
                mesh2.normals = normals2;
                mesh2.triangles = triangles2.ToArray();
                mf2.sharedMesh = mesh2;
                mf2.mesh.RecalculateNormals();
                mf2.mesh.RecalculateBounds();
            }
        }
        #endregion
        #region DisplacedMesh
        else
        {
            List<Vector3> vertices3 = new List<Vector3>();
            List<Vector3> normals3 = new List<Vector3>();
            List<int> triangles3 = new List<int>();
            for (int pol = 0; pol < polygons.Length; ++pol)
            {
                List<int> polygon = polygons[pol];
                Vector3[] vertices2 = new Vector3[2 * polygon.Count + 1];
                Vector3[] normals2 = new Vector3[2 * polygon.Count + 1];

                Vector3 center = Vector3.zero;
                Vector3 normal = Vector3.zero;


                int idx = 0;
                foreach (int p in polygon)
                {
                    center += centers[p];
                    normal += newNormals[p];

                    vertices2[idx] = centers[p];
                    normals2[idx] = newNormals[p];

                    vertices2[idx + polygon.Count] = centers[p];
                    normals2[idx + polygon.Count] = newNormals[p];

                    ++idx;
                }
                center /= polygon.Count;
                int centerIdx = vertices2.Length - 1;
                vertices2[centerIdx] = center;

                normal.Normalize();
                normals2[centerIdx] = normal;



                List<int> triangles2 = new List<int>();

                {
                    for (int idxp = 0; idxp < polygon.Count - 1; ++idxp)
                    {
                        triangles2.Add(centerIdx);
                        triangles2.Add(idxp);
                        triangles2.Add(idxp + 1);
                    }
                    triangles2.Add(centerIdx);
                    triangles2.Add(polygon.Count - 1);
                    triangles2.Add(0);

                    {

                        for (int idxp = 0; idxp < polygon.Count - 1; idxp++)
                        {
                            triangles2.Add(idxp);
                            triangles2.Add(idxp + polygon.Count);
                            triangles2.Add(idxp + 1);



                            triangles2.Add(idxp + polygon.Count);
                            triangles2.Add(idxp + polygon.Count + 1);
                            triangles2.Add(idxp + 1);

                        }
                        triangles2.Add(polygon.Count - 1);
                        triangles2.Add(polygon.Count + polygon.Count - 1);
                        triangles2.Add(0);

                        triangles2.Add(polygon.Count + polygon.Count - 1);
                        triangles2.Add(polygon.Count);
                        triangles2.Add(0);
                    }

                }
                float height = offset[pol];
                for (int v = 0; v < polygon.Count; ++v)
                {

                    vertices2[v] += normal * height;

                }
                vertices2[centerIdx] += normal * height;

                int prevCount = vertices3.Count;
                foreach (Vector3 v in vertices2) vertices3.Add(v);
                foreach (Vector3 n in normals2) normals3.Add(n);
                foreach (int t in triangles2) triangles3.Add(t + prevCount);
            }

            mesh.vertices = vertices3.ToArray();
            mesh.normals = normals3.ToArray();
            mesh.triangles = triangles3.ToArray();
            mesh.RecalculateNormals();

        }
        #endregion
    }


    void Clear()
    {
        for (int c = transform.childCount - 1; c >= 0; --c)
        {
            StartCoroutine(Destroy(transform.GetChild(c).gameObject));
        }
    }
    IEnumerator Destroy(GameObject go)
    {
        yield return new WaitForSecondsRealtime(0.01f);
        DestroyImmediate(go);
    }

    IEnumerator CreateTile(Mesh mesh, Vector3 center, Color color)
    {
        yield return new WaitForSecondsRealtime(0.01f);
        GameObject go2 = new GameObject();
        MeshFilter mf2 = go2.AddComponent<MeshFilter>();
        MeshRenderer mr2 = go2.AddComponent<MeshRenderer>();

        mr2.sharedMaterial = new Material(this.GetComponent<MeshRenderer>().sharedMaterial);// new Material(Shader.Find("Diffuse"));
        mr2.sharedMaterial.color = color;
        // mr2.material = material;

        go2.transform.parent = this.transform;
        //go2.transform.localPosition = Vector3.zero;
        go2.transform.localRotation = Quaternion.identity;
        go2.transform.localScale = Vector3.one;
        go2.transform.localPosition = center;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mf2.sharedMesh = mesh;

        SphereMapCell cell = go2.AddComponent<SphereMapCell>();
        cell.PositionSpherical = FromCartesian(center);

    }

    IEnumerator CreateTile(int polygonId, Mesh mesh, Vector3 center, Vector3 orientation, Color color, float height, SphereMap map)
    {
        yield return new WaitForSecondsRealtime(0.0f);
        GameObject meshGo = new GameObject();
        MeshFilter mf2 = meshGo.AddComponent<MeshFilter>();
        MeshRenderer mr2 = meshGo.AddComponent<MeshRenderer>();

        mr2.sharedMaterial = new Material(this.GetComponent<MeshRenderer>().sharedMaterial);// new Material(Shader.Find("Diffuse"));
        mr2.sharedMaterial.color = color;
        // mr2.material = material;

        GameObject cellGo = new GameObject();
        cellGo.transform.parent = this.transform;
        cellGo.transform.localScale = Vector3.one;
        //cellGo.transform.localRotation = Quaternion.identity;
        cellGo.transform.LookAt(center + orientation);
        cellGo.transform.Rotate(90, 0, 0);
        meshGo.transform.parent = cellGo.transform;
        //go2.transform.localPosition = Vector3.zero;
        //meshGo.transform.localRotation = Quaternion.identity;
        meshGo.transform.localScale = Vector3.one;
        meshGo.transform.localPosition = Vector3.zero;

        meshGo.hideFlags = HideFlags.HideInHierarchy;
        /*
        MeshCollider collider = meshGo.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        collider.convex = true;
        collider.inflateMesh = true;
        */
        cellGo.transform.localPosition = center;


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mf2.sharedMesh = mesh;

        bool walkable = height > 0.1f;

        SphereMapCell cell = cellGo.AddComponent<SphereMapCell>();
        cell.PositionSpherical = FromCartesian(center);
        cellGo.name = "[" + (int)(cell.PositionSpherical.x * Mathf.Rad2Deg + 360) % 360 + "," + (int)(cell.PositionSpherical.y * Mathf.Rad2Deg + 360) % 360 + "]";
        cell.index = polygonId;
        cell.sphere = map;
        map.cells[polygonId] = cell;

        cell.walkable = walkable;

        NavMeshSurface navSurface = cellGo.AddComponent<NavMeshSurface>();
        navSurface.collectObjects = CollectObjects.Children;
        navSurface.defaultArea = walkable ? 0 : 1;
        navSurface.AddData();
        navSurface.BuildNavMesh();
        
    }
    IEnumerator CreateJumpLinks(SphereMap map, List<int>[] neighbours, List<Vector3>[] neighboursJoints)
    {
        yield return new WaitForSecondsRealtime(0.25f);
        for (int pol1 = 0; pol1 < neighbours.Length; ++pol1)
        {
            if (map.cells[pol1].walkable)
            {
                for (int pol2 = 0; pol2 < neighbours[pol1].Count; ++pol2)
                {
                    if (map.cells[neighbours[pol1][pol2]].walkable)
                    {
                        Vector3 p1 = Vector3.zero; // cells[pol1].transform.position;
                        //Vector3 c = this.transform.TransformPoint(neighboursJoints[pol1][pol2]);
                        Vector3 p2 = map.cells[neighbours[pol1][pol2]].transform.position;
                        p2 = map.cells[pol1].transform.InverseTransformPoint(p2);

                        Vector3 p12 = (p2 - p1) * 0.3f + p1; p12.y = 0;
                        NavMeshLink link = map.cells[pol1].gameObject.AddComponent<UnityEngine.AI.NavMeshLink>();
                        link.startPoint = p12;
                        link.endPoint = p2;
                        link.autoUpdate = false;
                    }
                }
            }
        }
    }

    public Vector3 FromCartesian(Vector3 cartesianCoordinate)
    {
        if (cartesianCoordinate.x == 0f)
            cartesianCoordinate.x = Mathf.Epsilon;
        float radius = cartesianCoordinate.magnitude;

        float polar = Mathf.Atan(cartesianCoordinate.z / cartesianCoordinate.x);

        if (cartesianCoordinate.x < 0f)
            polar += Mathf.PI;
        float elevation = Mathf.Asin(cartesianCoordinate.y / radius);
        return new Vector3(polar, elevation, radius);
    }
    /*
    private void OnDrawGizmos()
    {
        if (neighbours != null && neighbours.Length == cells.Length) {
            for (int pol1 = 0; pol1 < neighbours.Length; ++pol1)
            {
                for (int pol2 = 0; pol2 < neighbours[pol1].Count; ++pol2)
                {
                    Vector3 p1 = cells[pol1].transform.position;
                    Vector3 c = this.transform.TransformPoint(neighboursJoints[pol1][pol2]);
                    Vector3 p2 = cells[neighbours[pol1][pol2]].transform.position;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(p1, c);
                    Gizmos.DrawSphere(c, 1f);
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(c+Vector3.one*0.1f, p2 + Vector3.one * 0.1f);
                }
            }
        }
        
    }*/
}
