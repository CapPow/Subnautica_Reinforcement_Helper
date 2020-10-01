using System.Reflection;
using QModManager.API.ModLoading;
using HarmonyLib;
using UnityEngine;

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
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "ReinforcementHelper");
		}
		// Token: 0x06000003 RID: 3 RVA: 0x00002094 File Offset: 0x00000294
	}
	[HarmonyPatch(typeof(VFXSchoolFish))]
	[HarmonyPatch("Awake")]
	public class Patch_SchoolFish
	{
		private static bool Prefix(VFXSchoolFish __instance)
		{
			__instance.quantity = 0;
			__instance.randomQuantity = 0;
			return true;
		}
	}
		// Token: 0x06000004 RID: 4 RVA: 0x000020A9 File Offset: 0x000002A9
	class Destructor : MonoBehaviour
	{
			// Token: 0x06000006 RID: 6 RVA: 0x000020DB File Offset: 0x000002DB
			public void Start()
			{
				UnityEngine.Object.Destroy(base.GetComponent<VFXSchoolFish>());
			}
	}
}