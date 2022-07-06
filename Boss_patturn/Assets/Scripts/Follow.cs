using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈타겟
    public Vector3 offset; //고정값

    private void Update()
    {
        transform.position = target.position + offset; // 카메라값 player 따라 이동
    }
}
