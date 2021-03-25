using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset_;
    void Start()
    {
        offset_ = transform.position - player.transform.position;
    }

    void Update()
    {
        transform.position = player.transform.position + offset_;
    }
}
