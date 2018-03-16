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

		private Camera _myCam;
		private Camera _itemCam;

		private Transform _player;
		private Vector3 initialPos;
		private Vector3 initialRotation;

		private Vector3 playerEuler;
		private Vector3 camOffset;

		//private PlayerEditCollision playerCollision;

		//private Vector3 mousePos;
		//private Vector3 dragOrigin;
		//private Vector3 dragDiff;

		private Vector3 _tempPos;

		private RaycastHit hitInfo;
		private Vector3 _playerPosSave;
		private LayerMask _layermask;
		private Collider[] _hitColliders;

		//private float _mouseWheel;
		private bool _mouseRightIsDown;

		private float _time;
		private float _nextPosUpdate;
		private float _nextDistanceUpdate;

		//private bool _camCanMove;
		private bool _move;

		private Popup _popup;

		public bool drawWireframe;

		#region Getters

		public Transform player {
			get { return _player; }
		}

		#endregion

		void Awake()
		{
			_myCam = GetComponent<Camera> ();

			_player = transform.parent;

			// center player in level
			initialPos = _player.position;
			initialPos.x += Globals.LEVEL_WIDTH  / 2;
			initialPos.y += Globals.LEVEL_HEIGHT / 2;
			initialPos.z += Globals.LEVEL_DEPTH  / 2;
			_player.position = initialPos;

			initialRotation = _player.eulerAngles;

			playerEuler = _player.eulerAngles;

			//mousePos   = Vector3.zero;
			//dragOrigin = Vector3.zero;
			//dragDiff   = Vector3.zero;

			_playerPosSave = _player.position;
			_layermask = 1 << 8;

			//_mouseWheel = 0;
			_mouseRightIsDown = false;

			_time = 0;
			_nextPosUpdate = 0;
			_nextDistanceUpdate = 0;

			//_camCanMove = true;
			_move = false;

			drawWireframe = false;
		}

		void Start() {

			_itemCam = LevelEditor.Instance.itemCam;

			_popup = MainMenu.Instance.popup;
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
			if (_popup.isVisible ()) {
				return;
			}

			_time = Time.realtimeSinceStartup;

			//if (_camCanMove) {
				_move = Input.GetAxis ("Horizontal") != 0.0f || Input.GetAxis ("Vertical") != 0.0f || Input.GetAxis ("Depth") != 0.0f;
			//}
			/*else {
				if (!Input.anyKey) { //Input.GetAxis ("Horizontal") == 0.0f && Input.GetAxis ("Vertical") == 0.0f && Input.GetAxis ("Depth") == 0.0f) {
					_camCanMove = true;
					Input.ResetInputAxes();
				}
			}*/

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

			if (_move) {

				if (drawWireframe) {
					//if (_camCanMove) {
					_player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;
					//}
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

					// did camera move?
					/*if (_player.position != _playerPosSave) {

						_hitColliders = Physics.OverlapSphere (_player.position, 0.26f, _layermask);
						if (_hitColliders.Length > 0) {
							_player.position = _playerPosSave;
							//_move = false;
							Input.ResetInputAxes();
						}
						else {
							_playerPosSave = _player.position;
						}
					}*/
				}
			}

			if (_itemCam.enabled) {
				_itemCam.transform.position = _myCam.transform.position;
				_itemCam.transform.rotation = _myCam.transform.rotation;
			}

			if (_time > _nextDistanceUpdate) {
				_nextDistanceUpdate = _time + 1.0f;
				LevelEditor.Instance.checkLevelChunkDistances ();
			}
			else if (_time > _nextPosUpdate) {
				_nextPosUpdate = _time + 0.5f;
				MainMenu.Instance.setCameraPositionText (_player.position);
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
			MainMenu.Instance.setMovementSpeedText (movementSpeed);
			MainMenu.Instance.setCameraPositionText (_player.position);
		}
	}
}