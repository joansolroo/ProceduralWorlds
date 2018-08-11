using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class testnavmesh : MonoBehaviour {
    [SerializeField] NavMeshBuildSource source;
    public void Start()
    {
       source = BoxSource10x10();
    }
    // Make a build source for a box in local space
    public NavMeshBuildSource BoxSource10x10()
        {
            var src = new NavMeshBuildSource();
   
            src.transform = transform.localToWorldMatrix;
            src.shape = NavMeshBuildSourceShape.Sphere;
            src.size = new Vector3(10.0f, 10f, 10.0f);
            return src;
        }
    
}
