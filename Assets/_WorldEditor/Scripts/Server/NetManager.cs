using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;

namespace DragginzWorldEditor
{
	public class NetManager : MonoSingleton<NetManager>
	{
		private readonly string scriptGetLevelList = "level_list.json";

		private Action _callback;

		public void loadLevelList(Action callback = null)
		{
			_callback = callback;
			StartCoroutine(GetData(Globals.urlLevelList + scriptGetLevelList));
		}

		IEnumerator GetData(string url)
		{
			UnityWebRequest www = UnityWebRequest.Get(url);
			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
				AppController.Instance.showPopup (PopupMode.Notification, "ERROR", www.error);
			}
			else {
				// Show results as text
				if (_callback != null) {
					_callback.Invoke ();
				} else {
					Debug.Log (www.downloadHandler.text);
				}

				// Or retrieve results as binary data
				// byte[] results = www.downloadHandler.data;
			}
		}
	}
}