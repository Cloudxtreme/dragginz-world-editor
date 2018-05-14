﻿//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using AssetsShared;

namespace PrefabWorldEditor
{
	public class PrefabLevelEditor : MonoSingleton<PrefabLevelEditor>
    {
        public Transform playerEdit;
		public Transform playerPlay;

        [SerializeField]
        public Vector3Int levelSize;
        public float gridSize;

        public Transform trfmWalls;

		public Material matElementMarker;

		public GizmoTranslateScript gizmoTranslateScript;

		//

		private enum PartType {
			Floor,
			WallZ,
			WallX,
			Tunnel,
			Pillar
		};

		private enum PartList {
			Floor_1,
			Floor_2,
			Wall_Z,
			Wall_X,
			Path_1,
			Path_2,
			Path_3,
			Path_4,
			Pillar_1,
			Pillar_2,
			Pillar_3,
			End_Of_List
		};

        private struct Part
        {
			public PartList id;
			public PartType type;
			public bool canRotate;
			public bool usesGravity;

            public GameObject prefab;

            public float w;
            public float h;
            public float d;
        }

        private struct LevelElement
        {
            public GameObject go;
			public PartList part;
        }

		private Transform _trfmMarkerX;
		private Transform _trfmMarkerY;
		private Transform _trfmMarkerZ;
		private Transform _trfmMarkerX2;
		private Transform _trfmMarkerY2;
		private Transform _trfmMarkerZ2;

        private Transform _container;

		private Dictionary<PartList, Part> _parts;
		private Dictionary<GameObject, LevelElement> _levelElements;

		private Part _curEditPart;
        private GameObject _goEditPart;
        private Vector3 _v3EditPartPos;
		private int _curRotation;

		private LevelElement _selectedElement;
		private List<Material> _aSelectedElementMaterials;

        private float _rayDistance;
        private Ray _ray;
        private RaycastHit _hit;
        private GameObject _goHit;

		private int _playerMode;

        private float _timer;
        private float _lastMouseWheelUpdate;

        // ------------------------------------------------------------------------
        void Start()
        {
			_parts = new Dictionary<PartList, Part>();

			createPart(PartList.Floor_1,  PartType.Floor,  "MDC/Floor_1",  6f, .75f, 6f,      false, false);
			createPart(PartList.Floor_2,  PartType.Floor,  "MDC/Floor_2",  6f, .25f, 6f,      false, false);
			createPart(PartList.Wall_Z,   PartType.WallZ,  "MDC/Wall_Z",   3f, 3f, .5f,       false, false);
			createPart(PartList.Wall_X,   PartType.WallX,  "MDC/Wall_X",   0.5f, 3f, 3f,      false, false);
			createPart(PartList.Path_1,   PartType.Tunnel, "MDC/Path_1",   5f, 1.8f, 12f,     true, false);
			createPart(PartList.Path_2,   PartType.Tunnel, "MDC/Path_2",   5f, 6f, 12f,       true, false);
			createPart(PartList.Path_3,   PartType.Tunnel, "MDC/Path_3",  12f, 3f, 3f,        true, false);
			createPart(PartList.Path_4,   PartType.Tunnel, "MDC/Path_4",   8f, 8f, 8f,        true, false);
			createPart(PartList.Pillar_1, PartType.Pillar, "MDC/Pillar_1", 2f, 3f, 2f,        true, true);
			createPart(PartList.Pillar_2, PartType.Pillar, "MDC/Pillar_2", 1.5f, 1.5f, 4.75f, true, true);
			createPart(PartList.Pillar_3, PartType.Pillar, "MDC/Pillar_3", 1.5f, 1.5f, 1.5f,  true, true);

            _container = new GameObject("[Container]").transform;

            setWalls();

			_levelElements = new Dictionary<GameObject, LevelElement>();

			_curEditPart   = _parts [PartList.Floor_1];
            _v3EditPartPos = Vector3.zero;
			_goEditPart    = createPartAt(_curEditPart.id, -10, -10, -10);
			setMarkerScale (_curEditPart);
			_curRotation   = 0;
			setMeshCollider(_goEditPart, false);

			_selectedElement = new LevelElement();
			_aSelectedElementMaterials = new List<Material> ();

			_playerMode = 0;
        }

        //
        void Update() {

			_timer = Time.realtimeSinceStartup;

			if (Input.GetKeyDown (KeyCode.P)) {
				setPlayerMode();
			}

			if (_playerMode == 1) {
				return;
			}

			if (Input.GetKeyDown (KeyCode.X)) {
				_goEditPart.SetActive (!_goEditPart.activeSelf);
				setMarkerActive (_goEditPart.activeSelf);
				if (_goEditPart.activeSelf) {
					resetSelectedElement ();
					setMarkerScale (_curEditPart);
				} else {
					gizmoTranslateScript.gameObject.SetActive (false);
				}
			}

			_rayDistance = 5f;
			_goHit = null;
			_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_ray, out _hit, 100)) {
				_goHit = _hit.collider.gameObject;
				_rayDistance = _hit.distance;
			}

			if (!_goEditPart.activeSelf) {
				if (Input.GetMouseButtonDown (0))
				{
					if (!EventSystem.current.IsPointerOverGameObject ()) {
						if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
							if (_goHit != null) {
								selectElement (_goHit.transform.parent);
							}
						}
					}
				}
				if (_selectedElement.go != null) {
					if (Input.GetKeyDown (KeyCode.Delete)) {
						deleteSelectedElement ();
					} else {
						positionSelectedElement ();
					}
				}
			} else {
				positionEditPart ();				
			}
		}

		//
		private void setPlayerMode()
		{
			_playerMode = (_playerMode == 0 ? 1 : 0);

			playerEdit.gameObject.SetActive (_playerMode == 0);
			playerPlay.gameObject.SetActive (!playerEdit.gameObject.activeSelf);

			_goEditPart.SetActive (_playerMode == 0);

			setMarkerActive (_goEditPart.activeSelf);

			resetSelectedElement ();
		}

		//
		private void positionEditPart()
		{
            _v3EditPartPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _rayDistance));
            _v3EditPartPos.x = Mathf.RoundToInt(_v3EditPartPos.x / gridSize) * gridSize;
			if (_v3EditPartPos.x - _curEditPart.w / 2 < 0) {
				_v3EditPartPos.x = _curEditPart.w / 2;
			}
			else if (_v3EditPartPos.x + _curEditPart.w / 2 > levelSize.x) {
				_v3EditPartPos.x = levelSize.x - _curEditPart.w / 2;
			}

            _v3EditPartPos.y = Mathf.RoundToInt(_v3EditPartPos.y / gridSize) * gridSize;
			if (_v3EditPartPos.y - _curEditPart.h / 2 < 0) {
				_v3EditPartPos.y = _curEditPart.h / 2;
			}
			else if (_v3EditPartPos.y + _curEditPart.h / 2 > levelSize.y) {
				_v3EditPartPos.y = levelSize.y - _curEditPart.h / 2;
			}

            _v3EditPartPos.z = Mathf.RoundToInt(_v3EditPartPos.z / gridSize) * gridSize;
			if (_v3EditPartPos.z - _curEditPart.d / 2 < 0) {
				_v3EditPartPos.z = _curEditPart.d / 2;
			}
			else if (_v3EditPartPos.z + _curEditPart.d / 2 > levelSize.z) {
				_v3EditPartPos.z = levelSize.z - _curEditPart.d / 2;
			}
            _goEditPart.transform.position = _v3EditPartPos;

			setMarkerPosition (_goEditPart.transform);

            if (Input.GetAxis("Mouse ScrollWheel") != 0) {
                if (_timer > _lastMouseWheelUpdate) {
                    _lastMouseWheelUpdate = _timer + 0.2f;
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                        toggleEditPart();
                    }
                    else {
						if (_curEditPart.canRotate) {
							_curRotation = (_curRotation < 3 ? _curRotation + 1 : 0);
							if (_curRotation == 0) {
								_goEditPart.transform.rotation = Quaternion.identity;
							} else {
								_goEditPart.transform.Rotate (Vector3.up * 90f);
							}
						}
                    }
                }
            }

			if (Input.GetMouseButtonDown (0))
			{
				if (!EventSystem.current.IsPointerOverGameObject ()) {
					if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.type == PartType.Floor) {
						fillY (_goEditPart.transform.position);
					} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.type == PartType.WallZ) {
						fillZ (_goEditPart.transform.position);
					} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.type == PartType.WallX) {
						fillX (_goEditPart.transform.position);
					} else {
						placePart (_goEditPart.transform.position);
					}
				}
			}
        }

		//
		private void selectElement(Transform trfmHit)
		{
			resetSelectedElement ();

			if (_levelElements.ContainsKey (trfmHit.gameObject)) {
				_selectedElement = _levelElements [trfmHit.gameObject];
				//getMaterials (_selectedElement.go, matElementMarker);
				setMeshCollider (_selectedElement.go, false);
				setRigidBody (_selectedElement.go, false);
				Part part = _parts [_selectedElement.part];
				setMarkerScale (part);
				gizmoTranslateScript.translateTarget = _selectedElement.go;
				gizmoTranslateScript.init();
				gizmoTranslateScript.gameObject.SetActive (true);
			} else {
				_selectedElement = new LevelElement();
				gizmoTranslateScript.gameObject.SetActive (false);
			}

			setMarkerActive (_selectedElement.go != null);
		}

		//
		private void positionSelectedElement()
		{
			_v3EditPartPos = _selectedElement.go.transform.position;

			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				_v3EditPartPos.x += gridSize;
			}
			else if (Input.GetKeyDown (KeyCode.Alpha2)) {
				_v3EditPartPos.x -= gridSize;
			}
			else if (Input.GetKeyDown (KeyCode.Alpha3)) {
				_v3EditPartPos.y += gridSize;
			}
			else if (Input.GetKeyDown (KeyCode.Alpha4)) {
				_v3EditPartPos.y -= gridSize;
			}
			else if (Input.GetKeyDown (KeyCode.Alpha5)) {
				_v3EditPartPos.z += gridSize;
			}
			else if (Input.GetKeyDown (KeyCode.Alpha6)) {
				_v3EditPartPos.z -= gridSize;
			}

			_selectedElement.go.transform.position = _v3EditPartPos;

			setMarkerPosition (_selectedElement.go.transform);
		}

		//
		private void deleteSelectedElement()
		{
			if (_selectedElement.go != null) {
				if (_levelElements.ContainsKey (_selectedElement.go)) {
					_levelElements.Remove (_selectedElement.go);
				}
				Destroy (_selectedElement.go);
				_selectedElement.go = null;
			}

			_selectedElement = new LevelElement ();

			setMarkerActive (_goEditPart.activeSelf);
		}

        //
        private void toggleEditPart()
		{
			float direction = Input.GetAxis ("Mouse ScrollWheel");

			int id = (int)_curEditPart.id;
			if (direction > 0) {
				if (++id >= (int)PartList.End_Of_List) {
					id = (int)PartList.Floor_1;
				}
			} else {
				if (--id < 0) {
					id = (int)PartList.End_Of_List - 1;
				}
			}
			_curEditPart = _parts [(PartList)id];

            Destroy(_goEditPart);
            _goEditPart = null;
			_goEditPart = createPartAt(_curEditPart.id, -10, -10, -10);
			setMarkerScale (_curEditPart);
			_curRotation = 0;
			setMeshCollider(_goEditPart, false);
        }

        //
		private void setMeshCollider (GameObject go, bool state) {

            if (go.GetComponent<Collider>()) {
				go.GetComponent<Collider>().enabled = state;
            }
			else {
				foreach (Transform child in go.transform) {
					if (child.gameObject.GetComponent<Collider> ()) {
						child.gameObject.GetComponent<Collider> ().enabled = state;
					}
				}
            }
        }

		//
		private void setRigidBody (GameObject go, bool state) {

			if (go.GetComponent<Rigidbody>()) {
				go.GetComponent<Rigidbody>().useGravity = state;
			}
			else {
				foreach (Transform child in go.transform) {
					if (child.gameObject.GetComponent<Rigidbody> ()) {
						child.gameObject.GetComponent<Rigidbody> ().useGravity = state;
					}
				}
			}
		}

		//
		private void setRigidBodies (bool state)
		{
			foreach (KeyValuePair<GameObject, LevelElement> element in _levelElements)
			{
				GameObject go = element.Value.go;
				if (go.GetComponent<Rigidbody> ()) {
					go.GetComponent<Rigidbody> ().useGravity = state;
				} else {
					foreach (Transform child in go.transform) {
						if (child.gameObject.GetComponent<Rigidbody> ()) {
							child.gameObject.GetComponent<Rigidbody> ().useGravity = state;
						}
					}
				}
			}
		}

		//
        private void placePart(Vector3 pos) {

            int x = (int)(pos.x / gridSize);
            int y = (int)(pos.y / gridSize);
            int z = (int)(pos.z / gridSize);
			//Debug.Log (x+", "+y+", "+z);

			LevelElement element = new LevelElement ();
			element.part = _curEditPart.id;
			element.go = createPartAt(_curEditPart.id, x, y, z);
            element.go.transform.rotation = _goEditPart.transform.rotation;

			setMeshCollider(element.go, true);
			setRigidBody (element.go, _curEditPart.usesGravity);

			_levelElements.Add(element.go, element);
        }

		//
		private void fillX(Vector3 pos)
		{
			int lenZ = (int)((float)levelSize.z / _curEditPart.d);
			int lenY = (int)((float)levelSize.y / _curEditPart.h);
			int z, y;
			for (z = 0; z < lenZ; ++z) {
				for (y = 0; y < lenY; ++y) {
					pos.z = _curEditPart.d / 2 + (z * _curEditPart.d);
					pos.y = (y * _curEditPart.h);
					placePart (pos);
				}
			}
		}

		//
		private void fillY(Vector3 pos)
		{
			int lenX = (int)((float)levelSize.x / _curEditPart.w);
			int lenZ = (int)((float)levelSize.z / _curEditPart.d);
			int x, z;
			for (x = 0; x < lenX; ++x) {
				for (z = 0; z < lenZ; ++z) {
					pos.x = _curEditPart.w / 2 + (x * _curEditPart.w);
					pos.z = _curEditPart.d / 2 + (z * _curEditPart.d);
					placePart (pos);
				}
			}
		}

		//
		private void fillZ(Vector3 pos)
		{
			int lenX = (int)((float)levelSize.x / _curEditPart.w);
			int lenY = (int)((float)levelSize.y / _curEditPart.h);
			int x, y;
			for (x = 0; x < lenX; ++x) {
				for (y = 0; y < lenY; ++y) {
					pos.x = _curEditPart.w / 2 + (x * _curEditPart.w);
					pos.y = (y * _curEditPart.h);
					placePart (pos);
				}
			}
		}

        //
		private void createPart(PartList id, PartType type, string prefab, float w, float h, float d, bool canRotate, bool usesGravity)
        {
            Part p = new Part();

            p.id   = id;
			p.type = type;
			p.canRotate = canRotate;
			p.usesGravity = usesGravity;
            p.prefab    = Resources.Load<GameObject>("Prefabs/" + prefab);
            p.w = w;
            p.h = h;
            p.d = d;

            _parts.Add(id, p);
        }

        //
		private GameObject createPartAt(PartList partId, float x, float y, float z)
		{
            GameObject go = null;

            if (!_parts.ContainsKey(partId)) {
                return go;
            }

            go = Instantiate(_parts[partId].prefab);
            if (go != null) {
                go.name = "part_" + _container.childCount.ToString();
                go.transform.SetParent(_container);
                go.transform.position = new Vector3(x * gridSize, y * gridSize, z * gridSize);
            }

            return go;
        }

        // ------------------------------------------------------------------------
        private void setWalls() {

			_trfmMarkerX  = trfmWalls.Find ("marker_x");
			_trfmMarkerY  = trfmWalls.Find ("marker_y");
			_trfmMarkerZ  = trfmWalls.Find ("marker_z");
			_trfmMarkerX2 = trfmWalls.Find ("marker_x2");
			_trfmMarkerY2 = trfmWalls.Find ("marker_y2");
			_trfmMarkerZ2 = trfmWalls.Find ("marker_z2");

            float w = (float)levelSize.x;
            float h = (float)levelSize.y;
            float d = (float)levelSize.z;

            Vector2 matScale = new Vector2(w, h);
            Vector3 goScale = new Vector3(w, h, d);

            setWall("wall_f", goScale, new Vector3(w / 2f, h / 2f, d), matScale);
            setWall("wall_b", goScale, new Vector3(w / 2f, h / 2f, 0), matScale);
            setWall("wall_l", goScale, new Vector3(0, h / 2f, d / 2f), matScale);
            setWall("wall_r", goScale, new Vector3(w, h / 2f, d / 2f), matScale);
            setWall("wall_u", goScale, new Vector3(w / 2f, h, d / 2f), matScale);
            setWall("wall_d", goScale, new Vector3(w / 2f, 0, d / 2f), matScale);

            /*goScale = new Vector3(w + 0.2f, h + 0.2f, d + 0.2f);
            matScale = new Vector2(w / 2, h / 2);

            setWall("wall_f_rock", goScale, new Vector3(w / 2f, h / 2f, d + 0.1f), matScale);
            setWall("wall_b_rock", goScale, new Vector3(w / 2f, h / 2f, -0.1f), matScale);
            setWall("wall_l_rock", goScale, new Vector3(-0.1f, h / 2f, d / 2f), matScale);
            setWall("wall_r_rock", goScale, new Vector3(w + 0.1f, h / 2f, d / 2f), matScale);
            setWall("wall_u_rock", goScale, new Vector3(w / 2f, h + 0.1f, d / 2f), matScale);
            setWall("wall_d_rock", goScale, new Vector3(w / 2f, -0.1f, d / 2f), matScale);*/
        }

        private void setWall(string name, Vector3 scale, Vector3 pos, Vector2 matScale) {

            Transform child = trfmWalls.Find(name);
            if (child != null) {
                child.localScale = scale;
                child.localPosition = pos;

                Renderer r = child.GetComponent<MeshRenderer>();
                if (r != null) {
                    r.material.mainTextureScale = matScale;
                }

                child.gameObject.isStatic = true;
            }
        }

		// ------------------------------------------------------------------------
		// Material stuff
		// ------------------------------------------------------------------------
		/*private void getMaterials(GameObject go, Material setMaterial = null)
		{
			return;

			_aSelectedElementMaterials.Clear ();

			if (go != null) {

				foreach (Transform child in go.transform) { 
				
					Renderer r = child.GetComponent<Renderer> ();
					if (r != null) {
						_aSelectedElementMaterials.Add (r.material);
						if (setMaterial != null) {
							r.material = setMaterial;
						}
					}
				}
			}
		}*/

		/*private void resetMaterials(GameObject go)
		{
			if (go != null) {

				int index = 0;
				int len = _aSelectedElementMaterials.Count;
				foreach (Transform child in go.transform) { 

					Renderer r = child.GetComponent<Renderer> ();
					if (r != null && index < len) {
						r.material = _aSelectedElementMaterials[index];
						index++;
					}
				}
			}

			_aSelectedElementMaterials.Clear ();
		}*/

		private void resetSelectedElement()
		{
			if (_selectedElement.go != null) {

				Part part = _parts [_selectedElement.part];

				setMeshCollider(_selectedElement.go, true);
				setRigidBody (_selectedElement.go, part.usesGravity);
			}
		}

		// ------------------------------------------------------------------------
		// Marker stuff
		// ------------------------------------------------------------------------
		private void setMarkerActive(bool state)
		{
			_trfmMarkerX.gameObject.SetActive (state);
			_trfmMarkerY.gameObject.SetActive (state);
			_trfmMarkerZ.gameObject.SetActive (state);

			_trfmMarkerX2.gameObject.SetActive (state);
			_trfmMarkerY2.gameObject.SetActive (state);
			_trfmMarkerZ2.gameObject.SetActive (state);
		}

		private void setMarkerScale(Part part)
		{
			_trfmMarkerX.localScale = new Vector3 (part.d, part.h, 1);
			_trfmMarkerY.localScale = new Vector3 (part.w, part.d, 1);
			_trfmMarkerZ.localScale = new Vector3 (part.w, part.h, 1);

			_trfmMarkerX2.localScale = new Vector3 (part.d, part.h, 1);
			_trfmMarkerY2.localScale = new Vector3 (part.w, part.d, 1);
			_trfmMarkerZ2.localScale = new Vector3 (part.w, part.h, 1);
		}

		private void setMarkerPosition(Transform trfm)
		{
			_trfmMarkerX.position = new Vector3 (0.01f, trfm.position.y, trfm.position.z);
			_trfmMarkerY.position = new Vector3 (trfm.position.x, 0.01f, trfm.position.z);
			_trfmMarkerZ.position = new Vector3 (trfm.position.x, trfm.position.y, 0.01f);

			_trfmMarkerX2.position = new Vector3 (levelSize.x - 0.01f, trfm.position.y, trfm.position.z);
			_trfmMarkerY2.position = new Vector3 (trfm.position.x, levelSize.y - 0.01f, trfm.position.z);
			_trfmMarkerZ2.position = new Vector3 (trfm.position.x, trfm.position.y, levelSize.z - 0.01f);
		} 
	}
}