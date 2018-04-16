//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using RTEditor;

namespace DragginzVoxelWorldEditor
{
	//
	public enum AppState {
		Null,
		Splash,
		Select,
		Dig,
		Paint,
		Build,
		Props,
		Play
	};

	//
	public enum PopupMode {
		Notification,
		Confirmation,
		Input
	};

	//
    public class AppController : MonoSingletonBase<AppController>
    {
		private bool _editorIsInOfflineMode;

        private AppState _appState;

		private float time;
		private float timeDelta;

		#region Getters

		public AppState appState {
			get { return _appState; }
		}

		public bool editorIsInOfflineMode {
			get { return _editorIsInOfflineMode; }
			set { _editorIsInOfflineMode = value; }
		}

		#endregion

		#region PublicMethods

        void Awake() {

			Application.targetFrameRate = Globals.TargetClientFramerate;

			_editorIsInOfflineMode = true;

			_appState = AppState.Null;
        }

		//
		void Start() {

			_appState = AppState.Splash;

			SceneManager.LoadScene(BuildSettings.VWE_SplashScreenScene, LoadSceneMode.Additive);
		}

		//
        void Update() {

			time = Time.realtimeSinceStartup;
			timeDelta = Time.deltaTime;

			if (_appState == AppState.Splash) {
				//
			} else if (_appState != AppState.Null) {
				LevelEditor.Instance.customUpdateCheckControls (time, timeDelta);
			}
        }

		//
		void LateUpdate()
		{
			if (_appState == AppState.Splash) {
				//
			} else {
				LevelEditor.Instance.customUpdate (time, timeDelta);
			}
		}

		//
		public void setAppState(AppState state) {
			
			if (_appState != state) {
				_appState = state;
			}
		}

		//
		public void showPopup(PopupMode mode, string header, string message, Action<int> callback = null) {

			if (MainMenu.Instance.popup != null) {
				MainMenu.Instance.popup.showPopup(mode, header, message, callback);
			}
		}

		//
		public void hidePopup() {

			if (MainMenu.Instance.popup != null) {
				MainMenu.Instance.popup.hide();
			}

		}

		#endregion
    }
}