//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelChunks
{
	public class FlyCamVoxelsDemo2 : MonoBehaviour {

		private static float movementSpeed = 0.15f;

		//private Camera _myCam;

		private Transform _player;
		private Vector3 initialPos;
		private Vector3 initialRotation;

		private Vector3 playerEuler;
		private Vector3 camOffset;

		private Vector3 _tempPos;

		private RaycastHit hitInfo;
		private Vector3 _playerPosSave;
		private LayerMask _layermask;
		private Collider[] _hitColliders;

		private bool _mouseRightIsDown;

		private bool _move;

		public bool drawWireframe;

		#region Getters

		public Transform player {
			get { return _player; }
		}

		#endregion

		void Awake()
		{
			//_myCam = GetComponent<Camera> ();

			_player = transform.parent;

			// center player in level
			initialPos = _player.position;
			initialPos.x += 18f;
			initialPos.y += 18f;
			initialPos.z += 18f;
			_player.position = initialPos;

			initialRotation = _player.eulerAngles;

			playerEuler = _player.eulerAngles;

			_playerPosSave = _player.position;
			_layermask = 1 << 8;

			_mouseRightIsDown = false;

			_move = false;

			drawWireframe = false;
		}

		void OnPreRender() {
			GL.wireframe = drawWireframe;
		}
		void OnPostRender() {
			GL.wireframe = false;
		}

		//
		void Update ()
		{
			_move = Input.GetAxis ("Horizontal") != 0.0f || Input.GetAxis ("Vertical") != 0.0f || Input.GetAxis ("Depth") != 0.0f;

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
				_player.Rotate(-2f * Input.GetAxis("Mouse Y"), 2f * Input.GetAxis("Mouse X"), 0);
				playerEuler = _player.eulerAngles;
				playerEuler.z = 0;
				_player.eulerAngles = playerEuler;
			}

			if (_move) {

				if (drawWireframe) {
					_player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;
				}
				else
				{
					_tempPos = _player.position;
					_tempPos += (transform.right * Input.GetAxis ("Horizontal")) * movementSpeed;
					if (_tempPos != _playerPosSave)
					{
						_hitColliders = Physics.OverlapSphere (_tempPos, 0.26f, _layermask);
						if (_hitColliders.Length <= 0) {
							_player.position = _tempPos;
						}
					}

					_tempPos = _player.position;
					_tempPos += (transform.forward * Input.GetAxis ("Vertical")) * movementSpeed;
					if (_tempPos != _playerPosSave)
					{
						_hitColliders = Physics.OverlapSphere (_tempPos, 0.26f, _layermask);
						if (_hitColliders.Length <= 0) {
							_player.position = _tempPos;
						}
					}

					_tempPos = _player.position;
					_tempPos += (transform.up * Input.GetAxis ("Depth")) * movementSpeed;
					if (_tempPos != _playerPosSave)
					{
						_hitColliders = Physics.OverlapSphere (_tempPos, 0.26f, _layermask);
						if (_hitColliders.Length <= 0) {
							_player.position = _tempPos;
						}
					}

					_playerPosSave = _player.position;
				}
			}
		}

		//
		public void setNewInitialPosition(Vector3 newPos, Vector3 newRot) {
			initialPos = newPos;
			initialRotation = newRot;
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

			_player.position = initialPos;
			_player.eulerAngles = initialRotation;
		}
	}
}