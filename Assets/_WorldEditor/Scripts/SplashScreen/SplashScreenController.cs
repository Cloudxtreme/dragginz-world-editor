//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using UnityEngine;
using UnityEngine.UI;

namespace DragginzWorldEditor
{
    public class SplashScreenController : MonoBehaviour
    {
		[SerializeField]
		private Text FileInfo;
        [SerializeField]
        private Button ButtonOnline;
		[SerializeField]
		private Button ButtonOffline;
        [SerializeField]
        private GameObject Spinner;

		void Awake() {

			FileInfo.text = Globals.version;
		}

        public void workOnline()
        {
			ButtonOnline.gameObject.SetActive(false);
			ButtonOffline.gameObject.SetActive(false);

            Spinner.SetActive(true);

            AttemptConnection();
        }

		public void workOffline()
		{
			AppController.Instance.editorIsInOfflineMode = true;

			ButtonOnline.gameObject.SetActive(false);
			ButtonOffline.gameObject.SetActive(false);

			LevelEditor.Instance.init ();
		}

		private void AttemptConnection()
        {
			NetManager.Instance.loadLevelList (ConnectionSuccess);

            StartCoroutine(TimerUtils.WaitAndPerform(5.0f, ConnectionTimeout));
        }

		private void ConnectionSuccess()
		{
			AppController.Instance.editorIsInOfflineMode = false;

			StopCoroutine(TimerUtils.WaitAndPerform(5.0f, ConnectionTimeout));

			Spinner.SetActive(false);
			LevelEditor.Instance.init ();
		}

        private void ConnectionTimeout()
        {
			StopCoroutine(TimerUtils.WaitAndPerform(5.0f, ConnectionTimeout));

			Spinner.SetActive(false);
			AppController.Instance.showPopup (PopupMode.Notification, "Warning", "Could not connect to Server!\n\nEditor will run in Offline Mode!", timeOutPopupContinue);
        }

		private void timeOutPopupContinue(int buttonId)
		{
			MainMenu.Instance.popup.hide();
			workOffline ();
		}
    }
}