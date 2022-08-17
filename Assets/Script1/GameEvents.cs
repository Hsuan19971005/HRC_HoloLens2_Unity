using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Trajectory;
using System;
public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
    public event Action actionMoveRobotByInterpolationTrajectory;
    public event Action actionMoveRobotByLinearTrajectory;
    public event Action<double[]> actionMoveRobotByJointPositions;
    public event Action<List<ScheduledTrajectory>> actionInitializeTrajectoryPlanningPanel;
    public event Action<double[], string,float> actionCalculateNonLinearTrajectoryForScheduledTrajectory;
    public event Action<double[], string,float> actionCalculateLinearTrajectoryForScheduledTrajectory;
    public event Action<JointTrajectoryMsg, string> actionSetRespondTrajectory;
    public event Action<List<ScheduledTrajectory>> actionDemoOneStationTrajectory;
    public event Action actionDisableAllReferencePoints;
    public event Action<Vector3,Quaternion> actionMoveRobotBase;
    public event Action<List<ScheduledTrajectory>> actionGiveCommandsToRealRobot;
    public event Action actionSwitchBimMoveAxisConstraint;
    public event Action actionSwitchBimMeshRender;
    private event Func<Transform> funcRobotTransform;
    private event Func<double[]> funcRobotAllJointPositions;
    // Start is called before the first frame update
    private void Awake()
    {
        current = this;
    }
    //Action
    public void MoveRobotByInterpolationTrajectory()
    {
        if (actionMoveRobotByInterpolationTrajectory != null)
        {
            actionMoveRobotByInterpolationTrajectory();
        }
    }
    public void MoveRobotByLinearTrajectory()
    {
        if (actionMoveRobotByLinearTrajectory != null)
        {
            actionMoveRobotByLinearTrajectory();
        }
    }
    public void MoveRobotByJointPositions(double[]jointPositions)
    {
        if(actionMoveRobotByJointPositions != null)
        {
            actionMoveRobotByJointPositions(jointPositions);
        }
    }
    public void InitializeTrajectoryPlanningPanel(List<ScheduledTrajectory> scheduledTrajectories)
    {
        if (actionInitializeTrajectoryPlanningPanel != null)
        {
            actionInitializeTrajectoryPlanningPanel(scheduledTrajectories);
        }
    }
    public void CalculateNonLinearTrajectoryForScheduledTrajectory(double[]previousJointAngles,string poseButtonName, float speedFactor)
    {
        if (actionCalculateNonLinearTrajectoryForScheduledTrajectory != null)
        {
            actionCalculateNonLinearTrajectoryForScheduledTrajectory(previousJointAngles, poseButtonName,speedFactor);
        }
    }
    public void CalculateLinearTrajectoryForScheduledTrajectory(double[] previousJointAngles, string poseButtonName, float speedFactor)
    {
        if (actionCalculateLinearTrajectoryForScheduledTrajectory != null)
        {
            actionCalculateLinearTrajectoryForScheduledTrajectory(previousJointAngles, poseButtonName,speedFactor);
        }
    }
    public void SetRespondTrajectory(JointTrajectoryMsg respondTraj, string poseButtonName)
    {
        if (actionSetRespondTrajectory != null)
        {
            actionSetRespondTrajectory(respondTraj,poseButtonName);
        }
    }
    public void DemoOneStationTrajectory(List<ScheduledTrajectory>scheduledTrajectory)
    {
        if (actionDemoOneStationTrajectory != null)
        {
            actionDemoOneStationTrajectory(scheduledTrajectory);
        }
    }
    public void DisableAllReferencePoints()
    {
        if(actionDisableAllReferencePoints != null)
        {
            actionDisableAllReferencePoints();
        }
    }
    public void MoveRobotBase(Vector3 position, Quaternion quaternion)
    {
        if (actionMoveRobotBase != null)
        {
            actionMoveRobotBase(position,quaternion);
        }
    }
    public void GiveCommandsToRealRobot(List<ScheduledTrajectory> stationTrajectories)
    {
        if (this.actionGiveCommandsToRealRobot != null)
        {
            actionGiveCommandsToRealRobot(stationTrajectories);
        }
    }
    public void SwitchBimMoveAxisConstraint()
    {
        if (this.actionSwitchBimMoveAxisConstraint != null)
        {
            actionSwitchBimMoveAxisConstraint();
        }
    }
    public void SwitchBimMeshRender()
    {
        if (this.actionSwitchBimMeshRender != null)
        {
            actionSwitchBimMeshRender();
        }
    }
    //Func
    public void InitiateFuncRobotAllJointPositions(Func<double[]> jointPositions)
    {
        funcRobotAllJointPositions = jointPositions;
    }
    public double[] GetRobotAllJointPositions()
    {
        if (funcRobotAllJointPositions != null)
        {
            return funcRobotAllJointPositions();
        }
        return null;
    }
    public void SetFuncRobotTransform(Func<Transform> robotTransform)
    {
        funcRobotTransform = robotTransform;
    }
    public Transform GetRobotTransform()
    {
        if(funcRobotTransform != null)
        {
            return funcRobotTransform();
        }
        return null;
    }
}
