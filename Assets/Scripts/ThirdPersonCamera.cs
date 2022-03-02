using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("視角旋轉")]
    public Transform cameraHolder;
    //垂直旋轉設定
    public float sensitivityVert = 9f;
    public float minmumVert = -45f;
    public float maxmumVert = 45f;
    private float _rotationX = 0;
    //水平旋轉設定
    public float sensitivityHor = 9f;
    public float minmumHor = -60f;
    public float maxmumHor = 60f;
    private float _rotationY = 0;

    [Header("Breath")]
    //呼吸晃動幅度
    [Range(0f, 1f)] public float noise = 0f;
    //呼吸晃動範圍
    public Vector2 range = new Vector2(7.5f, 4.5f);
    //呼吸影響的旋轉偏移量
    private float noiseRotateX = 0f;
    private float noiseRotateY = 0f;

    [Header("ConcentretionSystem")]
    public ConcentrationSystem concentrationSystem;
    private bool isConcentretionSystemActive = false;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;

        //判斷是否有專注系統
        isConcentretionSystemActive = concentrationSystem.gameObject.activeInHierarchy;
    }

    void Update()
    {
        //專注數值影響
        if (isConcentretionSystemActive)
        {
            noise = Mathf.Lerp(1f, 0f, concentrationSystem.concentration);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (cameraHolder.gameObject.activeInHierarchy == false) return;

        //視角隨滑鼠旋轉
        _rotationY = _rotationY + Input.GetAxis("Mouse X") * sensitivityHor;
        _rotationY = Mathf.Clamp(_rotationY, minmumHor, maxmumHor);
        _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
        _rotationX = Mathf.Clamp(_rotationX, minmumVert, maxmumVert);
        
        //設定呼吸偏移角度
        if (isConcentretionSystemActive)
        {
            noiseRotateY += (noise * range.y * Mathf.Cos(Time.time) - noiseRotateY) / 100f;
            noiseRotateX += (noise * range.x * Mathf.Sin(Time.time) - noiseRotateX) / 100f;
        }

        //設定相機旋轉
        cameraHolder.localEulerAngles = new Vector3(_rotationX + noiseRotateX, _rotationY + noiseRotateY, 0);

    }
}
