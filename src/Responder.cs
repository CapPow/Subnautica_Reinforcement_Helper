using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;

using NetMQ;
using NetMQ.Sockets;

namespace ReinforcementHelper
{
	public class Responder : MonoBehaviour
	{
		public bool responderIsStarted = false;
		void Respond()
		{
			AsyncIO.ForceDotNet.Force();

			var socket = new ResponseSocket("tcp://*:5558");

			try
			{
				while (responderIsStarted)
				{
					string inMsg;
					if (!socket.TryReceiveFrameString(out inMsg))
					{
						continue;
					}
					//Debug.Log("Received: " + inMsg);
					QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
							   "Responder Recieved: " + inMsg,
							   null,
							   true);
					if (inMsg == "get_outputs")
					{
						{
							string outputs = Methods.RetrieveOutputs();
							socket.SendFrame(outputs);
						}
						
					}
					else if (inMsg == "somethingelse")
					{
						//do something else 
					}
				}
			}
			finally
			{
				if (socket != null)
				{
					socket.Close();
					((IDisposable)socket).Dispose();
					NetMQConfig.Cleanup(true);
				}
			}
		}

		// Use this for initialization
		public void Start()
		{
			responderIsStarted = true;
			Task task = new Task(async () => Respond());
			task.Start();
			QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
							  "Server Start() Called",
							  null,
							  true);
		}

		// Update is called once per frame
		void Update()
		{
			// could use this to modify the controls
			//if (lightIsOn)
			//{
		//		light.enabled = true;
		//	}
		//	else
		//	{
		//		light.enabled = false;
		//	}
		}

		void OnDestroy()
		{
			responderIsStarted = false;
		}
	}
}
