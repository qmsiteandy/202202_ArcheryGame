using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRecorder : MonoBehaviour
{
    public int _score = 0;   //分數
    public int _arrow_number = 10;   //箭的數量

    void Start()
    {
        Reset();
    }

    public void Record(int score)
    {
        _score += score;
    }

    public void Shoot()
    {
        _arrow_number -= 1;
    }

    public void Reset()
    {
        _score = 0;
        _arrow_number = 10;
    }
}
