using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ArrowController : MonoBehaviour
{
    public AudioSource audio_hit;

    private Rigidbody rigidbody;
    private bool isHit = false;
    private float timer = 0f;

    void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //射出後三秒如果沒有命中，刪除此箭
        timer += Time.deltaTime;
        if(timer > 3f && !isHit)
        {
            Destroy(this.gameObject);
        }

        if (!isHit)
        {
            //設定箭方向與飛行方向平行
            Vector3 speed = rigidbody.velocity.normalized;
            Vector3 rotation = new Vector3(-Mathf.Atan(speed.y / speed.z) / Mathf.PI * 180f, Mathf.Atan(speed.x / speed.z) / Mathf.PI * 180f, 0f);
            this.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ring")
        {
            //避免同時命中兩環
            if (isHit) return;

            isHit = true;

            //播放擊中音效
            audio_hit.Play();

            //讓箭停止在靶上
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;

            //關閉碰撞器
            this.GetComponent<Collider>().enabled = false;

            //算分數
            int score = collision.gameObject.GetComponent<RingData>().score;
            GameObject.Find("GameManager").GetComponent<ShootRecorder>().Record(score);
            Debug.Log(score);

            //1秒後刪除跟隨的鏡頭
            Destroy(this.GetComponentInChildren<CinemachineVirtualCamera>().gameObject, 1f);
        }
    }

    public void DestroyArrow()
    {
        Destroy(this.gameObject);
    }
}
