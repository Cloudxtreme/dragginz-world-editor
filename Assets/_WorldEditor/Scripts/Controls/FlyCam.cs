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

		private static float movementSpeed = 0.05f;
		private Vector3 initialPos;
		private Vector3 initialRotation;

		void Start() {
			initialPos = transform.position;
			initialRotation = transform.eulerAngles;
			reset ();
		}

		void Update () {
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				movementSpeed = Mathf.Max (movementSpeed += (Input.GetAxis ("Mouse ScrollWheel") * 0.5f), 0.05f);
				MainMenu.Instance.setMovementSpeedText (movementSpeed);
			}
			transform.position += (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * Input.GetAxis("Depth")) * movementSpeed;
			transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), Input.GetAxis("Rotation"));
			MainMenu.Instance.setCameraPositionText (transform.position);
		}

		public void reset()
		{
			movementSpeed = 0.05f;
			transform.position = initialPos;
			transform.eulerAngles = initialRotation;
			MainMenu.Instance.setMovementSpeedText (movementSpeed);
			MainMenu.Instance.setCameraPositionText (transform.position);
		}
	}
}