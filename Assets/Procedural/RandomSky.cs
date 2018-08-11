using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSky : MonoBehaviour {

    [SerializeField] Gradient gradient;
    [SerializeField] AnimationCurve scale;
    [SerializeField] int starCount=10;
    [SerializeField] GameObject starPrefab;

	// Use this for initialization
	void Start () {
        for (int s = 0; s < starCount; ++s)
        {
            Color color = gradient.Evaluate(Random.value);
            GameObject go =Instantiate<GameObject>(starPrefab);
            go.GetComponent<MeshRenderer>().material.SetColor("_Emission",color);
            go.GetComponent<MeshRenderer>().material.color = color;
            for(int c = 0; c < go.transform.childCount; ++c)
            {
                go.transform.GetChild(c).gameObject.GetComponent<MeshRenderer>().material.SetColor("_Emission", color);
                go.transform.GetChild(c).gameObject.GetComponent<MeshRenderer>().material.color = color;
            }
            go.transform.parent = this.transform;
            go.transform.localScale = Vector3.one * scale.Evaluate(Random.value);
            go.transform.rotation = Quaternion.LookRotation(go.transform.position - this.transform.position);
            go.SetActive(true);
            float a = Random.Range(0f, 360 * Mathf.Deg2Rad);
            float b = Random.Range(0f, 360 * Mathf.Deg2Rad);
            float r = 100;
            float x = r*Mathf.Sin(a) * Mathf.Cos(b);
            float y = r*Mathf.Sin(a) * Mathf.Sin(b);
            float z = r*Mathf.Cos(a);
            go.transform.localPosition = new Vector3(x,y,z);

        }
	}

}
