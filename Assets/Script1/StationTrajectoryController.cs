using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
/**StationTrajectoryCOntoller 噴漆站點控制器
 * 簡述：基座位置選定頁面的UI控制、所有噴漆站點的資料處理
 * 使用方式：
 * 腳本放入GameEvents底下
 * _StationTrajectoryPanel放入基座位置選定頁面的物件
 * _button_template放入StationButtonTemplate這個Prefab
 */
public class StationTrajectoryController : MonoBehaviour
{
    [SerializeField]
    public GameObject _StationTrajectoryPanel;
    public GameObject _button_template;
    [HideInInspector]
    private List<RobotStation> AllRobotStations = new List<RobotStation>();//All Stations' Information
    private GridObjectCollection _container;
    private ScrollingObjectCollection _scrooling_object;
    private void Start()
    {
        this._container = this._StationTrajectoryPanel.transform.Find("ScrollingContent/Container").GetComponent< GridObjectCollection>();
        this._scrooling_object = this._StationTrajectoryPanel.transform.Find("ScrollingContent").GetComponent<ScrollingObjectCollection>();
        this._StationTrajectoryPanel.transform.Find("AddStationButton").GetComponent<ButtonConfigHelper>().OnClick.AddListener(()=>AddOneStation());
    }
    private void AddOneStation()
    {
        //***Add a new station list button, and it means that we also need to extend　AddRobotStations.
        
        //***Fristly add a new button and set it's OnClick event. The new button's name has to be set as it's GameObject ID.
        GameObject oneButton= Instantiate(this._button_template, this._container.transform);
        oneButton.transform.name = oneButton.transform.GetInstanceID().ToString();
        oneButton.GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => ButtonClick(oneButton.transform.GetInstanceID()));
        oneButton.transform.Find("DeleteStationButton").GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => DeleteStationButton(oneButton.transform.GetInstanceID()));
        oneButton.transform.Find("MoveRobotButton").GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => MoveRobotBase(oneButton.transform.GetInstanceID()));
        oneButton.transform.Find("StationTitle").GetComponent<TextMesh>().text = "Station_ID: " + oneButton.transform.GetInstanceID().ToString();
        StartCoroutine(InvokeUpdateCollection());//Rearrange the buttons' order
        this._scrooling_object.UpdateContent();

        //***Secondly extend AllRobotStations. Also we need to set it's station_button_name to new button's name, which has been set to it's GameObject ID.
        RobotStation robotStation = new RobotStation();
        Transform robot_transform= GameEvents.current.GetRobotTransform();
        robotStation.position = robot_transform.position;
        robotStation.quaternion = robot_transform.rotation;
        robotStation.station_button_name= oneButton.transform.GetInstanceID().ToString();
 
        this.AllRobotStations.Add(robotStation);
    }
    private void ButtonClick(int gameObjectID)
    {
        int index = FindIndexOfAllRobotStationsByName(gameObjectID.ToString());
        if (index != -1) GameEvents.current.InitializeTrajectoryPlanningPanel(this.AllRobotStations[index].StationTrajectories);
    }
    private void DeleteStationButton(int gameObjectID)
    {
        Debug.Log("gameObjectID="+gameObjectID);
        //Destroy the button
        Destroy(GameObject.Find(gameObjectID.ToString()));
        StartCoroutine(InvokeUpdateCollection());//Rearrange the buttons' order
        //Delete the robot station data relating to the button
        int index = FindIndexOfAllRobotStationsByName(gameObjectID.ToString());
        if (index != -1) this.AllRobotStations.RemoveAt(index);
        this._scrooling_object.UpdateContent();
    }
    private int FindIndexOfAllRobotStationsByName(string station_button_name)
    {
        for (int i = 0; i < this.AllRobotStations.Count; i++)
        {
            if (this.AllRobotStations[i].station_button_name == station_button_name)
            {
                return i;
            }
        }
        return -1;
    }
    private void MoveRobotBase(int gameObjectID)
    {
        Debug.Log("gameObjectID="+gameObjectID);
        int index = FindIndexOfAllRobotStationsByName(gameObjectID.ToString());
        if (index!=-1) GameEvents.current.MoveRobotBase(this.AllRobotStations[index].position,this.AllRobotStations[index].quaternion);
    }
    private IEnumerator InvokeUpdateCollection()
    {
        yield return null;
        this._container.UpdateCollection();
    }

}
