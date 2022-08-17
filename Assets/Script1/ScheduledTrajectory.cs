using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.Trajectory;
/**ScheduledTrajectory
 * ²�z�G�����H�Q�����ȳ�@���O
 */
public class ScheduledTrajectory : MonoBehaviour
{
    public double[] joint_angles=new double[6];//�����H���ʫ᪺���`���A
    public bool first_nozzle_state = false;//���ʫe���Q�Y�}��
    public float speed = 1;//������u���ʳt��(0.01~1)
    public string trajectory_type;//������u�B�ʭy������(non-linear or linear)
    public JointTrajectoryMsg recorded_trajectories=new JointTrajectoryMsg();//������u�B�ʭy��
    public bool second_nozzle_state = false;//���ʫ᪺�Q�Y�}��
    public float wait_time = 0;//�̫�����H���ݮɶ�
}
