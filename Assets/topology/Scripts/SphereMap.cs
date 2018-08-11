using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMap : MonoBehaviour {

    [SerializeField] public SphereMapCell[] cells;

	// Use this for initialization
	void Start () {
        cells = new SphereMapCell[this.transform.childCount];
        for(int c = 0; c< this.transform.childCount; ++c)
        {
            SphereMapCell cell = this.transform.GetChild(c).gameObject.GetComponent<SphereMapCell>();
            cells[cell.index] = cell;
        }
	}
}
