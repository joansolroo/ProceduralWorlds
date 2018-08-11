using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMapCell : MonoBehaviour {

    [SerializeField] public int index;
    [SerializeField] public Vector3 PositionSpherical;
   // [SerializeField] public List<SphereMapCell> neighbours = new List<SphereMapCell>();
    [SerializeField] public List<int> neighbours = new List<int>();
    public SphereMap sphere;
    
    public bool walkable = true;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        
    }
}
