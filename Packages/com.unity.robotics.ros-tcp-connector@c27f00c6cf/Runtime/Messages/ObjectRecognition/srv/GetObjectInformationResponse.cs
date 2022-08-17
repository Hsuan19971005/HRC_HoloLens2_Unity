//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.ObjectRecognition
{
    [Serializable]
    public class GetObjectInformationResponse : Message
    {
        public const string k_RosMessageName = "object_recognition_msgs-master/GetObjectInformation";
        public override string RosMessageName => k_RosMessageName;

        //  Extra object info 
        public ObjectInformationMsg information;

        public GetObjectInformationResponse()
        {
            this.information = new ObjectInformationMsg();
        }

        public GetObjectInformationResponse(ObjectInformationMsg information)
        {
            this.information = information;
        }

        public static GetObjectInformationResponse Deserialize(MessageDeserializer deserializer) => new GetObjectInformationResponse(deserializer);

        private GetObjectInformationResponse(MessageDeserializer deserializer)
        {
            this.information = ObjectInformationMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.information);
        }

        public override string ToString()
        {
            return "GetObjectInformationResponse: " +
            "\ninformation: " + information.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Response);
        }
    }
}
