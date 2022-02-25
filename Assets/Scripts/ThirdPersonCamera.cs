using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform cameraHolder;
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityHor = 9f;
    public float sensitivityVert = 9f;

    public float minmumVert = -45f;
    public float maxmumVert = 45f;
    private float _rotationX = 0;

    public float minmumHor = -60f;
    public float maxmumHor = 60f;
    private float _rotationY = 0;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraHolder.gameObject.activeInHierarchy == false) return;

        if (axes == RotationAxes.MouseX)
        {
            _rotationY = _rotationY + Input.GetAxis("Mouse X") * sensitivityHor;
            _rotationY = Mathf.Clamp(_rotationY, minmumHor, maxmumHor);

            float rotationX = cameraHolder.localEulerAngles.x;

            cameraHolder.localEulerAngles = new Vector3(rotationX, _rotationY, 0);
        }
        else if (axes == RotationAxes.MouseY)
        {
            _rotationX = _rotationX - Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minmumVert, maxmumVert);

            float rotationY = cameraHolder.localEulerAngles.y;

            cameraHolder.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
        else
        {
            _rotationY = _rotationY + Input.GetAxis("Mouse X") * sensitivityHor;
            _rotationY = Mathf.Clamp(_rotationY, minmumHor, maxmumHor);

            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minmumVert, maxmumVert);

            cameraHolder.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
        }

    }
}
