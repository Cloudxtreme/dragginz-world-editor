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
	public enum AppState {
		Null,
		Look,
		Dig,
		Paint,
		Build,
		Play
	};

	public enum PopupMode {
		Notification,
		Confirmation
	};

	//
    public class AppController : MonoSingletonBase<AppController>
    {
        private AppState _appState;

        //private GameObject _goLightsContainer;
        private GameObject _goWorldContainer;

        private MainMenu _mainMenu;
        private Popup _popup;

		#region Getters

		public AppState appState {
			get { return _appState; }
		}

		public GameObject goWorldContainer {
			get { return _goWorldContainer; }
		}

		#endregion

		#region PublicMethods

        void Awake() {

			_appState = AppState.Null;
        }

        /*
        // called first
        void OnEnable() {
            Debug.Log("OnEnable called");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(mode);
        }

        // called third
        void Start() {
            Debug.Log("Start");
        }

        // called when the game is terminated
        void OnDisable() {
            Debug.Log("OnDisable");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        */

		//
        void Update() {

			LevelEditor.Instance.customUpdateCheckControls ();

            if (Input.GetKeyDown(KeyCode.B)) {
                LevelEditor.Instance.toggleCubes();
            }
            else {
				if (_appState == AppState.Dig || _appState == AppState.Paint || _appState == AppState.Build) {
                    if (Input.GetKeyDown(KeyCode.Escape)) {
                        LevelEditor.Instance.resetFlyCam();
                    }
                }
                else if (_appState == AppState.Play) {
                    if (Input.GetKeyDown(KeyCode.Escape)) {
                        //setAppState(AppState.Dig);
                    }
                    else if (Input.GetKeyDown(KeyCode.X)) {
                        LevelEditor.Instance.toggleFlyCamOffset();
                    }
                }
            }
        }

		//
		void LateUpdate()
		{
			if (_appState == AppState.Dig) {
				LevelEditor.Instance.customUpdateDig ();
			} else if (_appState == AppState.Paint) {
				LevelEditor.Instance.customUpdatePaint ();
			} else if (_appState == AppState.Build) {
				LevelEditor.Instance.customUpdateBuild ();
			}
		}

		//
		public void setAppState(AppState state) {
			
			if (_appState != state) {
				_appState = state;
				/*if (_appState == AppState.Dig) {
				}
				else if (_appState == AppState.Paint) {
				}
				else if (_appState == AppState.Build) {
				}
				else if (_appState == AppState.Play) {
				}*/
			}
		}

        /// <summary>
        /// ...
        /// </summary>
		public void showPopup(PopupMode mode, string header, string message, Action<int> callback = null) {

			if (MainMenu.Instance.popup != null) {
				MainMenu.Instance.popup.showPopup(mode, header, message, callback);
			}

		}

		public void hidePopup() {

			if (MainMenu.Instance.popup != null) {
				MainMenu.Instance.popup.hide();
			}

		}

		#endregion

		#region PrivateMethods

		/// <summary>
		/// ...
		/// </summary>
		private void popupCallback(int buttonId) {

			MainMenu.Instance.popup.hide();
		}

		#endregion
    }
}