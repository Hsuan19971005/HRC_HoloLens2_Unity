using System;
using System.Collections;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.HsuanMoveitCommand;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class AddTwoInt : MonoBehaviour
{
    string ServiceName = "add_two";
    ROSConnection m_Ros;

    // Start is called before the first frame update
    void Start()
    {
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterRosService<AddTwoIntsRequest, AddTwoIntsResponse>(ServiceName);
    }
    public void Publish()
    {
        var request = new AddTwoIntsRequest();
        request.a = 12;
        request.b = 21;
        m_Ros.SendServiceMessage<AddTwoIntsResponse>(ServiceName, request, ResponseFunction);

    }
    void ResponseFunction(AddTwoIntsResponse response)
    {
        Debug.Log("sum=" + response.sum);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Publish();
        }
    }
}
