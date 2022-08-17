using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using System;
/**RobotJointSlider 機器人關節滑桿
 * 簡述：機器人控制頁面的滑桿控制，控制與反應機器人的關節狀態
 * 使用方式：
 * 腳本放入機器人控制頁面的滑桿物件中
 * link放入虛擬機器人的關節Link，注意每個Link都須具有ArticulationBody元件
 * _textMesh放入滑桿旁的文字物件
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
