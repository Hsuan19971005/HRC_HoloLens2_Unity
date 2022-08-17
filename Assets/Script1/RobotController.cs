using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.Trajectory;
using RosMessageTypes.Std;
using RosMessageTypes.HsuanMoveitCommand;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
/**RobotController 機器人控制器
 * 簡述：負責機器人本體操控相關功能與ROS之間的通訊
 * 使用方式：
 * 腳本放入GameEvents底下
 * m_MirobotOne放入機械手臂物件
 * _ref_end_effector放入虛擬末端效應器的指示物件，效應器物件應位在機械手臂物件的的base物件下
 * _recommanded_work_area放入有效工作空間物件，工作空間物件應位在機械手臂物件的的base物件下
 * _ref_base放入虛擬基座
 * _main_camera放入相機
 * _robot_control_panel放入機器人控制頁面物件
 * _station_trajectory_panel放入基座位置選定頁面
 * _painting_head放入噴漆粒子效果的Prefab "PaintnigHead"
 */
public class RobotController : MonoBehaviour
{
    // Hardcoded variables
    const int k_NumRobotJoints = 6;
    const float k_JointAssignmentWait = 0.1f;
    const float k_PoseAssignmentWait = 0.5f;
    public static readonly string[] LinkNamesPrototype =
        { "base_link/Link1", "/Link2", "/Link3", "/Link4", "/Link5", "/Link6" };

    // Variables required for ROS communication
    [SerializeField]
    public string _ros_service_name_interpolation = "nonlinear_move";
    public string _ros_service_name_linear = "linear_move";
    public string _ros_msg_name_nozzle_state = "nozzle/power_state";
    public string _ros_service_name_trajectory_execution = "trajectory_execution";
    public GameObject m_MirobotOne;
    public GameObject _ref_end_effector;//ref_end_effector should under the robot's base
    public GameObject _recommanded_work_area;//worksapce should under the robot's base
    public GameObject _ref_base;
    public GameObject _main_camera;
    public GameObject _robot_control_panel;
    public GameObject _station_trajectory_panel;
    public GameObject _painting_head;

    // Articulation Bodies
    private ArticulationBody[] m_JointArticulationBodies;

    // ROS Connector
    private ROSConnection m_Ros_interpolation;
    private ROSConnection m_Ros_linear;

    private ROSConnection m_ROS_nozzle_state;
    private ROSConnection m_ROS_trajectory_execution;

    //For calculating recorded_trajecotory of ScheduledTrajectory
    private string _pose_button_name = "";

    //Real Robot execute trajectories condition
    private bool RealRobotTrajectoryExecutionComplete = false;

    void Start()
    {
        //設定ROSConnection名稱
        m_Ros_interpolation = ROSConnection.GetOrCreateInstance();
        m_Ros_linear = ROSConnection.GetOrCreateInstance();
        m_ROS_nozzle_state = ROSConnection.GetOrCreateInstance();
        m_ROS_trajectory_execution = ROSConnection.GetOrCreateInstance();
        m_Ros_interpolation.RegisterRosService<HsuanMoveServiceRequest, HsuanMoveServiceResponse>(_ros_service_name_interpolation);
        m_Ros_linear.RegisterRosService<HsuanMoveServiceRequest, HsuanMoveServiceResponse>(_ros_service_name_linear);
        m_ROS_nozzle_state.RegisterPublisher<BoolMsg>(_ros_msg_name_nozzle_state);
        m_ROS_trajectory_execution.RegisterRosService<TrajectoryExecutionServiceRequest, TrajectoryExecutionServiceResponse>(_ros_service_name_trajectory_execution);
        m_JointArticulationBodies = new ArticulationBody[k_NumRobotJoints];
        //抓取Robot的Articulatoin
        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += LinkNamesPrototype[i];
            m_JointArticulationBodies[i] = m_MirobotOne.transform.Find(linkName).GetComponent<ArticulationBody>();
        }
        this._painting_head.GetComponent<ParticleSystem>().Stop();

        //GameEventSystem
        GameEvents.current.actionMoveRobotByInterpolationTrajectory += MoveRobotToPoseByROSNonLinearNode;
        GameEvents.current.actionMoveRobotByLinearTrajectory += MoveRobotToPoseByROSLinearNode;
        GameEvents.current.actionMoveRobotByJointPositions += MoveRobotByJointPositions;
        GameEvents.current.actionCalculateNonLinearTrajectoryForScheduledTrajectory += CalculateNonLinearTrajectoryForScheduledTrajectory;
        GameEvents.current.actionCalculateLinearTrajectoryForScheduledTrajectory += CalculateLinearTrajectoryForScheduledTrajectory;
        GameEvents.current.InitiateFuncRobotAllJointPositions(GetRobotAllJointPositions);
        GameEvents.current.actionDemoOneStationTrajectory += DemoOneStationTrajectory;
        GameEvents.current.SetFuncRobotTransform(GetRobotTransform);
        GameEvents.current.actionMoveRobotBase += MoveBasePosition;
        GameEvents.current.actionGiveCommandsToRealRobot += GiveCommandsToRealRobot;
    }
    //Move robot to Target End Effector, including Non-linear & Linear
    public void MoveRobotToPoseByROSNonLinearNode()
    {
        HsuanMoveServiceRequest request = GenerateHsuanMoveService(1f);
        m_Ros_interpolation.SendServiceMessage<HsuanMoveServiceResponse>(_ros_service_name_interpolation, request, MoveAccordingToTrajectory);//Send service message
    }
    public void MoveRobotToPoseByROSLinearNode()
    {
        HsuanMoveServiceRequest request = GenerateHsuanMoveService(1f);
        m_Ros_linear.SendServiceMessage<HsuanMoveServiceResponse>(_ros_service_name_linear, request, MoveAccordingToTrajectory);//Send service message
    }
    private void MoveAccordingToTrajectory(HsuanMoveServiceResponse response)
    {
        if (response.trajectories.points.Length > 0)
        {
            Debug.Log("Trajectory returned.");
            StartCoroutine(ExecuteTrajectories(response));
        }
        else
        {
            Debug.LogError("No trajectory returned from interpolation_move_server.");
        }
    }
    IEnumerator ExecuteTrajectories(HsuanMoveServiceResponse response)
    {
        if (response.trajectories != null)
        {
            foreach (var t in response.trajectories.points)
            {
                var jointPositions = t.positions;
                var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();
                // Set the joint values for every joint
                for (var joint = 0; joint < m_JointArticulationBodies.Length; joint++)
                {
                    var joint1XDrive = m_JointArticulationBodies[joint].xDrive;
                    joint1XDrive.target = result[joint];
                    m_JointArticulationBodies[joint].xDrive = joint1XDrive;
                }

                // Wait for robot to achieve pose for all joint assignments
                yield return new WaitForSeconds(k_JointAssignmentWait);
            }
        }
    }
    private HsuanMoveServiceRequest GenerateHsuanMoveService(float speed_factor)
    {
        HsuanMoveServiceRequest request = new HsuanMoveServiceRequest();
        //Set current robot 6 angles
        request.hsuan_moveit_joints.start_joint_state.name = new string[6] { "joint1", "joint2", "joint3", "joint4", "joint5", "joint6" };
        request.hsuan_moveit_joints.start_joint_state.position = GetRobotAllJointPositions();
        //Calculate Relative Target position and  Target rotation
        Transform base_transform = this.m_MirobotOne.transform.Find("base_link").transform;
        Vector3 target_position = this._ref_end_effector.transform.position - base_transform.position;
        target_position = Quaternion.Inverse(base_transform.rotation) * target_position;
        Quaternion target_quaternion = this._ref_end_effector.transform.localRotation;
        //Set target end effector pose
        PoseMsg poseMsg = new PoseMsg
        {
            position = target_position.To<FLU>(),
            orientation = target_quaternion.To<FLU>()
        };
        request.hsuan_moveit_joints.end_pose = poseMsg;
        //Set speed
        if (speed_factor > 1 || speed_factor < 0.01f) speed_factor = 1;
        request.hsuan_moveit_joints.speed_factor = speed_factor;
        return request;
    }
    private void MoveRobotByJointPositions(double[] jointPositions)
    {
        for (int i = 0; i < k_NumRobotJoints; i++)
        {
            var drive = this.m_JointArticulationBodies[i].xDrive;
            drive.target = (float)(jointPositions[i] * 180f / Math.PI);
            this.m_JointArticulationBodies[i].xDrive = drive;
        }
    }
    private double[] GetRobotAllJointPositions()
    {
        double[] angles = new double[k_NumRobotJoints];
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            angles[i] = m_JointArticulationBodies[i].jointPosition[0];
        }
        return angles;
    }
    private void DemoOneStationTrajectory(List<ScheduledTrajectory> scheduledTrajectories)
    {
        StartCoroutine(ExecuteOneStationTrajectory(scheduledTrajectories));
    }
    IEnumerator ExecuteOneStationTrajectory(List<ScheduledTrajectory> scheduledTrajectories)
    {
        foreach (var one_traj in scheduledTrajectories)
        {
            //First pose
            if (one_traj.recorded_trajectories.points.Length == 0)
            {
                MoveRobotByJointPositions(one_traj.joint_angles);
                yield return new WaitForSeconds(k_JointAssignmentWait);// Wait for robot to achieve pose for all joint assignments
            }
            //First spray
            if (one_traj.first_nozzle_state) this._painting_head.GetComponent<ParticleSystem>().Play();
            else this._painting_head.GetComponent<ParticleSystem>().Stop();
            //Execute traj
            foreach (var t in one_traj.recorded_trajectories.points)
            {
                var jointPositions = t.positions;
                var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();
                // Set the joint values for every joint
                for (var joint = 0; joint < m_JointArticulationBodies.Length; joint++)
                {
                    var joint1XDrive = m_JointArticulationBodies[joint].xDrive;
                    joint1XDrive.target = result[joint];
                    m_JointArticulationBodies[joint].xDrive = joint1XDrive;
                }
                yield return new WaitForSeconds(k_JointAssignmentWait);// Wait for robot to achieve pose for all joint assignments
            }
            //Second spray
            if (one_traj.second_nozzle_state) this._painting_head.GetComponent<ParticleSystem>().Play();
            else this._painting_head.GetComponent<ParticleSystem>().Stop();
            //Wait time
            yield return new WaitForSeconds(one_traj.wait_time);
        }
    }
    private Transform GetRobotTransform()
    {
        return this.m_MirobotOne.transform.Find("base_link").transform;
    }
    //Calculating recorded_trajecotory of ScheduledTrajectory
    private void CalculateNonLinearTrajectoryForScheduledTrajectory(double[] previousJointAngles, string poseButtonNmae, float speedFactor)
    {
        this._pose_button_name = poseButtonNmae;
        HsuanMoveServiceRequest request = GenerateHsuanMoveService(speedFactor);
        request.hsuan_moveit_joints.start_joint_state.position = previousJointAngles;//Reset the joint angles
        m_Ros_interpolation.SendServiceMessage<HsuanMoveServiceResponse>(_ros_service_name_interpolation, request, PassResultOfScheduledTrajectoryToStationTrajectory);//Send service message
    }
    private void CalculateLinearTrajectoryForScheduledTrajectory(double[] previousJointAngles, string poseButtonNmae, float speedFactor)
    {
        this._pose_button_name = poseButtonNmae;
        HsuanMoveServiceRequest request = GenerateHsuanMoveService(speedFactor);
        request.hsuan_moveit_joints.start_joint_state.position = previousJointAngles;//Reset the joint angles
        m_Ros_interpolation.SendServiceMessage<HsuanMoveServiceResponse>(_ros_service_name_linear, request, PassResultOfScheduledTrajectoryToStationTrajectory);//Send service message
    }
    private void PassResultOfScheduledTrajectoryToStationTrajectory(HsuanMoveServiceResponse response)
    {
        GameEvents.current.SetRespondTrajectory(response.trajectories, this._pose_button_name);
    }

    /**Make real robot execute series of commands. 
     */
    public void GiveCommandsToRealRobot(List<ScheduledTrajectory> stationTrajectories)
    {
        StartCoroutine(GiveCommandsToRealRobotCoroutine(stationTrajectories));
    }
    IEnumerator GiveCommandsToRealRobotCoroutine(List<ScheduledTrajectory> stationTrajectories)
    {
        this.RealRobotTrajectoryExecutionComplete = false;
        for(int i = 0; i < stationTrajectories.Count; i++)
        {
            SendNozzleMsgToROS(stationTrajectories[i].first_nozzle_state);
            yield return null;
            if (i != 0)
            {
                SendTrajectoriesToROS(stationTrajectories[i].recorded_trajectories, stationTrajectories[i].speed);
                yield return new WaitWhile(() => this.RealRobotTrajectoryExecutionComplete == false);
                Debug.Log("wait end! this.RealRobotTrajExecutionCOmplete="+this.RealRobotTrajectoryExecutionComplete);
                this.RealRobotTrajectoryExecutionComplete = false;
            }
            SendNozzleMsgToROS(stationTrajectories[i].second_nozzle_state);
            yield return null;
            yield return new WaitForSecondsRealtime(stationTrajectories[i].wait_time);
        }
    }
    public void SendNozzleMsgToROS(bool nozzle_state)
    {
        BoolMsg answer = new BoolMsg(nozzle_state);
        m_ROS_nozzle_state.Publish(_ros_msg_name_nozzle_state,answer);
    }
    public void SendTrajectoriesToROS(JointTrajectoryMsg trajectoryMsg, float speed_factor)
    {
        TrajectoryExecutionServiceRequest request = new TrajectoryExecutionServiceRequest();
        request.trajectories = trajectoryMsg;
        request.speed_factor = speed_factor;
        m_Ros_interpolation.SendServiceMessage<TrajectoryExecutionServiceResponse>(_ros_service_name_trajectory_execution, request, SetRealRobotTrajectoryExecutionComplete);//Send service message
    }
    public void SetRealRobotTrajectoryExecutionComplete(TrajectoryExecutionServiceResponse response)
    {
        if (response.complete_execution.data == true) this.RealRobotTrajectoryExecutionComplete = true;
        else Debug.LogError("TrajExecution fail!");
    }
    //Other supporting methods
    public void SwitchRecommandedWrokArea()
    {
        bool state = this._recommanded_work_area.GetComponent<MeshRenderer>().enabled;
        if (state == false) this._recommanded_work_area.GetComponent<MeshRenderer>().enabled = true;
        else this._recommanded_work_area.GetComponent<MeshRenderer>().enabled = false;
    }
    public void SwitchRefEndEffector()
    {
        bool state = this._ref_end_effector.activeSelf;
        if (state == true) this._ref_end_effector.SetActive(false);
        else
        {
            this._ref_end_effector.SetActive(true);
            var link6 = this.m_MirobotOne.transform.Find("base_link/Link1/Link2/Link3/Link4/Link5/Link6");
            this._ref_end_effector.transform.position = link6.transform.position;
            this._ref_end_effector.transform.rotation = link6.transform.rotation;
        }
    }
    public void SwitchRefBase()
    {
        bool state = this._ref_base.activeSelf;
        if (state == true) this._ref_base.SetActive(false);
        else
        {
            this._ref_base.SetActive(true);
            this._ref_base.transform.position = this._main_camera.transform.position + this._main_camera.transform.forward * 0.4f;
            this._ref_base.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void MoveBasePosition()
    {
        var articulation = this.m_MirobotOne.transform.Find("base_link").GetComponent<ArticulationBody>();
        articulation.TeleportRoot(this._ref_base.transform.position, this._ref_base.transform.rotation);
    }
    public void MoveBasePosition(Vector3 position, Quaternion quaternion)
    {
        Debug.Log(transform.position.x+", "+ transform.position.y + ", "+transform.position.z + ", "+transform.rotation.x + ", " + transform.rotation.y + ", " + transform.rotation.z + ", ");
        var articulation = this.m_MirobotOne.transform.Find("base_link").GetComponent<ArticulationBody>();
        articulation.TeleportRoot(position, quaternion);
    }
    public void SwitchRobotControlPanel()
    {
        if (this._robot_control_panel.activeSelf)
        {
            this._robot_control_panel.SetActive(false);
        }
        else
        {
            this._robot_control_panel.SetActive(true);
            this._robot_control_panel.transform.position= this._main_camera.transform.position + this._main_camera.transform.forward * 0.4f;
        }
    }
    public void SwitchStationTrajectoryPanel()
    {
        if (this._station_trajectory_panel.activeSelf)
        {
            this._station_trajectory_panel.SetActive(false);
        }
        else
        {
            this._station_trajectory_panel.SetActive(true);
            this._station_trajectory_panel.transform.position = this._main_camera.transform.position + this._main_camera.transform.forward * 0.4f;
        }
    }
    public void SwitchPaintingNozzle()
    {
        if (this._painting_head.GetComponent<ParticleSystem>().isPlaying) this._painting_head.GetComponent<ParticleSystem>().Stop();
        else this._painting_head.GetComponent<ParticleSystem>().Play();
    }
    //Robot to certain pose
    public void MoveToDefaultPoseHome()
    {
        foreach (var i in m_JointArticulationBodies)
        {
            var drive = i.xDrive;
            drive.target = 0;
            i.xDrive = drive;
        }
    }
    public void MoveToDefaultPoseReady()
    {
        for (int i = 0; i < 6; i++)
        {
            var drive = m_JointArticulationBodies[i].xDrive;
            if (i == 1) drive.target = -30f;
            else if (i == 2) drive.target = 30f;
            else drive.target = 0;
            m_JointArticulationBodies[i].xDrive = drive;
        }
    }
    //End effector axis constraint
    public void ConstraintEndEffectorMoveByXAxis()
    {
        this._ref_end_effector.GetComponent<MoveAxisConstraint>().ConstraintOnMovement= (AxisFlags)6;
        this._ref_end_effector.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = (AxisFlags)(-1);
    }
    public void ConstraintEndEffectorMoveByYAxis()
    {
        this._ref_end_effector.GetComponent<MoveAxisConstraint>().ConstraintOnMovement = (AxisFlags)5;
        this._ref_end_effector.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = (AxisFlags)(-1);
    }
    public void ConstraintEndEffectorMoveByZAxis()
    {
        this._ref_end_effector.GetComponent<MoveAxisConstraint>().ConstraintOnMovement = (AxisFlags)3;
        this._ref_end_effector.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = (AxisFlags)(-1);
    }
    public void ConstraintEndEffectorAllMoveAxis()
    {
        this._ref_end_effector.GetComponent<MoveAxisConstraint>().ConstraintOnMovement = (AxisFlags)(-1);
        this._ref_end_effector.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = (AxisFlags)0;
    }
    public void UnConstraintEndEffector()
    {
        this._ref_end_effector.GetComponent<MoveAxisConstraint>().ConstraintOnMovement = (AxisFlags)0;
        this._ref_end_effector.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = (AxisFlags)0;
    }
}
