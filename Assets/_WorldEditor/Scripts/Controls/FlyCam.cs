//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public class FlyCam : MonoSingleton<FlyCam> {

		private static float movementSpeed = 0.15f;

		private Camera myCam;

		private Transform player;
		private Vector3 initialPos;
		private Vector3 initialRotation;

		private Vector3 playerEuler;
		private Vector3 camOffset;

		private Vector3 mousePos;
		private Vector3 dragOrigin;
		private Vector3 dragDiff;

		private float _mouseWheel;
		private float _inputH;

		private float _nextPosUpdate;

		void Awake()
		{
			myCam = GetComponent<Camera> ();

			player = transform.parent;
			initialPos = player.position;
			initialRotation = player.eulerAngles;

			playerEuler = player.eulerAngles;

			mousePos   = Vector3.zero;
			dragOrigin = Vector3.zero;
			dragDiff   = Vector3.zero;

			_nextPosUpdate = 0;
		}

		void Update ()
		{
			//if (PlayerEditCollision.Instance.isColliding) {
			//	return;
			//}

			_mouseWheel = 0;
			if (!Input.GetKey (KeyCode.LeftShift)) {
				_mouseWheel = (AppController.Instance.appState != AppState.Null ? Input.GetAxis ("Mouse ScrollWheel") : 0);
			}

			if (_mouseWheel != 0) {
				_mouseWheel = (_mouseWheel < 0 ? -0.1f : 0.1f);
				player.position += transform.forward * _mouseWheel;// * movementSpeed;
			}

			// Looking around with the mouse
			if (Input.GetMouseButton (1)) {

				player.Rotate(-2f * Input.GetAxis("Mouse Y"), 2f * Input.GetAxis("Mouse X"), 0);
				playerEuler = player.eulerAngles;
				playerEuler.z = 0;
				player.eulerAngles = playerEuler;
			}

			if (_mouseWheel == 0) {
				player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;
				//player.position += (transform.up * Input.GetAxis ("Depth")) * movementSpeed;
			}

			if (AppController.Instance.appState == AppState.Look) {
				
				if (Input.GetMouseButtonDown(0))
				{
					mousePos = Input.mousePosition;
					mousePos.z = 10;
					dragOrigin = myCam.ScreenToWorldPoint(mousePos);
				}

				if (Input.GetMouseButton (0)) {
					
					mousePos = Input.mousePosition;
					mousePos.z = 10;
					dragDiff = myCam.ScreenToWorldPoint(mousePos) - player.position;
					player.position = dragOrigin - dragDiff;
				}
			}
			/*else
			{
				if (_mouseWheel == 0) {
					player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;
					//player.position += (transform.up * Input.GetAxis ("Depth")) * movementSpeed;
				}
			}*/

			if (Time.realtimeSinceStartup > _nextPosUpdate) {
				_nextPosUpdate = Time.realtimeSinceStartup + 0.5f;
				MainMenu.Instance.setCameraPositionText (player.position);
			}
		}

		public void toggleOffset()
		{
			camOffset = (camOffset == Vector3.zero ? new Vector3(0, 0.74f, 0.183f) : Vector3.zero);
			transform.localPosition = camOffset;
			transform.localEulerAngles = (camOffset == Vector3.zero ? Vector3.zero : new Vector3(30f, 0, 0));
		}

		public void reset()
		{
			movementSpeed = 0.15f;

			camOffset = Vector3.zero;
			transform.localPosition = camOffset;

			player.position = initialPos;
			player.eulerAngles = initialRotation;
			MainMenu.Instance.setMovementSpeedText (movementSpeed);
			MainMenu.Instance.setCameraPositionText (player.position);
		}
	}
}