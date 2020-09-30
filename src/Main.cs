//using System;
using System.Reflection;
//using System.Collections;
using QModManager.API.ModLoading;
//using HarmonyLib;
//using UnityEngine;
//using System.Diagnostics;
namespace ReinforcementHelper
{
	// Your main patching class must have the QModCore attribute (and must be public)
	[QModCore]
	public class Main
	{
		// Your patching method must have the QModPatch attribute (and must be public)
		[QModPatch]
		public static void Load()
		{
			Placeholder.Awake();
		}
	}
}