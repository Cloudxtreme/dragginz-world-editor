//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;

using UnityEngine;
using UnityEngine.UI;

using RTEditor;

namespace DragginzWorldEditor
{
    public class Popup : MonoBehaviour
    {
        public GameObject blocker;
        public Text txtHeader;
        public Text txtMessage;
        public GameObject btnOkay;
        public GameObject btnYes;
        public GameObject btnNo;

        private Action<int> _callback;

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

            showButton(btnOkay, false);
            showButton(btnYes, false);
            showButton(btnNo, false);
            if (mode == PopupMode.Confirmation) {
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
            transform.gameObject.SetActive(true);
        }

        public void hide() {

            _callback = null;
            if (blocker) {
                blocker.SetActive(false);
            }
            transform.gameObject.SetActive(false);
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