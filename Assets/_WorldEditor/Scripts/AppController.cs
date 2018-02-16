//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using RTEditor;

namespace DragginzWorldEditor
{
	//
	public enum AppState {
		Null,
		Look,
		Dig,
		Paint,
		Build,
		Props,
		Play
	};

	//
	public enum PopupMode {
		Notification,
		Confirmation
	};

	//
	public struct levelProp {
		public string name;
		public GameObject prefab;
		public Vector3 forward;
		public bool useGravity;
	};

	//
    public class AppController : MonoSingletonBase<AppController>
    {
        private AppState _appState;

		private float time;
		private float timeDelta;

		#region Getters

		public AppState appState {
			get { return _appState; }
		}

		#endregion

		#region PublicMethods

        void Awake() {

			_appState = AppState.Null;
        }

		//
        void Update() {

			time = Time.realtimeSinceStartup;
			timeDelta = Time.deltaTime;

			LevelEditor.Instance.customUpdateCheckControls (time, timeDelta);

            /*if (Input.GetKeyDown(KeyCode.B)) {
                LevelEditor.Instance.toggleCubes();
            }
            else {*/
            /*    else if (_appState == AppState.Play) {
                    if (Input.GetKeyDown(KeyCode.Escape)) {
                        //setAppState(AppState.Dig);
                    }
                    else if (Input.GetKeyDown(KeyCode.X)) {
                        LevelEditor.Instance.toggleFlyCamOffset();
                    }
                }
            }*/
        }

		//
		void LateUpdate()
		{
			LevelEditor.Instance.customUpdate (time, timeDelta);
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

		#region PrivateMethods

		//
		private void popupCallback(int buttonId) {

			MainMenu.Instance.popup.hide();
		}

		#endregion
    }
}