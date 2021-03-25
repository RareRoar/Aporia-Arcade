using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float timer_;
    void Start()
    {
        timer_ = 0.0f;
    }
    void Update()
    {
        timer_ += Time.deltaTime;
        Text text = GetComponent<Text>();
        text.text = Math.Round(timer_, 2).ToString();
    }
}
