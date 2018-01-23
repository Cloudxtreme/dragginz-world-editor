//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using UnityEngine.SceneManagement;

using RTEditor;

namespace DragginzWorldEditor
{
	/// <summary>
	/// ...
	/// </summary>
    public class AppController : MonoSingletonBase<AppController>
    {
        public enum AppState {
            Edit,
            Play
        };
        private AppState _appState;

        //private GameObject _goLightsContainer;
        private GameObject _goWorldContainer;

        private MainMenu _mainMenu;
        private Popup _popup;

		#region Getters

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

            _appState = AppState.Edit;
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

			if (_appState == AppState.Edit) {
				if (Input.GetKeyDown (KeyCode.Escape)) {
					LevelEditor.Instance.resetFlyCam ();
				}
			}
            else if (_appState == AppState.Play) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    switchToEditMode();
                }
            }
        }

        /// <summary>
        /// ...
        /// </summary>
        public void switchToPlayMode() {
            if (_appState != AppState.Play) {
                _appState = AppState.Play;
                SceneManager.LoadScene(1);
            }
        }

        public void switchToEditMode() {
            if (_appState != AppState.Edit) {
                _appState = AppState.Edit;
                SceneManager.LoadScene(0);
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