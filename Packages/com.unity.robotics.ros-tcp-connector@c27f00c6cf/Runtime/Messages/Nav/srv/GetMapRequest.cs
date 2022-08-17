//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Nav
{
    [Serializable]
    public class GetMapRequest : Message
    {
        public const string k_RosMessageName = "nav_msgs/GetMap";
        public override string RosMessageName => k_RosMessageName;

        //  Get the map as a nav_msgs/OccupancyGrid

        public GetMapRequest()
        {
        }
        public static GetMapRequest Deserialize(MessageDeserializer deserializer) => new GetMapRequest(deserializer);

        private GetMapRequest(MessageDeserializer deserializer)
        {
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
        }

        public override string ToString()
        {
            return "GetMapRequest: ";
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
