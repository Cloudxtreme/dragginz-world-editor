//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public class FlyCam : MonoBehaviour {

		private static float movementSpeed = 0.15f;

		private Transform player;
		private Vector3 initialPos;
		private Vector3 initialRotation;

		private Vector3 camOffset;

		private float mouseWheel;

		void Awake()
		{
			player = transform.parent;
			initialPos = player.position;
			initialRotation = player.eulerAngles;
		}

		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.Equals)) {
				movementSpeed = Mathf.Max (movementSpeed += 0.05f, 0.15f);
				MainMenu.Instance.setMovementSpeedText (movementSpeed);
			} else if (Input.GetKeyDown(KeyCode.Minus)) {
				movementSpeed = Mathf.Max (movementSpeed -= 0.05f, 0.15f);
				MainMenu.Instance.setMovementSpeedText (movementSpeed);
			}	

			mouseWheel = Input.GetAxis ("Mouse ScrollWheel");
			if (mouseWheel != 0) {
				mouseWheel = (mouseWheel < 0 ? -0.1f : 0.1f);
				player.position += transform.forward * mouseWheel;// * movementSpeed;
				//movementSpeed = Mathf.Max (movementSpeed += (Input.GetAxis ("Mouse ScrollWheel") * 0.5f), 0.05f);
				//MainMenu.Instance.setMovementSpeedText (movementSpeed);
			} else {
				player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;
			}
		
			player.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), Input.GetAxis("Rotation"));
			MainMenu.Instance.setCameraPositionText (player.position);
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