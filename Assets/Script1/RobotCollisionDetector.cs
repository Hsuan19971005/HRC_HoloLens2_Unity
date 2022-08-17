using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**RobotCollisionDetector �����H�I��������
 * ²�z�G�ΥH���������H�O�_�P��L����I���A�I���ɧ��ܨ��C�⬰Red�A�D�I���ɫ�_���C��
 * �ϥΤ覡�G
 * �N�}����m�bRobot Visual������Collider������U�ACollider�Ұ�IsTrigger�A�ñN�Ӫ����m�bArmNoCollision��Layer���A��l�����HCollision����h�t���m���LLayer�C
 * �bProject Setting����I�����Y�i��]�w�AArmNoCollision layer�������I���AArmNoCollision�P��l�����HCollision����ҦbLayer�]���I���C
 * �ت������G
 * �����H����Visual�PCollision���Collider�A��̽d���|�A��������̤����I���~�බ�Q��������H�I�����\��
 */
public class RobotCollisionDetector : MonoBehaviour
{
    private Color origin_color;
    void Start()
    {
        this.origin_color = this.GetComponent<Renderer>().material.color;
    }
    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<Renderer>().material.color = Color.red;
    }
    private void OnTriggerExit(Collider other)
    {
        this.GetComponent<Renderer>().material.color = this.origin_color;
    }
}
