//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;

using UnityEngine;
using UnityEngine.UI;

using RTEditor;

namespace DragginzVoxelWorldEditor
{
    public class Popup : MonoBehaviour
    {
        public GameObject blocker;
        public Text txtHeader;
        public Text txtMessage;

		public Text txtInputCaption;
		public Text txtInputPlaceHolder;
		public InputField inputField;

        public GameObject btnOkay;
        public GameObject btnYes;
        public GameObject btnNo;

        private Action<int> _callback;

		#region Getters

		public string inputText {
			get { return inputField.text; }
		}

		#endregion

        //
        // System methods
        //
        void Awake() {

        }

        //
        // Public methods
        //
        public void showPopup(PopupMode mode, string header, string message, Action<int> callback = null) {

			//EditorGizmoSystem.Instance.TurnOffGizmos ();

            if (txtHeader) {
                txtHeader.text = header;
            }
            if (txtMessage) {
                txtMessage.text = message;
            }

			txtInputCaption.text = "";
			inputField.gameObject.SetActive(false);

            showButton(btnOkay, false);
            showButton(btnYes, false);
            showButton(btnNo, false);
            if (mode == PopupMode.Confirmation) {
                showButton(btnYes, true);
                showButton(btnNo, true);
            }
			else if (mode == PopupMode.Input) {
				showButton(btnYes, true);
				showButton(btnNo, true);
			}
            else if (mode == PopupMode.Notification) {
                showButton(btnOkay, true);
            }

            _callback = callback;
            if (blocker) {
                blocker.SetActive(true);
            }
            gameObject.SetActive(true);
        }

		//
		public void showPopup(PopupMode mode, string header, string caption, string placeholder, Action<int> callback = null) {

			showPopup (mode, header, "", callback);

			inputField.gameObject.SetActive(true);

			txtInputCaption.text = caption;
			if (LevelData.Instance.lastLevelName == Globals.defaultLevelName) {
				txtInputPlaceHolder.text = placeholder;
				inputField.text = "";
			} else {
				txtInputPlaceHolder.text = "";
				inputField.text = LevelData.Instance.lastLevelName;
			}
		}

		//
        public void hide() {

            _callback = null;
            if (blocker) {
                blocker.SetActive(false);
            }
            gameObject.SetActive(false);
        }

		//
		public bool isVisible()
		{
			return (gameObject.activeSelf);
		}

        //
        // Private methods
        //
        private void showButton(GameObject btn, bool active) {
            if (btn) {
                btn.SetActive(active);
            }
        }

        //
        // button click handlers
        //
        public void onBtnOkayClick() {

			if (_callback != null) {
				_callback.Invoke (0);
			} else {
				hide ();
			}
        }

        public void onBtnYesClick() {

            if (_callback != null) {
                _callback.Invoke(1);
			} else {
				hide ();
            }
        }

        public void onBtnNoClick() {

            if (_callback != null) {
                _callback.Invoke(2);
			} else {
				hide ();
            }
        }
    }
}