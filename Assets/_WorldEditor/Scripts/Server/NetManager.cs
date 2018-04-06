using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;

using AssetsShared;

namespace DragginzWorldEditor
{
	public class NetManager : MonoSingleton<NetManager>
	{
		private readonly string scriptGetLevelList = "level_list.json";

		private Action<string> _callbackSuccess;
		private Action<string> _callbackFail;

		//
		public void loadLevelList(Action<string> callbackSuccess = null, Action<string> callbackFail = null)
		{
			_callbackSuccess = callbackSuccess;
			_callbackFail = callbackFail;

			StartCoroutine(GetData(Globals.urlLevelList + scriptGetLevelList));
		}

		//
		public void loadLevelChunk(string filename, Action<string> callbackSuccess = null, Action<string> callbackFail = null)
		{
			_callbackSuccess = callbackSuccess;
			_callbackFail = callbackFail;

			StartCoroutine(GetData(Globals.urlLevelList + filename));
		}

		//
		IEnumerator GetData(string url)
		{
			Debug.Log ("loading "+url);

			UnityWebRequest www = UnityWebRequest.Get(url);
			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError)
			{
				if (_callbackFail != null) {
					_callbackFail.Invoke (www.error);
				} else {
					Debug.Log(www.error);
				}
			}
			else
			{
				if (_callbackSuccess != null) {
					_callbackSuccess.Invoke (www.downloadHandler.text);
				} else {
					 Debug.Log (www.downloadHandler.text);
				}
			}
		}
	}
}