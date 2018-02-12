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

		private PlayerEditCollision playerCollision;

		private Vector3 mousePos;
		private Vector3 dragOrigin;
		private Vector3 dragDiff;

		private Vector3 _playerPosSave;
		private LayerMask _layermask;
		private Collider[] _hitColliders;
		private Vector3 _v3PlayerExtents;

		private float _mouseWheel;
		private float _inputH;

		private float _nextPosUpdate;

		public bool drawWireframe = false;

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

			mousePos   = Vector3.zero;
			dragOrigin = Vector3.zero;
			dragDiff   = Vector3.zero;

			_playerPosSave = _player.position;
			_layermask = 1 << 8;
			_v3PlayerExtents = new Vector3 (0.3f, 0.2f, 0.2f);

			_nextPosUpdate = 0;

			drawWireframe = false;
		}

		void Start() {

			_itemCam = LevelEditor.Instance.itemCam;

			playerCollision = PlayerEditCollision.Instance;
		}

		void OnPreRender() {
			GL.wireframe = drawWireframe;
		}
		void OnPostRender() {
			GL.wireframe = false;
		}

		void Update ()
		{
			_mouseWheel = 0;
			//if (!Input.GetKey (KeyCode.LeftShift)) {
			//	_mouseWheel = (AppController.Instance.appState != AppState.Null ? Input.GetAxis ("Mouse ScrollWheel") : 0);
			//}

			if (_mouseWheel != 0) {
				_mouseWheel = (_mouseWheel < 0 ? -0.1f : 0.1f);
				_player.position += transform.forward * _mouseWheel;// * movementSpeed;
			}

			// Looking around with the mouse
			if (Input.GetMouseButton (1)) {

				_player.Rotate(-2f * Input.GetAxis("Mouse Y"), 2f * Input.GetAxis("Mouse X"), 0);
				playerEuler = _player.eulerAngles;
				playerEuler.z = 0;
				_player.eulerAngles = playerEuler;
			}

			if (_mouseWheel == 0) {
				_player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;
				//player.position += (transform.up * Input.GetAxis ("Depth")) * movementSpeed;
			}

			/*if (AppController.Instance.appState == AppState.Look) {
				
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
			}*/
			/*else
			{
				if (_mouseWheel == 0) {
					player.position += (transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical") + transform.up * Input.GetAxis ("Depth")) * movementSpeed;
					//player.position += (transform.up * Input.GetAxis ("Depth")) * movementSpeed;
				}
			}*/

			if (!drawWireframe) {

				// did camera move?
				if (_player.position != _playerPosSave) {
					
					_hitColliders = Physics.OverlapBox (_player.position, _v3PlayerExtents, Quaternion.identity, _layermask);
					if (_hitColliders.Length > 0) {
						_player.position = _playerPosSave;
						dragOrigin = _myCam.ScreenToWorldPoint (mousePos);
					} else {
						_playerPosSave = _player.position;
					}
				}
			}

			if (_itemCam.enabled) {
				_itemCam.transform.position = _myCam.transform.position;
				_itemCam.transform.rotation = _myCam.transform.rotation;
			}

			if (Time.realtimeSinceStartup > _nextPosUpdate) {
				_nextPosUpdate = Time.realtimeSinceStartup + 0.5f;
				MainMenu.Instance.setCameraPositionText (_player.position);
			}
		}

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