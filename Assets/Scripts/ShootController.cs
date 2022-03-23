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
    public enum status { idle, standBy, drawing, handling };
    public status playerStatus = status.idle;
    public bool isArcheryProcess { get { return playerStatus == status.drawing || playerStatus == status.handling; } }

    [Header("Shoot")]
    public ShootRecorder shootRecorder;
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
        //沒箭了
        if (shootRecorder.arrow_count <= 0) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //進入射擊預備狀態
            playerStatus = status.standBy;
            //開啟第三人稱攝影機
            _3rePersonView.SetActive(true);
            //設定Animator
            animator.SetBool("StandBy", true);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            //拉弓
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //設定狀態
                playerStatus = status.drawing;
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
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (playerStatus == status.handling)
                {

                    //設定Animator
                    animator.SetTrigger("Shoot");
                    //播放射出音效
                    audio_shoot.Play();
                    //放箭
                    Shoot();
                    //射出後視角重置
                    this.gameObject.GetComponent<ThirdPersonCamera>().ResetAngle();
                }

                //設定Animator
                animator.SetBool("Drawing", false);
                //設定狀態
                playerStatus = status.standBy;
                //停止專注度判斷
                if (isConcentretionSystemActive) concentrationSystem.FocusDetectionReset();
                //設定Mask
                maskCanvas.SetActive(false);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            //回到普通狀態
            playerStatus = status.idle;
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

        //專注數值影響
        if (isConcentretionSystemActive)
        {
            if (playerStatus == status.drawing || playerStatus == status.handling)
            {
                concermMask.GetComponent<Image>().DOFade(concentrationSystem.concentration, 0.15f);
            }
        }
    }

    //放箭
    void Shoot()
    {
        //生成箭並射出
        GameObject newArrow = Instantiate(arrowPrefab, initPoint.position, Quaternion.identity);
        newArrow.transform.LookAt(targetPoint.position);
        newArrow.GetComponent<Rigidbody>().AddForce(newArrow.transform.forward * shootSpeed);

        //紀錄箭數量減一
        shootRecorder.Shoot();
    }

    //拉弓動畫完成時呼叫
    public void ArrowReady()
    {
        playerStatus = status.handling;
    }
}
