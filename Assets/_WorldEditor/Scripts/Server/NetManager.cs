using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace DragginzWorldEditor
{
	public class NetManager : MonoSingleton<NetManager>
	{
		private readonly string scriptGetLevelList = "level_list.json";

		void Start()
		{
			StartCoroutine(GetText());
		}

		IEnumerator GetText()
		{
			UnityWebRequest www = UnityWebRequest.Get(Globals.urlLevelList + scriptGetLevelList);
			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
				AppController.Instance.showPopup (PopupMode.Notification, "ERROR", www.error);
			}
			else {
				// Show results as text
				Debug.Log(www.downloadHandler.text);

				// Or retrieve results as binary data
				// byte[] results = www.downloadHandler.data;
			}
		}
	}
}