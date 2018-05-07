//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace PrefabWorldEditor
{
	public class PrefabLevelEditor : MonoBehaviour
    {
        public Transform playerEdit;
		public Transform playerPlay;

        [SerializeField]
        public Vector3Int levelSize;
        public float gridSize;

        public Transform trfmWalls;

		//

		const int CAVE_FLOOR = 0;
		const int CAVE_WALL = 1;
		const int CAVE_PATH_A = 2;
		const int CAVE_PATH_B = 3;
		const int CAVE_PATH_C1 = 4;
		const int CAVE_PATH_C2 = 5;

        private struct Part
        {
            public int id;
            public GameObject prefab;

            public float w;
            public float h;
            public float d;
        }

        private struct LevelElement
        {
            public GameObject go;
            public int partId;
        }

        private Transform _container;

        private Dictionary<int, Part> _parts;
        private LevelElement[,,] _levelElements;

        //private int _curEditPartId;
		private Part _curEditPart;
        private GameObject _goEditPart;
        private Vector3 _v3EditPartPos;
		private int _curRotation;

		private LevelElement _selectedElement;

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
            _parts = new Dictionary<int, Part>();

			createPart(CAVE_FLOOR, "MDC/CATA_BlockC1", 6f, .75f, 6f);
			createPart(CAVE_WALL, "MDC/CATA_GenericWall", 3f, 3f, .5f);
			createPart(CAVE_PATH_A, "MDC/CATA_CavePathA", 5f, 4f, 12f);
			createPart(CAVE_PATH_B, "MDC/CATA_CavePathB", 5f, 6f, 12f);
			createPart(CAVE_PATH_C1, "MDC/CATA_CavePathC1", 12f, 3f, 3f);
			createPart(CAVE_PATH_C2, "MDC/CATA_CavePathC2", 8f, 8f, 8f);

            _container = new GameObject("[Container]").transform;

            setWalls();

            createDefaultLevel();

			//_curEditPartId = CAVE_PATH_A;
			_curEditPart = _parts [CAVE_FLOOR];
            _v3EditPartPos = Vector3.zero;
			_goEditPart = createPartAt(_curEditPart.id, -1, -1, -1);
			_curRotation = 0;
            disableMeshCollider();

			_selectedElement = new LevelElement();
			_selectedElement.partId = -1;

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
			}

			_rayDistance = 5f;
			_goHit = null;
			_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_ray, out _hit, 100)) {
				_goHit = _hit.collider.gameObject;
				_rayDistance = _hit.distance;
			}

			if (!_goEditPart.activeSelf) {
				if (Input.GetMouseButtonDown (0)) {
					if (_goHit != null) {
						selectElement (_goHit.transform.parent);
					}
				}
				if (_selectedElement.go != null) {
					positionSelectedElement ();
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
			if (_v3EditPartPos.y < 0) {
				_v3EditPartPos.y = 0;
			}
			else if (_v3EditPartPos.y + _curEditPart.h > levelSize.y) {
				_v3EditPartPos.y = levelSize.y - _curEditPart.h;
			}

            _v3EditPartPos.z = Mathf.RoundToInt(_v3EditPartPos.z / gridSize) * gridSize;
			if (_v3EditPartPos.z - _curEditPart.d / 2 < 0) {
				_v3EditPartPos.z = _curEditPart.d / 2;
			}
			else if (_v3EditPartPos.z + _curEditPart.d / 2 > levelSize.z) {
				_v3EditPartPos.z = levelSize.z - _curEditPart.d / 2;
			}
            _goEditPart.transform.position = _v3EditPartPos;

			// add wall collision check

            if (Input.GetAxis("Mouse ScrollWheel") != 0) {
                if (_timer > _lastMouseWheelUpdate) {
                    _lastMouseWheelUpdate = _timer + 0.2f;
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                        toggleEditPart();
                    }
                    else {
						_curRotation = (_curRotation < 3 ? _curRotation + 1 : 0);
						if (_curRotation == 0) {
							_goEditPart.transform.rotation = Quaternion.identity;
						} else {
							_goEditPart.transform.Rotate (Vector3.up * 90f);
						}
                    }
                }
            }

			if (Input.GetMouseButtonDown (0))
			{
				if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.id == CAVE_FLOOR) {
					fillY (_goEditPart.transform.position);
				}
				else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.id == CAVE_WALL) {
					fillZ (_goEditPart.transform.position);
				}
				else {
					placePart (_goEditPart.transform.position);
				}
			}
        }

		//
		private void selectElement(Transform trfmHit)
		{
			_selectedElement = new LevelElement();
			_selectedElement.partId = -1;

			int lenX = (int)((float)levelSize.x / gridSize);
			int lenY = (int)((float)levelSize.y / gridSize);
			int lenZ = (int)((float)levelSize.z / gridSize);
			int x, y, z;
			for (x = 0; x < lenX; ++x) {
				for (y = 0; y < lenY; ++y) {
					for (z = 0; z < lenZ; ++z) {
						if (_levelElements [x, y, z].go != null && _levelElements [x, y, z].go.transform == trfmHit) {
							_selectedElement = _levelElements [x, y, z];
							break;	
						}
					}
				}
			}
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
		}

        //
        private void toggleEditPart() {

			int id = _curEditPart.id;
			if (++id > CAVE_PATH_C2) {
				id = CAVE_FLOOR;
            }
			_curEditPart = _parts [id];

            Destroy(_goEditPart);
            _goEditPart = null;
			_goEditPart = createPartAt(_curEditPart.id, -1, -1, -1);
			_curRotation = 0;
            disableMeshCollider();
        }

        //
        private void disableMeshCollider () {

            GameObject go = _goEditPart;
            if (go.GetComponent<Collider>()) {
                go.GetComponent<Collider>().enabled = false;
            }
			else {
				foreach (Transform child in go.transform) {
					if (child.gameObject.GetComponent<Collider> ()) {
						child.gameObject.GetComponent<Collider> ().enabled = false;
					}
				}
				/*if (go.transform.Find ("part")) {
					GameObject go2 = go.transform.Find ("part").gameObject;
					if (go2.GetComponent<MeshCollider> ()) {
						go2.GetComponent<MeshCollider> ().enabled = false;
					}
				}*/
            }
        }

        //
        private void placePart(Vector3 pos) {

            int x = (int)(pos.x / gridSize);
            int y = (int)(pos.y / gridSize);
            int z = (int)(pos.z / gridSize);
			Debug.Log (x+", "+y+", "+z);

            LevelElement element = _levelElements[x, y, z];

            if (element.go != null) {
                Destroy(element.go);
                element.go = null;
            }
			element.partId = _curEditPart.id;
			element.go = createPartAt(_curEditPart.id, x, y, z);
            element.go.transform.rotation = _goEditPart.transform.rotation;

            _levelElements[x, y, z] = element;
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
        private void createPart(int id, string prefab, float w, float h, float d)
        {
            Part p = new Part();

            p.id = id;
            p.prefab = Resources.Load<GameObject>("Prefabs/" + prefab);
            p.w = w;
            p.h = h;
            p.d = d;

            _parts.Add(id, p);
        }

        //
        private GameObject createPartAt(int partId, float x, float y, float z) {
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

            float w = (float)levelSize.x;
            float h = (float)levelSize.y;
            float d = (float)levelSize.z;

            Vector2 matScale = new Vector2(w, h);
            Vector3 goScale = new Vector3(w, h, d);

            setWall("wall_f", goScale, new Vector3(w / 2f + 1, h / 2f + 1, d + 1), matScale);
            setWall("wall_b", goScale, new Vector3(w / 2f + 1, h / 2f + 1, 0 + 1), matScale);
            setWall("wall_l", goScale, new Vector3(0 + 1, h / 2f + 1, d / 2f + 1), matScale);
            setWall("wall_r", goScale, new Vector3(w + 1, h / 2f + 1, d / 2f + 1), matScale);
            setWall("wall_u", goScale, new Vector3(w / 2f + 1, h + 1, d / 2f + 1), matScale);
            setWall("wall_d", goScale, new Vector3(w / 2f + 1, 0 + 1, d / 2f + 1), matScale);

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
        //
        // ------------------------------------------------------------------------
        private void createDefaultLevel()
        {
            int lenX = (int)((float)levelSize.x / gridSize);
            int lenY = (int)((float)levelSize.y / gridSize);
            int lenZ = (int)((float)levelSize.z / gridSize);
            _levelElements = new LevelElement[lenX, lenY, lenZ];

            int x, y, z;
            for (x = 0; x < lenX; ++x) {
                for (y = 0; y < lenY; ++y) {
                    for (z = 0; z < lenZ; ++z) {

                        LevelElement element = new LevelElement();
                        element.go = null;
                        element.partId = -1;

                        /*
                        // corners
                        if (x == 0 && y == 0 && z == (len - 1)) {
                            element.go = createPartAt(PART_CORNER_BFL, x, y, z);
                        }
                        else if (x == (len - 1) && y == 0 && z == (len - 1)) {
                            element.go = createPartAt(PART_CORNER_BFR, x, y, z);
                        }
                        else if (x == 0 && y == 0 && z == 0) {
                            element.go = createPartAt(PART_CORNER_BBL, x, y, z);
                        }
                        else if (x == (len - 1) && y == 0 && z == 0) {
                            element.go = createPartAt(PART_CORNER_BBR, x, y, z);
                        }
                        else if (x == 0 && y == (len - 1) && z == (len - 1)) {
                            element.go = createPartAt(PART_CORNER_TFL, x, y, z);
                        }
                        else if (x == (len - 1) && y == (len - 1) && z == (len - 1)) {
                            element.go = createPartAt(PART_CORNER_TFR, x, y, z);
                        }
                        else if (x == 0 && y == (len - 1) && z == 0) {
                            element.go = createPartAt(PART_CORNER_TBL, x, y, z);
                        }
                        else if (x == (len - 1) && y == (len - 1) && z == 0) {
                            element.go = createPartAt(PART_CORNER_TBR, x, y, z);
                        }
                        // edges
                        else if (x == 0 && y == 0 && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_EDGE_BL, x, y, z);
                        }
                        else if (x == (len - 1) && y == 0 && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_EDGE_BR, x, y, z);
                        }
                        else if (x > 0 && x < (len - 1) && y == 0 && z == (len - 1)) {
                            element.go = createPartAt(PART_EDGE_BF, x, y, z);
                        }
                        else if (x > 0 && x < (len - 1) && y == 0 && z == 0) {
                            element.go = createPartAt(PART_EDGE_BB, x, y, z);
                        }
                        else if (x == 0 && y == (len - 1) && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_EDGE_TL, x, y, z);
                        }
                        else if (x == (len - 1) && y == (len - 1) && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_EDGE_TR, x, y, z);
                        }
                        else if (x > 0 && x < (len - 1) && y == (len - 1) && z == (len - 1)) {
                            element.go = createPartAt(PART_EDGE_TF, x, y, z);
                        }
                        else if (x > 0 && x < (len - 1) && y == (len - 1) && z == 0) {
                            element.go = createPartAt(PART_EDGE_TB, x, y, z);
                        }
                        // side edges
                        else if (x == 0 && y > 0 && y < (len - 1) && z == (len - 1)) {
                            element.go = createPartAt(PART_EDGE_LB, x, y, z);
                        }
                        else if (x == (len - 1) && y > 0 && y < (len - 1) && z == (len - 1)) {
                            element.go = createPartAt(PART_EDGE_RB, x, y, z);
                        }
                        else if (x == 0 && y > 0 && y < (len - 1) && z == 0) {
                            element.go = createPartAt(PART_EDGE_LF, x, y, z);
                        }
                        else if (x == (len - 1) && y > 0 && y < (len - 1) && z == 0) {
                            element.go = createPartAt(PART_EDGE_RF, x, y, z);
                        }
                        */
                        // sides
                        /*else if (x > 0 && x < (len - 1) && y > 0 && y < (len - 1) && z == (len - 1)) {
                            element.go = createPartAt(PART_SIDE_B, x, y, z);
                        }
                        else if (x > 0 && x < (len - 1) && y > 0 && y < (len - 1) && z == 0) {
                            element.go = createPartAt(PART_SIDE_F, x, y, z);
                        }
                        else if (x == 0 && y > 0 && y < (len - 1) && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_SIDE_L, x, y, z);
                        }
                        else if (x == (len - 1) && y > 0 && y < (len - 1) && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_SIDE_R, x, y, z);
                        }
                        else if (x > 0 && x < (len - 1) && y == 0 && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_SIDE_BOT, x, y, z);
                        }
                        else if (x > 0 && x < (len - 1) && y == (len - 1) && z > 0 && z < (len - 1)) {
                            element.go = createPartAt(PART_SIDE_TOP, x, y, z);
                        }*/

                        _levelElements[x, y, z] = element;
                    }
                }
            }

            Debug.Log("num objects: " + _container.childCount);
        }
    }
}