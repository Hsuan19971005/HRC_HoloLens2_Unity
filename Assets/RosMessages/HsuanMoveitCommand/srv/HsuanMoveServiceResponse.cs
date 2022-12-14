//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.HsuanMoveitCommand
{
    [Serializable]
    public class HsuanMoveServiceResponse : Message
    {
        public const string k_RosMessageName = "hsuan_moveit_command/HsuanMoveService";
        public override string RosMessageName => k_RosMessageName;

        public Trajectory.JointTrajectoryMsg trajectories;

        public HsuanMoveServiceResponse()
        {
            this.trajectories = new Trajectory.JointTrajectoryMsg();
        }

        public HsuanMoveServiceResponse(Trajectory.JointTrajectoryMsg trajectories)
        {
            this.trajectories = trajectories;
        }

        public static HsuanMoveServiceResponse Deserialize(MessageDeserializer deserializer) => new HsuanMoveServiceResponse(deserializer);

        private HsuanMoveServiceResponse(MessageDeserializer deserializer)
        {
            this.trajectories = Trajectory.JointTrajectoryMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.trajectories);
        }

        public override string ToString()
        {
            return "HsuanMoveServiceResponse: " +
            "\ntrajectories: " + trajectories.ToString();
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
