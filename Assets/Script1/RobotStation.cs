using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotStation : MonoBehaviour
{
    public Vector3 position = new Vector3();
    public Quaternion quaternion = new Quaternion();
    public List<ScheduledTrajectory> StationTrajectories = new List<ScheduledTrajectory>();
    public string station_button_name;
}
