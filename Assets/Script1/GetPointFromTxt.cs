using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class GetPointFromTxt : MonoBehaviour
{
    // �n�ϥμҫ��U���l����(ex: ReferenceParent)�Ӻ޲zpoint
    private Transform referenceParent;
    public GameObject referenceParentObject;

    // �Ѧ��I�nclone������
    public GameObject cloneObject;
    private List<GameObject> newCloneObject=new List<GameObject>();

    struct Direction_right
    {
        public float x;
        public float y;
        public float z;
    }
    Direction_right direction_right;

    struct Coordinate_right
    {
        public float x;
        public float y;
        public float z;
    }
    Coordinate_right coordinate_right;

    private void Start()
    {
        this.referenceParent = this.referenceParentObject.transform;
        string readFromFilePath = Application.streamingAssetsPath + "/Point/" + "point" + ".txt";
        List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();
        foreach (string line in fileLines)
        {
            var splitLine = new char[] { ',' };
            var inline = line.Split(splitLine);

            coordinate_right.x = float.Parse(inline[0]);
            coordinate_right.y = float.Parse(inline[1]);
            coordinate_right.z = float.Parse(inline[2]);

            direction_right.x = float.Parse(inline[3]);
            direction_right.y = float.Parse(inline[4]);
            direction_right.z = float.Parse(inline[5]);

            // �첾�첾�Ѽ�
            float shift_x = 927.2f;
            float shift_y = -300f;
            float shift_z = -686.7f;

            // �]�w�������������q
            direction_right = SetRotationAmount(direction_right);

            // convert vector from right to left hand coordianates
            Vector3 position_right = new Vector3(coordinate_right.x, coordinate_right.y, coordinate_right.z);
            Vector3 position_left =  ConvertRightHandedToLeftHandedVector(position_right);

            // �� point �K�� FBX �ҫ�(�ؼҿ�X���D�y���ҫ����b���I)
            Vector3 position_left_shift = ShiftBecauseFbxModel(position_left, shift_x, shift_y, shift_z);

            // convert quaternion from right to left hand coordianates
            Quaternion rotationR = Quaternion.Euler(direction_right.x, direction_right.y, direction_right.z);
            Quaternion rotationL = ConvertRightHandedToLeftHandedQuaternion(rotationR);

            // �ͦ��Ѧ��I
            GameObject obj=Instantiate(cloneObject, position_left_shift, rotationL, referenceParent);
            //this.newCloneObject.Add(obj);
        }

        MakePointAttachReferenceParent();
        //cloneObject.SetActive(false);

        //GameEvents
        GameEvents.current.actionDisableAllReferencePoints += DisableAllReferencePoints;
    }
    private Vector3 ConvertRightHandedToLeftHandedVector(Vector3 rightHandedVector)
    {
        return new Vector3(rightHandedVector.x, rightHandedVector.z, rightHandedVector.y);
    }
    private Vector3 ShiftBecauseFbxModel(Vector3 originVector, float shift_x, float shift_y, float shift_z)
    {
        return new Vector3(originVector.x + shift_x, originVector.y + shift_y, originVector.z + shift_z);
    }
    private Quaternion ConvertRightHandedToLeftHandedQuaternion(Quaternion rightHandedQuaternion)
    {
        return new Quaternion(-rightHandedQuaternion.x,
                               -rightHandedQuaternion.z,
                               -rightHandedQuaternion.y,
                                 rightHandedQuaternion.w);
    }
    private void MakePointAttachReferenceParent()
    {
        // point��X�ɳ�쬰cm�A�n�ରm
        float scaleParent = (0.01f / 7.5f);
        referenceParentObject.transform.localScale = new Vector3(scaleParent, scaleParent, scaleParent);
        
        // ����ܻP�ҫ��P�V
        Quaternion rotateToModel = Quaternion.Euler(0, 180, 0);
        referenceParentObject.transform.rotation = rotateToModel;

        // �첾point�Ϩ�P�ҫ����| ( �G���ץ� )
        Vector3 translationToModel = new Vector3(9.244f/7.5f, 3f/7.5f, -6.863f/7.5f);
        referenceParentObject.transform.position = translationToModel;
    }
    private Direction_right SetRotationAmount(Direction_right direction_right)
    {
        if (direction_right.x == 0 && direction_right.y == -1 && direction_right.z == 0)
        {
            direction_right.x = 0;
            direction_right.y = 0;
            direction_right.z = 180;
        }
        else if (direction_right.x == 1 && direction_right.y == 0 && direction_right.z == 0)
        {
            direction_right.x = 0;
            direction_right.y = 0;
            direction_right.z = 90;
        }
        else if (direction_right.x == 0 && direction_right.y == 0 && direction_right.z == -1)
        {
            direction_right.x = 90;
            direction_right.y = 0;
            direction_right.z = 0;
        }
        else if (direction_right.x == -1 && direction_right.y == 0 && direction_right.z == 0)
        {
            direction_right.x = 0;
            direction_right.y = 0;
            direction_right.z = 90;
        }
        else
        {
            Debug.LogError("the facing direction in txt file exceeds expectations");
        }
        return direction_right;
    }
    private void DisableAllReferencePoints()
    {
        if (this.referenceParentObject.activeSelf)
        {
            this.referenceParentObject.SetActive(false);
        }
        else
        {
            this.referenceParentObject.SetActive(true);
        }
    }
}
