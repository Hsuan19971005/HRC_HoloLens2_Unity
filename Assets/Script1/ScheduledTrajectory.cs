using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.Trajectory;
/**ScheduledTrajectory
 * 簡述：機器人噴漆任務單一指令
 */
public class ScheduledTrajectory : MonoBehaviour
{
    public double[] joint_angles=new double[6];//機器人移動後的關節狀態
    public bool first_nozzle_state = false;//移動前的噴頭開關
    public float speed = 1;//機械手臂移動速度(0.01~1)
    public string trajectory_type;//機械手臂運動軌跡類型(non-linear or linear)
    public JointTrajectoryMsg recorded_trajectories=new JointTrajectoryMsg();//機械手臂運動軌跡
    public bool second_nozzle_state = false;//移動後的噴頭開關
    public float wait_time = 0;//最後機器人等待時間
}
