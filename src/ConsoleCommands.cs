﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using QModManager.Utility;

namespace ReinforcementHelper
{
    class ConsoleCommands : MonoBehaviour
    {
		public void Awake()
		{
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.Command);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000228A File Offset: 0x0000048A
		public void OnDestroy()
		{
			Placeholder.Awake();
			SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.Command);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000022A4 File Offset: 0x000004A4
		public void Command(Scene off, LoadSceneMode on)
		{
			DevConsole.RegisterConsoleCommand(this, "getscore", false, false);
			DevConsole.RegisterConsoleCommand(this, "getoutput", false, false);
			DevConsole.RegisterConsoleCommand(this, "isresponderon", false, false);
		}
		public void OnConsoleCommand_isresponderon()
		{
			bool isresponderon = new Responder().responderIsStarted;
			QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
										   "Responder is on: " + isresponderon,
										   null,
										   true);
		}
		public void OnConsoleCommand_getscore()
		{
			float current_score = Methods.GetScore();
            QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
                                           current_score.ToString(),
										   null,
										   true);
		}
		public void OnConsoleCommand_getoutput()
		{
            //Methods.RetrieveOutputs();
            string ImgString = Methods.ScreenToText();

			//string recent_output = ZMQServer.recent_data;
			QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Info,
										   ImgString,
										   null,
										   false);
		}
	}
	public class Placeholder : MonoBehaviour
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002C90 File Offset: 0x00000E90
		public static void Awake()
		{
			try
			{
				UnityEngine.Object.Destroy(Placeholder.DummyObject);
				Placeholder.DummyObject = new GameObject("DummyObject");
				UnityEngine.Object.DontDestroyOnLoad(Placeholder.DummyObject);
				Placeholder.DummyObject.AddComponent<ConsoleCommands>();
			}
			catch
			{
				Placeholder.DummyObject = new GameObject("DummyObject");
				UnityEngine.Object.DontDestroyOnLoad(Placeholder.DummyObject);
				Placeholder.DummyObject.AddComponent<ConsoleCommands>();
			}
		}

		// Token: 0x0400000A RID: 10
		public static GameObject DummyObject;
	}
}
