using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShootController : MonoBehaviour
{
    [Header("Camera")]
    public GameObject _3rePersonView;

    [Header("State")]
    public Animator animator;
    public enum statusSring { idle, standBy, drawing, handling };
    public statusSring playerStatus = statusSring.idle;
    public bool isArcheryProcess { get { return playerStatus == statusSring.drawing || playerStatus == statusSring.handling; } }

    [Header("Shoot")]
    public ShootManager shootManager;
    public Transform initPoint;
    public Transform targetPoint;
    public GameObject arrowPrefab;
    public float shootSpeed = 50f;
    public AudioSource audio_draw;
    public AudioSource audio_shoot;

    [Header("ConcentretionSystem")]
    public ConcentrationSystem concentrationSystem;
    private bool isConcentretionSystemActive = false;
    public GameObject maskCanvas;
    public GameObject concermMask;
    public GameObject normalMask;

    void Start()
    {
        //關閉隨滑鼠移動的攝影機
        _3rePersonView.SetActive(false);

        //判斷是否有專注系統
        isConcentretionSystemActive = concentrationSystem.gameObject.activeInHierarchy;
        //設定mask
        normalMask.SetActive(isConcentretionSystemActive == false);
        concermMask.SetActive(isConcentretionSystemActive == true);
        concermMask.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        maskCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shootManager.canShoot)
        {
            if (playerStatus == statusSring.idle && Input.GetKeyDown(KeyCode.Space))
            {
                //進入射擊預備狀態
                playerStatus = statusSring.standBy;
                //開啟第三人稱攝影機
                _3rePersonView.SetActive(true);
                //設定Animator
                animator.SetBool("StandBy", true);
                //關閉鼠標
                Cursor.visible = false;
            }
            else if (playerStatus != statusSring.idle)
            {
                //拉弓
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //設定狀態
                    playerStatus = statusSring.drawing;
                    //設定Animator
                    animator.SetBool("Drawing", true);
                    //進行專注度判斷
                    if (isConcentretionSystemActive) concentrationSystem.FocusDetectionStart();
                    //設定Mask
                    maskCanvas.SetActive(true);
                    //播放拉弓音效
                    audio_draw.Play();
                }
                //放弓
                if (!Input.GetKey(KeyCode.Mouse0) && playerStatus == statusSring.handling)
                {

                    //設定Animator
                    animator.SetTrigger("Shoot");
                    //播放射出音效
                    audio_shoot.Play();
                    //放箭
                    Shoot();
                    //射出後視角重置
                    this.gameObject.GetComponent<ThirdPersonCamera>().ResetAngle();

                    ResetStatus();

                    ////設定Animator
                    //animator.SetBool("Drawing", false);
                    ////設定狀態
                    //playerStatus = statusSring.standBy;
                    ////停止專注度判斷
                    //if (isConcentretionSystemActive) concentrationSystem.FocusDetectionReset();
                    ////設定Mask
                    //maskCanvas.SetActive(false);
                }
            }
            //else if (Input.GetKeyUp(KeyCode.Space))
            //{
            //    ResetStatus()
            //}
        }
        else
        {
            if (playerStatus != statusSring.idle)
            {
                ResetStatus();
            }
        }


        //專注數值影響
        if (isConcentretionSystemActive)
        {
            if (playerStatus == statusSring.drawing || playerStatus == statusSring.handling)
            {
                float maskScale = Mathf.Lerp(6f, 1f, concentrationSystem.concentration);
                concermMask.transform.DOScale(Vector3.one * maskScale, 0.15f);
                concermMask.GetComponent<Image>().DOFade(concentrationSystem.concentration, 0.15f);
            }
        }
    }

    void ResetStatus()
    {
        //回到普通狀態
        playerStatus = statusSring.idle;
        //設定Animator
        animator.SetBool("Drawing", false);
        //關閉第三人稱攝影機
        _3rePersonView.SetActive(false);
        //設定Animator
        animator.SetBool("StandBy", false);
        //停止專注度判斷
        if (isConcentretionSystemActive) concentrationSystem.FocusDetectionReset();
        //設定Mask
        maskCanvas.SetActive(false);
    }

    //放箭
    void Shoot()
    {
        //生成箭並射出
        GameObject newArrow = Instantiate(arrowPrefab, initPoint.position, Quaternion.identity);
        newArrow.transform.LookAt(targetPoint.position);
        newArrow.GetComponent<Rigidbody>().AddForce(newArrow.transform.forward * shootSpeed);

        //紀錄箭數量減一
        shootManager.Shoot();
    }

    //拉弓動畫完成時呼叫
    public void ArrowReady()
    {
        playerStatus = statusSring.handling;
    }
}
