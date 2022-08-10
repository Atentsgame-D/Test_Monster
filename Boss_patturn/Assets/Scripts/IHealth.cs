using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    Transform transform { get; }
    float HP { get;  } // HP�� Ȯ���ϰ� ������ �� �ִ�.

    float MaxHP { get; }   // �ִ� HP�� Ȯ���� �� �ִ�.
    
    System.Action onHealthChange { get; set; } // HP�� ����� �� ����� ��������Ʈ
}
