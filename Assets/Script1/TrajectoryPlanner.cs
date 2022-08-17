using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Linq;
using RosMessageTypes.Trajectory;
/**TrajectoryPlanner 軌跡任務規劃器
 * 簡述：任務清單規劃頁面的UI控制、單一噴漆站點任務指令的產生與資料處理
 * 使用方式：
 * 腳本放入GameEvents底下
 * _TrajectoryPlanningPanel放入任務清單規劃頁面的物件
 * _button_template放入TrajectoryButtonTemplate這個Prefab
 */
public class TrajectoryPlanner : MonoBehaviour
{
    [SerializeField]
    public GameObject _TrajectoryPlanningPanel;
    public GameObject _button_template;
    [HideInInspector]
    private List<ScheduledTrajectory> _one_station_trajectories = new List<ScheduledTrajectory>();
    //UI elements
    private GridObjectCollection _container;
    private ScrollingObjectCollection _scrooling_object;
    private ToggleGroup _first_nozzle;
    private ToggleGroup _second_nozzle;
    private ToggleGroup _trajectory_type;
    private Slider _speed_slider;
    private Slider _wait_time_slider;
    private TextMeshProUGUI _speed_text;
    private TextMeshProUGUI _wait_time_text;
    private GameObject _submit_button;
    private GameObject _delete_button;
    private GameObject _run_all_traj_button;
    private GameObject _pub_to_real_robot_button;
    private List<GameObject> _pose_button_list=new List<GameObject>();
    void Start()
    {
        this._container = this._TrajectoryPlanningPanel.transform.Find("ScrollingContent/Container").GetComponent<GridObjectCollection>();
        this._scrooling_object = this._TrajectoryPlanningPanel.transform.Find("ScrollingContent").GetComponent<ScrollingObjectCollection>();
        GameObject Column1 = this._TrajectoryPlanningPanel.transform.Find("TrajectoryParameterPanel/UGUIScrollViewContent/Content/GridLayout1/Column1").gameObject;
        this._first_nozzle = Column1.transform.Find("FirstSpray/FirstSprayToggleGroup").GetComponent<ToggleGroup>();
        this._second_nozzle = Column1.transform.Find("SecondSpray/SecondSprayToggleGroup").GetComponent<ToggleGroup>();
        this._trajectory_type= Column1.transform.Find("Trajectory/TrajectoryToggleGroup").GetComponent<ToggleGroup>();
        this._speed_slider = Column1.transform.Find("Speed/SpeedSlider").GetComponent<Slider>();
        this._speed_text = Column1.transform.Find("Speed/SpeedText").GetComponent<TextMeshProUGUI>();
        this._wait_time_slider = Column1.transform.Find("WaitTime/WaitTimeSlider").GetComponent<Slider>();
        this._wait_time_text = Column1.transform.Find("WaitTime/WaitTimeText").GetComponent<TextMeshProUGUI>();
        this._submit_button = this._TrajectoryPlanningPanel.transform.Find("TrajectoryParameterPanel/SubmitButton").gameObject;
        this._submit_button.GetComponent<ButtonConfigHelper>().OnClick.AddListener(()=>SubmitNewTrajectory());
        this._delete_button = this._TrajectoryPlanningPanel.transform.Find("DeleteButton").gameObject;
        this._delete_button.GetComponent<ButtonConfigHelper>().OnClick.AddListener(DeleteLastPoseButton);
        this._run_all_traj_button= this._TrajectoryPlanningPanel.transform.Find("RunAllTrajButton").gameObject;
        this._run_all_traj_button.GetComponent<ButtonConfigHelper>().OnClick.AddListener(()=>GameEvents.current.DemoOneStationTrajectory(this._one_station_trajectories));
        this._pub_to_real_robot_button= this._TrajectoryPlanningPanel.transform.Find("PubToRealRobotButton").gameObject;
        this._pub_to_real_robot_button.GetComponent<ButtonConfigHelper>().OnClick.AddListener(()=> GameEvents.current.GiveCommandsToRealRobot(this._one_station_trajectories));
        //GmaeEvents
        GameEvents.current.actionInitializeTrajectoryPlanningPanel += InitializeTrajectoryPlanningPanel;
        GameEvents.current.actionSetRespondTrajectory += SetRespondTrajectory;
    }

    void Update()
    {
        this._speed_text.text = Math.Round(this._speed_slider.value, 2).ToString();
        this._wait_time_text.text = Math.Round(this._wait_time_slider.value, 1).ToString();
        if (this._pose_button_list.Count == 0) this._delete_button.SetActive(false);
        else this._delete_button.SetActive(true);

        //Test
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("station_traj.count="+this._one_station_trajectories.Count);
        }
    }
    public void InitializeTrajectoryPlanningPanel(List<ScheduledTrajectory> station_trajectory)
    {
        //Activate the TrajectoryPlanningPanel
        if (!this._TrajectoryPlanningPanel.activeSelf) this._TrajectoryPlanningPanel.SetActive(true);
        //Set the RobotStation's station_trajectory to this class
        this._one_station_trajectories = station_trajectory;
        //Destroy old buttons
        DestroyAllPoseButtons();
        //Generate the button according to the info of station_trajectory
        for(int i = 0; i < this._one_station_trajectories.Count; i++)
        {
            GenerateNewPoseButton(i);
        }
    }
    private void SubmitNewTrajectory()
    {
        //***Get new trajectory data from panel***//
        Toggle tmp_toggle;
        //first nozzle
        bool new_first_nozzle;
        tmp_toggle=this._first_nozzle.ActiveToggles().FirstOrDefault();
        if (tmp_toggle.name == "Toggle_on") new_first_nozzle = true;
        else new_first_nozzle = false;
        //speed
        float new_speed = (float)Math.Round(this._speed_slider.value,2);
        //traj_type
        string new_traj_type;
        tmp_toggle = this._trajectory_type.ActiveToggles().FirstOrDefault();
        if (tmp_toggle.name == "Toggle_non_linear") new_traj_type = "non-linear";
        else new_traj_type = "linear";
        //second nozzle
        bool new_second_nozzle;
        tmp_toggle = this._second_nozzle.ActiveToggles().FirstOrDefault();
        if (tmp_toggle.name == "Toggle_on") new_second_nozzle = true;
        else new_second_nozzle = false;
        //wait time
        float new_wait_time = (float)Math.Round(this._wait_time_slider.value,1);
        //angles
        double[] new_angles = GameEvents.current.GetRobotAllJointPositions();

        //***Add new scheduled_trajectory to one_station_trajectory***//
        ScheduledTrajectory new_scheduled_traj = new ScheduledTrajectory();
        new_scheduled_traj.first_nozzle_state = new_first_nozzle;
        new_scheduled_traj.second_nozzle_state = new_second_nozzle;
        new_scheduled_traj.speed = new_speed;
        new_scheduled_traj.wait_time = new_wait_time;
        new_scheduled_traj.trajectory_type = new_traj_type;
        new_scheduled_traj.joint_angles = new_angles;
        this._one_station_trajectories.Add(new_scheduled_traj);

        //***Generate a new Button***//
        GenerateNewPoseButton(this._one_station_trajectories.Count - 1);

        //***Send recorded_trajectory calculation request, if the new scheduled_trajectory is not the first one.***//
        JointTrajectoryMsg recorded_trajecotry=new JointTrajectoryMsg();
        if (this._one_station_trajectories.Count>1)
        {
            //Find previous robot angles
            double[] previous_angles = this._one_station_trajectories[this._one_station_trajectories.Count - 2].joint_angles;
            double[] pre_angles = new double[6];
            for(int i = 0; i < 6; i++)
            {
                pre_angles[i] = previous_angles[i];
            }
            //Set new scheduled_trajectory button name
            string new_button_name = "Pose"+(this._one_station_trajectories.Count);
            if (new_traj_type == "non-linear") GameEvents.current.CalculateNonLinearTrajectoryForScheduledTrajectory(pre_angles, new_button_name,new_speed);
            else GameEvents.current.CalculateLinearTrajectoryForScheduledTrajectory(previous_angles, new_button_name,new_speed);
        }
    }
    private void GenerateNewPoseButton(int indexOfTraj)
    {
        GameObject oneButton = Instantiate(this._button_template, this._container.transform);
        oneButton.GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => GameEvents.current.MoveRobotByJointPositions(this._one_station_trajectories[indexOfTraj].joint_angles));
        //***Button's appearance***//
        //name
        oneButton.transform.name = "Pose" + (indexOfTraj + 1).ToString();
        oneButton.transform.Find("PoseText").GetComponent<TextMesh>().text = oneButton.transform.name;
        //first nozzle state
        if (this._one_station_trajectories[indexOfTraj].first_nozzle_state)
        {
            oneButton.transform.Find("Content1").GetComponent<TextMesh>().text = "1. 1st nozzle: on";
        }
        else
        {
            oneButton.transform.Find("Content1").GetComponent<TextMesh>().text = "1. 1st nozzle: off";
        }
        //speed
        oneButton.transform.Find("Content2").GetComponent<TextMesh>().text = "2. Speed: " + this._one_station_trajectories[indexOfTraj].speed.ToString();
        //Traj type
        if (this._one_station_trajectories[indexOfTraj].trajectory_type == "non-linear")
        {
            oneButton.transform.Find("Content3").GetComponent<TextMesh>().text = "3. Traj: non-linear";
        }
        else
        {
            oneButton.transform.Find("Content3").GetComponent<TextMesh>().text = "3. Traj: linear";
        }

        //second nozzle state
        if (this._one_station_trajectories[indexOfTraj].second_nozzle_state)
        {
            oneButton.transform.Find("Content4").GetComponent<TextMesh>().text = "4. 2nd nozzle: on";
        }
        else
        {
            oneButton.transform.Find("Content4").GetComponent<TextMesh>().text = "4. 2nd nozzle: off";
        }
        //wait time
        oneButton.transform.Find("Content5").GetComponent<TextMesh>().text = "5. Wait time: " + this._one_station_trajectories[indexOfTraj].wait_time.ToString() + "s";
        //the recorded trajectory state (Note: first button don't contain trajecotry!)
        if (this._one_station_trajectories[indexOfTraj].recorded_trajectories.points.Length != 0 || indexOfTraj == 0)
        {
            oneButton.transform.Find("Content6").GetComponent<TextMesh>().text = "OK";
            oneButton.transform.Find("Content6").GetComponent<TextMesh>().color = Color.green;
        }
        else
        {
            oneButton.transform.Find("Content6").GetComponent<TextMesh>().text = "X";
            oneButton.transform.Find("Content6").GetComponent<TextMesh>().color = Color.red;
        }

        this._pose_button_list.Add(oneButton);//Add generated button to the _pose_button_list
        StartCoroutine(InvokeUpdateCollection());//Rearrange the buttons' order
        this._scrooling_object.UpdateContent();
    }
    private void DestroyAllPoseButtons()
    {
        for(int i = 0; i < this._pose_button_list.Count; i++)
        {
            Destroy(this._container.transform.Find(this._pose_button_list[i].name).gameObject);
        }
        this._pose_button_list.Clear();
        StartCoroutine(InvokeUpdateCollection());//Rearrange the buttons' order
        this._scrooling_object.UpdateContent();
    }
    private void SetRespondTrajectory(JointTrajectoryMsg respondTraj, string poseButtonName)
    {
        int index = FindIndexOfPoseButtons(poseButtonName);
        if (index == -1)
        {
            Debug.LogError("SetRespondTrajectory() in TrajectoryPlanner.cs Wrong poseButtonName! ButtonName:"+poseButtonName);
            return;
        }
        this._one_station_trajectories[index].recorded_trajectories = respondTraj;
        TextMesh text_mesh = this._container.transform.Find(poseButtonName + "/Content6").GetComponent<TextMesh>();
        if (respondTraj.points.Length > 0)
        {
            text_mesh.text = "OK";
            text_mesh.color = Color.green;
        }
        else
        {
            text_mesh.text = "X";
            text_mesh.color = Color.red;
        }
    }
    private int FindIndexOfPoseButtons(string name)
    {
        for(int i = 0; i < this._pose_button_list.Count; i++)
        {
            if (this._pose_button_list[i].name == name) return i;
        }
        return -1;
    }
    private void DeleteLastPoseButton()
    {
        int index = this._pose_button_list.Count - 1;
        this._one_station_trajectories.RemoveAt(index);
        Destroy(this._pose_button_list[index]);
        this._pose_button_list.RemoveAt(index);
        this._scrooling_object.UpdateContent();
    }
    private IEnumerator InvokeUpdateCollection()
    {
        yield return null;
        this._container.GetComponent<GridObjectCollection>().UpdateCollection();
    }
}

