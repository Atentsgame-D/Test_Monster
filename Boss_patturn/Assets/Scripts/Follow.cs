using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target; // ī�޶� ����Ÿ��
    public Vector3 offset; //������

    private void Update()
    {
        transform.position = target.position + offset; // ī�޶� player ���� �̵�
    }
}
