using UnityEngine;
using System;
using Component = UnityEngine.Component;

namespace GAME
{
	public class Tools : MonoBehaviour
	{
		public static T GetDataFromPrefs<T>(string key)
		{
			string json = PlayerPrefs.GetString(key, "");
			return JsonToObject<T>(json);
		}

		public static void SetDataToPrefs<T>(string key, T data)
		{
			string json = ObjectToJson(data);
			PlayerPrefs.SetString(key, json);
		}

		public static T JsonToObject<T>(string json)
		{
			try
			{
				return JsonUtility.FromJson<T>(json);
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public static string ObjectToJson<T>(T data)
		{
			return JsonUtility.ToJson(data);
		}

		public static T AddObject<T>(Transform parent) where T : Component
		{
			return AddObject<T>(null, parent, false);
		}

		public static T AddObject<T>(object obj, Transform parent, bool active = false) where T : Component
		{
			GameObject itemObj = null;

			if (obj == null)
			{
				itemObj = new GameObject();
				itemObj.AddComponent<T>();
			}
			else
			{
				if (obj.GetType() == typeof(string))
				{
					T prefObj = Resources.Load<T>(obj.ToString());
					itemObj = AddObject(prefObj.gameObject, parent);
				}
				else
				{
					itemObj = AddObject(((T) obj).gameObject, parent);
					if (itemObj.GetComponent<T>() == null)
					{
						itemObj.AddComponent<T>();
					}
				}
			}

			itemObj.transform.SetParent(parent);

			SetZero(itemObj);

			if (active)
				itemObj.gameObject.SetActive(true);

			return itemObj.GetComponent<T>();
		}

		public static GameObject AddObject(Transform parent)
		{
			GameObject obj = new GameObject();
			obj.transform.SetParent(parent);
			SetZero(obj);
			return obj;
		}

		public static GameObject AddObject(string objName, Transform parent, bool active = false)
		{
			GameObject obj = Resources.Load<GameObject>(objName);
			return AddObject(obj, parent, active);
		}

		public static GameObject AddObject(GameObject obj, Transform parent, bool active = false)
		{

			GameObject itemObj = (GameObject) Instantiate(obj);
			itemObj.transform.SetParent(parent);
			SetZero(itemObj);

			if (active)
				itemObj.gameObject.SetActive(true);

			return itemObj;
		}


		private static void SetZero(GameObject obj)
		{
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;

			RectTransform rt = obj.GetComponent<RectTransform>();
			if (rt != null)
			{
				rt.anchoredPosition3D = Vector3.zero;
			}
		}

		public static void RemoveObjects(Transform root, bool immediate = false)
		{
			if (root.childCount == 0) return;
			
			int counter = immediate ? 10 : 1;
			for (int i = 0; i < counter; i++)
			{
				foreach (Transform child in root)
				{
					child.gameObject.SetActive(false);
					if (immediate)
						DestroyImmediate(child.gameObject);
					else
						Destroy(child.gameObject);
				}
			}
		}

		public static T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			var type = original.GetType();
			var dst = destination.GetComponent(type) as T;
			if (!dst) dst = destination.AddComponent(type) as T;

			var fields = type.GetFields();
			foreach (var field in fields)
			{
				if (field.IsStatic) continue;
				field.SetValue(dst, field.GetValue(original));
			}

			var props = type.GetProperties();
			foreach (var prop in props)
			{
				if (!prop.CanWrite || prop.Name == "name") continue;
				prop.SetValue(dst, prop.GetValue(original, null), null);
			}

			return dst;
		}

		public static string[] ParseStr(string data, string split)
		{
			if (data == null)
				data = "";
			string[] array = data.Split(new string[] {split}, StringSplitOptions.None);
			return array;
		}

		public static Transform FindChild(Transform obj, string childName)
		{
			Transform[] childs = obj.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in childs)
			{
				if (child.name == childName)
					return child;
			}

			return null;
		}

		public static Transform ContainsChild(Transform obj, string childName)
		{
			Transform[] childs = obj.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in childs)
			{
				if (child.name.Contains(childName))
					return child;
			}

			return null;
		}

	}
}


