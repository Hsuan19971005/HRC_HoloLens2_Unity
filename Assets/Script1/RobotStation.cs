using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**RobotStation
 * 簡述：機器人噴漆站點的資料格式
 */
public class RobotStation : MonoBehaviour
{
    public Vector3 position = new Vector3();//機器人基座座標
    public Quaternion quaternion = new Quaternion();//機器人基座四元數
    public List<ScheduledTrajectory> StationTrajectories = new List<ScheduledTrajectory>();//機器人噴漆站點的任務清單
    public string station_button_name;//UI介面的Button ID名稱
}
