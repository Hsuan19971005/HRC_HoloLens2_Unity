//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.HsuanMoveitCommand
{
    [Serializable]
    public class AddTwoIntsResponse : Message
    {
        public const string k_RosMessageName = "hsuan_moveit_command/AddTwoInts";
        public override string RosMessageName => k_RosMessageName;

        public long sum;

        public AddTwoIntsResponse()
        {
            this.sum = 0;
        }

        public AddTwoIntsResponse(long sum)
        {
            this.sum = sum;
        }

        public static AddTwoIntsResponse Deserialize(MessageDeserializer deserializer) => new AddTwoIntsResponse(deserializer);

        private AddTwoIntsResponse(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.sum);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.sum);
        }

        public override string ToString()
        {
            return "AddTwoIntsResponse: " +
            "\nsum: " + sum.ToString();
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
