using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace ReinforcementHelper
{
	class Methods
	{
		public static float GetScore()
		{
			float score = 0f;
			float screenArea = (float)(Screen.height * Screen.width);
			foreach (GameObject gameObject in UnityEngine.Object.FindSceneObjectsOfType(typeof(GameObject)))
			{
				Collider collider = gameObject.GetComponent<Collider>();
				if (collider == null)
				{
					Collider[] componentsInParent = gameObject.GetComponentsInParent<Collider>();
					if (componentsInParent.Length != 0)
					{
						foreach (Collider collider2 in componentsInParent)
						{
							if (collider2.enabled)
							{
								collider = collider2;
								break;
							}
						}
					}
				}
				string name = gameObject.name;
				if (collider != null && gameObject.tag == "Creature" && !gameObject.name.Contains("Skyray"))
				{
					Vector3 center = collider.bounds.center;
					Vector3 extents = collider.bounds.extents;
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
					foreach (Vector2 rhs in array3)
					{
						vector = Vector2.Min(vector, rhs);
						vector2 = Vector2.Max(vector2, rhs);
					}
					if (vector.x > 0f && vector.y > 0f && vector2.x < (float)Screen.width && vector2.y < (float)Screen.height)
					{
						Rect rect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
						float this_item_score = rect.height * rect.width / screenArea;
						if ((double)this_item_score > 0.0025)
						{
							score += this_item_score;
						}
					}
				}
			}
			// return the score
			// score
			// encode ss
			return score;
		}
		public static string RetrieveOutputs()
        {
			float score = GetScore();
			string ImgString = ScreenToText();
			string output = string.Concat(new object[]
				{
							score,
							"|",
							ImgString
				});
			return output;

		}
		internal static ScreenCapture Capture;
		public static string ScreenToText()
		{
			if (Capture == null)
			{
				Capture = Camera.main.gameObject.AddComponent<ScreenCapture>();
			}
			Capture.SaveScreenshot();
			// if you wish to save:
			//System.IO.File.WriteAllBytes("filename.png", Capture.Image);
			return Capture.Image;
		}
		}
	internal class ScreenCapture : MonoBehaviour
	{
		internal string Image = null;

		internal void SaveScreenshot()
		{
			StartCoroutine(SaveScreenshot_ReadPixelsAsynch());
		}

		private IEnumerator SaveScreenshot_ReadPixelsAsynch()
		{
			//Wait for graphics to render
			yield return new WaitForEndOfFrame();

			//Create a texture to pass to encoding
			Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

			//Put buffer into texture
			texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

			//Split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
			yield return 0;

			byte[] bytes = texture.EncodeToJPG();

			String AsBase64String = Convert.ToBase64String(bytes);
			Image = AsBase64String;

			//Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
			DestroyObject(texture);
		}
	}
}
