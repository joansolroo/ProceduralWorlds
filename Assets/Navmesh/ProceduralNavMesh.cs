using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralNavMesh : MonoBehaviour {

    [SerializeField] bool generate = false;
    [SerializeField] float r = 0.5f;
    private void OnValidate()
    {
        if (generate)
        {
            float a = Random.value;
            float b = Random.value;

            Vector3 pos = new Vector3(
                r * Mathf.Sin(a) * Mathf.Cos(b),
                r * Mathf.Sin(a) * Mathf.Sin(b),
                r* Mathf.Cos(a));
        }
    }
}
