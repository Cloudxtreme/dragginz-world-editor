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
            // Disable buttons
			ButtonOnline.interactable = false;
			ButtonOffline.interactable = false;

            // Start loading spinner
            Spinner.SetActive(true);

            AttemptConnection();
        }

		public void workOffline()
		{
			// Disable buttons
			ButtonOnline.interactable = false;
			ButtonOffline.interactable = false;
		}

		private void AttemptConnection()
        {
            /*Bootstrap bootstrap = FindObjectOfType<Bootstrap>();
            if (!bootstrap)
            {
                throw new Exception("Couldn't find Bootstrap script on GameEntry in UnityScene");
            }
            bootstrap.ConnectToClient();

            // In case the client connection is successful this coroutine is destroyed as part of unloading
            // the splash screen so ConnectionTimeout won't be called
            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.ClientConnectionTimeoutSecs, ConnectionTimeout));*/
        }

        private void ConnectionTimeout()
        {
            Spinner.SetActive(false);
        }
    }
}