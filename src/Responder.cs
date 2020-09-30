using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;
using System.Threading;

namespace ReinforcementHelper
{
	public class NetMqPublisher : MonoBehaviour
	{
		private readonly Thread _listenerWorker;

		private bool _listenerCancelled;

		public delegate string MessageDelegate(string message);

		private readonly MessageDelegate _messageDelegate;

		private readonly Stopwatch _contactWatch;

		private const long ContactThreshold = 1000;

		public bool Connected;

		private void ListenerWork()
		{
			AsyncIO.ForceDotNet.Force();
			using (var server = new ResponseSocket())
			{
				server.Bind("tcp://*:12346");
				while (!_listenerCancelled)
				{
					Connected = _contactWatch.ElapsedMilliseconds < ContactThreshold;
					string message;
					if (!server.TryReceiveFrameString(out message)) continue;
					_contactWatch.Restart();
					var response = _messageDelegate(message);
					server.SendFrame(response);
				}
			}
			NetMQConfig.Cleanup();
		}

		public NetMqPublisher(MessageDelegate messageDelegate)
		{
			_messageDelegate = messageDelegate;
			_contactWatch = new Stopwatch();
			_contactWatch.Start();
			_listenerWorker = new Thread(ListenerWork);
		}

		public void Start()
		{
			_listenerCancelled = false;
			_listenerWorker.Start();
			//InvokeRepeating("SlowUpdate", 5.0f, 0.05f);
			QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
							  "Server Start() Called thread is alive: " + _listenerWorker.IsAlive,
							  null,
							  true);
		}
		private void SlowUpdate()
        {
			//if (!_listenerWorker.IsAlive)
			//	{
			//		this.Start();
			//	}
		}
		private void Update()
        {
		
		}
		public void Stop()
		{
			_listenerCancelled = true;
			_listenerWorker.Join();
		}
	}

	public class ServerObject : MonoBehaviour
	{
			public bool Connected;
			private NetMqPublisher _netMqPublisher;
			private string _response = "";

			private void Start()
			{
				_netMqPublisher = new NetMqPublisher(HandleMessage);
				_netMqPublisher.Start();
				//update the response message less frequently than every single frame
				InvokeRepeating("SlowUpdate", 5.0f, 0.05f);
			}

			private void Update()
			{
				Connected = _netMqPublisher.Connected;
			}
			private void SlowUpdate()
			{
				_response = Methods.RetrieveOutputs();
			}

			private string HandleMessage(string message)
			{
				// Not on main thread
			Connected = _netMqPublisher.Connected;
			if (message == "get_outputs")
			{
				return _response;
			}
			else return "not a proper request";
			
		}

			private void OnDestroy()
			{
				_netMqPublisher.Stop();
			}
		}
	}

