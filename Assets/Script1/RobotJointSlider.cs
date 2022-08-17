using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using System;
/**RobotJointSlider �����H���`�Ʊ�
 * ²�z�G�����H��������Ʊ챱��A����P���������H�����`���A
 * �ϥΤ覡�G
 * �}����J�����H��������Ʊ쪫��
 * link��J���������H�����`Link�A�`�N�C��Link�����㦳ArticulationBody����
 * _textMesh��J�Ʊ�Ǫ���r����
 */
public class RobotJointSlider : MonoBehaviour
{
    [SerializeField]
    public ArticulationBody link;
    public TextMesh _textMesh;
    void Update()
    {
        //Update Slider Value
        float value = (this.link.xDrive.target - this.link.xDrive.lowerLimit) / (this.link.xDrive.upperLimit - this.link.xDrive.lowerLimit);
        transform.GetComponent<PinchSlider>().SliderValue = value;
        //Update Slider Text
        this._textMesh.text = Math.Round(this.link.xDrive.target, 2).ToString();
    }
    public void ChangeAngle()
    {
        var drive = this.link.xDrive;
        drive.target = transform.GetComponent<PinchSlider>().SliderValue * (this.link.xDrive.upperLimit - this.link.xDrive.lowerLimit) + this.link.xDrive.lowerLimit;
        this.link.xDrive = drive;
    }
}
