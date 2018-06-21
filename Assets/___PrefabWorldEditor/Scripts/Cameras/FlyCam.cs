//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AssetsShared;

namespace PrefabWorldEditor
{
	public class FlyCam : MonoBehaviour
	{
		public Material matLineBounds;

		private static float movementSpeed = 0.15f;

		//private Camera _myCam;

		private Transform _player;
		//private Vector3 initialPos;
		//private Vector3 initialRotation;

		private Vector3 playerEuler;
		private Vector3 camOffset;
        
		//private float _time;
		//private float _nextPosUpdate;

        private bool _mouseRightIsDown;
        //private bool _move;

		void Awake()
		{
			//_myCam = GetComponent<Camera> ();

			_player = transform.parent;

			//initialPos = _player.position;
			//initialRotation = _player.eulerAngles;

			playerEuler = _player.eulerAngles;

			//_time = 0;
			//_nextPosUpdate = 0;

            _mouseRightIsDown = false;
            //_move = false;
		}

		//
		void Update ()
		{
			//_time = Time.realtimeSinceStartup;
			//_move = Input.GetAxis ("Horizontal") != 0.0f || Input.GetAxis ("Vertical") != 0.0f || Input.GetAxis ("Depth") != 0.0f;

			if (!_mouseRightIsDown) {
				if (Input.GetMouseButtonDown (1)) {
					_mouseRightIsDown = true;
				}
			}
			else {
				if (Input.GetMouseButtonUp (1)) {
					_mouseRightIsDown = false;
				}
			}

			// Looking around with the mouse
			if (_mouseRightIsDown) {
				//Debug.Log ("mouse is down - axis x: " + Input.GetAxis ("Mouse X"));
				_player.Rotate(-2f * Input.GetAxis("Mouse Y"), 2f * Input.GetAxis("Mouse X"), 0);
				playerEuler = _player.eulerAngles;
				playerEuler.z = 0;
				_player.eulerAngles = playerEuler;
			}

			_player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;

            //Debug.Log(transform.forward);
		}

		void OnPostRender()
		{
			GLTools.drawBoundingBox (PrefabLevelEditor.Instance.levelController.selectedElementBounds, matLineBounds);
		}
	}
}