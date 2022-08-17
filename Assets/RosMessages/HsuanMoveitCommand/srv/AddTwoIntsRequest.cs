//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.HsuanMoveitCommand
{
    [Serializable]
    public class AddTwoIntsRequest : Message
    {
        public const string k_RosMessageName = "hsuan_moveit_command/AddTwoInts";
        public override string RosMessageName => k_RosMessageName;

        public long a;
        public long b;

        public AddTwoIntsRequest()
        {
            this.a = 0;
            this.b = 0;
        }

        public AddTwoIntsRequest(long a, long b)
        {
            this.a = a;
            this.b = b;
        }

        public static AddTwoIntsRequest Deserialize(MessageDeserializer deserializer) => new AddTwoIntsRequest(deserializer);

        private AddTwoIntsRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.a);
            deserializer.Read(out this.b);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.a);
            serializer.Write(this.b);
        }

        public override string ToString()
        {
            return "AddTwoIntsRequest: " +
            "\na: " + a.ToString() +
            "\nb: " + b.ToString();
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