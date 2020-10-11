using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

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
                //server.Bind("tcp://localhost:12346");
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
            //InvokeRepeating("SlowUpdate", 5.0f, 0.1f);
            QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
                              "Server Start() Called thread is alive: " + _listenerWorker.IsAlive,
                              null,
                              true);
        }
        private void SlowUpdate()
        {

        }
        //private void Update()
        // {
        // }
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

        public void Start()
        {
            _netMqPublisher = new NetMqPublisher(HandleMessage);
            _netMqPublisher.Start();
        }

        private void Update()
        {
            Connected = _netMqPublisher.Connected;
            _response = Methods.GetOutputs();
        }

        private string HandleMessage(string message)
        {
            // Not on main thread
            if (message == "get_outputs")
            {
                return _response;
            }
            if (message.Contains("warp"))
            {
                string[] pos = message.Split('|');
                Methods.WarpTo(pos[1]);
                return "warping";
            }
            if (message == "resetview")
            {
                Methods.ResetPlayerView();
                return "resetting playerview";

            }
            else return "not a proper request";
        }

        private void OnDestroy()
        {
            _netMqPublisher.Stop();
        }
    }
}

