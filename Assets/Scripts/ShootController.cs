using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    [Header("Camera")]
    public GameObject _3rePersonView;

    [Header("State")]
    public Animator animator;
    private enum status { idle, standBy, drawing, handling };
    private status playerStatus = status.idle;

    [Header("Shoot")]
    public Transform initPoint;
    public Transform targetPoint;
    public GameObject arrowPrefab;
    public float shootSpeed = 50f;

    void Start()
    {
        _3rePersonView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //進入射擊預備狀態
            playerStatus = status.standBy;
            //開啟第三人稱攝影機
            _3rePersonView.SetActive(true);
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
            }
            //放弓
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (playerStatus == status.handling)
                {
                    //設定Animator
                    animator.SetTrigger("Shoot");
                    //放箭
                    Shoot();
                }

                //設定Animator
                animator.SetBool("Drawing", false);

                //設定狀態
                playerStatus = status.standBy;
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
        }
    }

    //放箭
    void Shoot()
    {
        GameObject newArrow = Instantiate(arrowPrefab, initPoint.position, Quaternion.identity);
        newArrow.transform.LookAt(targetPoint.position);
        newArrow.GetComponent<Rigidbody>().AddForce(newArrow.transform.forward * shootSpeed);
    }

    public void ArrowReady()
    {
        playerStatus = status.handling;
    }
}
