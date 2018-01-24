//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using UnityEngine.SceneManagement;

using RTEditor;

namespace DragginzWorldEditor
{
	//
	public enum AppState {
		Dig,
		Paint,
		Play
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

		/// <summary>
		/// ...
		/// </summary>
        void Awake() {

            /*GameObject lightsPrefab = Resources.Load<GameObject>("Prefabs/" + Globals.lightsContainerName);
            if (lightsPrefab != null) {
                _goLightsContainer = GameObject.Instantiate(lightsPrefab);
                _goLightsContainer.name = Globals.lightsContainerName;
                _goLightsContainer.transform.SetParent(transform);
            }

            GameObject worldPrefab = Resources.Load<GameObject>("Prefabs/" + Globals.worldContainerName);
            if (worldPrefab != null) {
                _goWorldContainer = GameObject.Instantiate(worldPrefab);
                _goWorldContainer.name = Globals.worldContainerName;
                _goWorldContainer.transform.SetParent(transform);
            }

            GameObject goMainMenu = GameObject.Find(Globals.mainMenuContainerName);
            if (goMainMenu) {
                _mainMenu = goMainMenu.GetComponent<MainMenu>();
                if (_mainMenu) {
                    _popup = _mainMenu.popup;
                }
            }*/

			_appState = AppState.Dig;
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

        /// <summary>
        /// ...
        /// </summary>
        void Update() {

			if (_appState == AppState.Dig) {
				if (Input.GetKeyDown (KeyCode.Escape)) {
					LevelEditor.Instance.resetFlyCam ();
				} else if (Input.GetKeyDown (KeyCode.X)) {
					LevelEditor.Instance.toggleFlyCamOffset ();
				}
			}
            else if (_appState == AppState.Play) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
					setAppState(AppState.Dig);
                }
            }
        }

		//
		public void setAppState(AppState state) {
			
			if (_appState != state) {
				_appState = state;
				if (_appState == AppState.Dig) {
					//SceneManager.LoadScene(0);
				}
				else if (_appState == AppState.Paint) {
					//
				}
				else if (_appState == AppState.Play) {
					//SceneManager.LoadScene(1);
				}
			}
		}

        /// <summary>
        /// ...
        /// </summary>
        public void showWarning(string message) {

			if (MainMenu.Instance.popup != null) {
				MainMenu.Instance.popup.showPopup(Popup.PopupMode.Notification, "Warning", message, popupCallback);
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