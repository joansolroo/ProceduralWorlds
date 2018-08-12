using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    [SerializeField] float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Quaternion targetRotation = Quaternion.LookRotation(this.transform.position - Camera.main.transform.position, Camera.main.transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime*speed* Quaternion.Angle(targetRotation,transform.rotation));
	}
}
