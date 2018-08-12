using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [SerializeField] Transform camera;
   
    [SerializeField] Vector3 speed = Vector3.one;
    [SerializeField] Vector2 zoomRange;
    [SerializeField] float zoom;
    // Use this for initialization

    private void Start()
    {
        zoom = -camera.transform.localPosition.z;
    }

    bool dragging = false;
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!dragging)
            {
                OnMouseDown();
            }
            else
            {
                OnMouseDrag();
            }
        }
        else if (dragging)
        {
            OnMouseUp();
        }
        zoom += Input.GetAxis("Mouse ScrollWheel")*speed[2];
        zoom = Mathf.Min(zoomRange[1], Mathf.Max(zoomRange[0], zoom));
        Vector3 pos = camera.transform.localPosition;
        pos.z = -zoom;
        camera.transform.localPosition = pos;
    }
    [SerializeField] Vector3 rotation;
    [SerializeField] Vector3 rotationDelta = Vector3.zero;
    Vector3 mousePos;
    private void OnMouseDown()
    {
        dragging = true;
        rotationDelta = Vector3.zero;
        mousePos = Input.mousePosition;
        Debug.Log("click");
    }
    void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - mousePos;
        rotationDelta.x = delta.y * speed.x;
        rotationDelta.y = delta.x * speed.y;
        transform.localEulerAngles = rotation + rotationDelta;
    }
    private void OnMouseUp()
    {

        rotation += rotationDelta;
        transform.localEulerAngles = rotation;
        dragging = false;
    }
}
