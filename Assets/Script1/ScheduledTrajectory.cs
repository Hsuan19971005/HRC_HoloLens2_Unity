using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using RosMessageTypes.Trajectory;

public class ScheduledTrajectory : MonoBehaviour
{
    public double[] joint_angles=new double[6];
    public bool first_nozzle_state = false;
    public float speed = 1;
    public string trajectory_type;//non-linear or linear
    public JointTrajectoryMsg recorded_trajectories=new JointTrajectoryMsg();
    public bool second_nozzle_state = false;
    public float wait_time = 0;
}
