using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using CameraPlus.Behaviours;

namespace CameraPlus.VMCProtocol
{
    public class ExternalSender : MonoBehaviour
    {
        internal List<SendTask> sendTasks = new List<SendTask>();
        private Vector3 position = new Vector3();
        private Quaternion rotation = new Quaternion();

        internal class SendTask
        {
            internal CameraPlusBehaviour parentBehaviour = null;
            internal OscClient client = null;
        }

        internal void AddSendTask(CameraPlusBehaviour camplus,string address = "127.0.0.1", int port = 39540)
        {
            SendTask sendTask = new SendTask();
            sendTask.parentBehaviour = camplus;
            sendTask.client = new OscClient(address, port);
            if (sendTask.client != null)
            {
                Logger.log.Notice($"Instance of OscClient {address}:{port} Starting.");
                sendTasks.Add(sendTask);
            }
            else
                Logger.log.Error($"Instance of OscClient Not Starting.");
        }

        internal void RemoveTask(CameraPlusBehaviour camplus)
        {
            foreach(SendTask sendTask in sendTasks)
            {
                if (sendTask.parentBehaviour.name == camplus.name)
                {
                    sendTasks.Remove(sendTask);
                    break;
                }
            }
        }

        private async Task SendData()
        {
            await Task.Run(() => {
                try
                {
                    foreach(SendTask sendTask in sendTasks)
                    {
                        position = sendTask.parentBehaviour.ThirdPersonPos;
                        rotation = Quaternion.Euler(sendTask.parentBehaviour.ThirdPersonRot);

                        sendTask.client.Send("/VMC/Ext/Cam", "Camera", new float[] {
                            position.x, position.y, position.z,
                            rotation.x, rotation.y, rotation.z, rotation.w,
                            sendTask.parentBehaviour.FOV});
                    }
                }
                catch (Exception e)
                {
                    Logger.log.Error($"ExternalSender Thread : {e}");
                }
            });
        }

        private void Update()
        {
         　Task.Run(() => SendData());
        }
    }
}
