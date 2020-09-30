﻿using System.Diagnostics;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

namespace ReinforcementHelper
{
    public class NetMqPublisher : MonoBehaviour
    // based on https://github.com/valkjsaaa/Unity-ZeroMQ-Example/blob/master/Assets/ServerObject.cs

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
                server.Bind("tcp://*:55555");

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
        private string _response;

        private void Start()
        {
            _netMqPublisher = new NetMqPublisher(HandleMessage);
            _netMqPublisher.Start();
            QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
                                          "Server Start() Called",
                                          null,
                                          true);
        }

        private void Update()
        {
            var position = transform.position;
            _response = $"{position.x} {position.y} {position.z}";
            Connected = _netMqPublisher.Connected;
        }

        private string HandleMessage(string message)
        {
            if (message == "get_outputs")
            {
                Methods.RetrieveOutputs();
                string outputs = Methods.RetrieveOutputs();
                return outputs;
            }
            // Not on main thread
            else
            {
                return null;
            }
        }

        private void OnDestroy()
        {
            _netMqPublisher.Stop();
        }
    }
}