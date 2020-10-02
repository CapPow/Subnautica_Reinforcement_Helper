using System;
using System.Collections;
using UnityEngine;
using UWE;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using QModManager.Utility;
using System.Linq;

namespace ReinforcementHelper
{
	class Methods : MonoBehaviour
	{
		private static float screenArea = (float)(Screen.height * Screen.width);

		// see the answer by "tall-josh"
		// https://answers.unity.com/questions/8003/how-can-i-know-if-a-gameobject-is-seen-by-a-partic.html
		private static bool IsInView(Creature toCheck)
		{
			Bounds rendbounds = toCheck.GetComponentInChildren<Renderer>().bounds;
			Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(toCheck.GetComponentInChildren<Renderer>().bounds.center);
			//Is in front
			if (pointOnScreen.z < 0)
			{
				//Debug.Log("Behind: " + toCheck.name);
				return false;
			}

			//Is in FOV
			if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
					(pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
			{
				//Debug.Log("OutOfBounds: " + toCheck.name);
				return false;
			}

			RaycastHit hit;
			Vector3 heading = toCheck.transform.position - Camera.main.transform.position;
			Vector3 direction = heading.normalized;// / heading.magnitude;
												   // Debug.Log("MainCam Transform position = " + Camera.main.transform.position);
												   // modified the linecast to start slightly ahead of player (given occluded by player is so common)
			if (Physics.Linecast(Camera.main.transform.position + (Camera.main.transform.forward / 2), toCheck.GetComponentInChildren<Renderer>().bounds.center, out hit))
			{
				if (hit.transform.name != toCheck.name)
				{
					Debug.Log(toCheck.name + " occluded by " + hit.transform.name);
					return false;
				}
			}
			Debug.Log(toCheck.name + " Was a success! ");
			return true;
		}

		public static float GetScore()
		{
			float score = 0f;
			float screenArea = (float)(Screen.height * Screen.width);

			// motivate multiple targets being in frame
			float perTargetModifier = 0.02f;
			foreach (Creature creature in GameObject.FindObjectsOfType<Creature>().Distinct<Creature>())
			{
				string name = creature.name;
				if (creature != null && creature.isActiveAndEnabled
					&& IsInView(creature)
					&& !creature.name.Contains("Skyray")
					&& !creature.name.Contains("School"))
				{
					float distance = 0f;
					Vector3 a = creature.transform.position - Player.mainObject.transform.position;
					distance = a.magnitude;
					Vector3 rhs = Player.mainObject.transform.forward;
					Vector3 lhs = a / distance;

					// non-working example code I found in the subnautica data to check for visibility.
					//if ((Mathf.Approximately(Camera.main.fieldOfView, -1f) || Vector3.Dot(lhs, rhs) >= Camera.main.fieldOfView) && !Physics.Linecast(Player.mainObject.transform.position, creature.transform.position, Voxeland.GetTerrainLayerMask()))
					// non-working bounds calculation (but looked promising)
					//Bounds bounds = UWE.Utils.GetEncapsulatedAABB(creature.gameObject);
					Bounds bounds = creature.GetComponentInChildren<Renderer>().bounds;
					Vector3 center = bounds.center;
					Vector3 extents = bounds.extents;
					Vector2[] array3 = new Vector2[]
					{
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z)),
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z)),
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z)),
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z)),
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z)),
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z)),
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z)),
						PerformanceConsoleCommands.WorldToGUIPoint(new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z))
					};
					Vector2 vector = array3[0];
					Vector2 vector2 = array3[0];
					foreach (Vector2 rhs1 in array3)
					{
						vector = Vector2.Min(vector, rhs1);
						vector2 = Vector2.Max(vector2, rhs1);
					}
					// Although IsInView should take care of this, there are occasional wonky high values
					if (vector.x > 0f && vector.y > 0f && vector2.x < (float)Screen.width && vector2.y < (float)Screen.height)
					{
						Rect rect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
						float this_item_score = (rect.height * rect.width) / screenArea;
						if (this_item_score > 0.02)
						{
							//reward multiple targets in frame
							score += this_item_score + perTargetModifier;
						}
					}
				}
			}
			// return the score
			return score;
		}
		// holds up a process until ImageLoaded is true
		public static void WarpTo(string pos)
		{
			string[] coords = pos.Split(',');
			// check if the input looks correct
			if (coords.Length == 3)
			{
				float x = float.Parse(coords[0]);
				float y = float.Parse(coords[1]);
				float z = float.Parse(coords[2]);
				Player player = Utils.GetLocalPlayer().GetComponent<Player>();
				player.SetPosition(new Vector3(x, y, z));
				player.OnPlayerPositionCheat();
			}
		}
		public static string GetOutputs()
		{
			float score = GetScore();
			string pos = GetPlayerCoords();
			string output = score + "|" + pos;
			return output;
		}
		public static void ResetPlayerView()
		{
			MainCameraControl.main.ResetCamera();
		}
		public static string GetPlayerCoords()
		{
			//Player player = Utils.GetLocalPlayer();
			Vector3 pos = Utils.GetLocalPlayerPos();
			float posx = pos.x;
			float posy = pos.y;
			float posz = pos.z;
			string posstring = string.Join("|", pos.x, pos.y, pos.z);
			return posstring;
		}
	}
}
