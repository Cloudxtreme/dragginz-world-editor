//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzWorldEditor
{
	public class World : MonoSingleton<World> {

		private LevelEditor _levelEditor;
		private AssetFactory _assetFactory;

		//private Dictionary<int, Dictionary<int, Dictionary<int, int>>> _quadrantFlags;
		private Dictionary<string, int> _quadrantFlagsNew;
		//private int _iMinLevelCoord;
		//private int _iMaxLevelCoord;

		private Dictionary<GameObject, bool> _visibleQuadrants;
		private List<GameObject> _aQuadrantChangedVisibility;

		private int _numCubes;

		//private bool _coroutineIsRunning;

		#region Getters

		public int numCubes {
			get { return _numCubes; }
			set { _numCubes = value; }
		}

		#endregion

		public void init()
		{
			_levelEditor = LevelEditor.Instance;
			_assetFactory = AssetFactory.Instance;

			//_iMinLevelCoord = -50;
			//_iMaxLevelCoord = 50;

			//_quadrantFlags = new Dictionary<int, Dictionary<int, Dictionary<int, int>>> ();
			_quadrantFlagsNew = new Dictionary<string, int> ();

			/*for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				_quadrantFlags.Add(x, new Dictionary<int, Dictionary<int, int>> ());
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					_quadrantFlags [x].Add(y, new Dictionary<int, int> ());
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_quadrantFlags [x] [y].Add(z, 0);
					}
				}
			}*/

			_visibleQuadrants = new Dictionary<GameObject, bool> ();
			_aQuadrantChangedVisibility = new List<GameObject> ();

			_numCubes = 0;

			//_coroutineIsRunning = false;

			//LevelData.Instance.loadLevelResource(_levelEditor.goWorld, "Data/Levels/Genesis");
			LevelManager.Instance.loadLevelByIndex (0);
		}

		//
		public void resetAll() {

			resetWorld();

			/*if (_coroutineIsRunning) {
				StopCoroutine("updateQuadrantVisibility");
				_coroutineIsRunning = false;
			}*/
		}

		public void createEmptyLevel() {
			
			createEmptyWorld ();
		}

		//
		/*public void customUpdate()
		{
			if (!_coroutineIsRunning && _visibleQuadrants.Count > 0) {
				StartCoroutine("updateQuadrantVisibility");
			}
		}*/

		//
		public void setQuadrantVisibilityFlag(GameObject quadrant, bool visible)
		{
			//Debug.Log (quadrant.name+".visible = "+visible);

			_visibleQuadrants [quadrant] = visible;

			if (_aQuadrantChangedVisibility.Contains (quadrant)) {
				_aQuadrantChangedVisibility.Remove (quadrant);
			}

			_aQuadrantChangedVisibility.Add (quadrant);
		}

		//
		/*private IEnumerator updateQuadrantVisibility()
		{
			int len = _aQuadrantChangedVisibility.Count;
			while (len > 0) {

				_coroutineIsRunning = true;

				GameObject go;
				Transform container;
				int i;
				for (i = 0; i < 5; ++i) {

					go = _aQuadrantChangedVisibility [0];
					bool visible = _visibleQuadrants [go];
					_aQuadrantChangedVisibility.RemoveAt (0);

					if (go != null) {
						container = go.transform.Find ("container");
						if (container != null) {
							container.gameObject.SetActive (visible);
						}
					}

					len = _aQuadrantChangedVisibility.Count;
					if (len <= 0) {
						break;
					}
				}

				yield return new WaitForEndOfFrame();
			}

			_coroutineIsRunning = false;
		}*/

		//
		private void createEmptyWorld() {

			float fQuadrantSize = _levelEditor.fQuadrantSize;
			int count = 0;

			// create hollow cube of cubes :)
			Vector3 v3Center = new Vector3(Globals.LEVEL_WIDTH / 2, Globals.LEVEL_HEIGHT / 2, Globals.LEVEL_DEPTH / 2);
			int size = 2; // actual size will be size*2+1
			int height = 3;
			Vector3 pos = Vector3.zero;
			for (int x = -size; x <= size; ++x) {
				for (int y = -1; y <= height; ++y) {
					for (int z = -size; z <= size; ++z) {

						pos = new Vector3 ((x + (int)v3Center.x) * fQuadrantSize, (y + (int)v3Center.y) * fQuadrantSize, (z + (int)v3Center.z) * fQuadrantSize);

						if (Mathf.Abs (x) == size || y == -1 || y == height || Mathf.Abs (z) == size) {
							createRockCube (pos);
						} else {
							createRockCube (pos, false);
						}

						count++;
					}
				}
			}
			Debug.Log ("quadrants: "+count.ToString());
			Debug.Log ("cubes: "+_numCubes.ToString());

			MainMenu.Instance.setCubeCountText (_numCubes);
		}

		//
		private void resetWorld()
		{
			PropsManager.Instance.reset ();

			_numCubes = 0;

			foreach (Transform child in _levelEditor.goWorld.transform) {
				Destroy (child.gameObject);
			}

			foreach (Transform child in _levelEditor.goProps.transform) {
				Destroy (child.gameObject);
			}

			_visibleQuadrants.Clear ();
			_aQuadrantChangedVisibility.Clear ();

			/*for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_quadrantFlags [x] [y] [z] = 0;
					}
				}
			}*/

			_quadrantFlagsNew.Clear ();
		}

		//
		public void createRockCube (Vector3 v3CubePos, bool fillQuadrant = true)
		{
			int qX = (int)v3CubePos.x;
			int qY = (int)v3CubePos.y;
			int qZ = (int)v3CubePos.z;

			// out of level bounds?
			if (qX < -1 || qY < -1 || qZ < -1) {
				return;
			}
			else if (qX > Globals.LEVEL_WIDTH || qY > Globals.LEVEL_HEIGHT || qZ > Globals.LEVEL_DEPTH) {
				return;
			}

			string quadrantId = qX.ToString() + "_" + qY.ToString() + "_" + qZ.ToString();

			// cube already created at that position
			//if (_quadrantFlagsNew.ContainsKey (quadrantId)) {
			//	return;
			//}

			//if (_quadrantFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] == 1) {
			//	return;
			//}

			bool isEdgeQuadrant = ((qX == -1 || qY == -1 || qZ == -1) || (qX == Globals.LEVEL_WIDTH || qY == Globals.LEVEL_HEIGHT || qZ == Globals.LEVEL_DEPTH));
			//Debug.Log (quadrantId + ":isEdgeQuadrant: " + isEdgeQuadrant);

			GameObject cubeParent = createQuadrant (v3CubePos, quadrantId);
			if (cubeParent == null) {
				return;
			}
			//GameObject container = createContainer (cubeParent.transform);

			Transform trfmContainer = cubeParent.transform.Find (Globals.cubesContainerName);
			if (trfmContainer == null) {
				return;
			}
			GameObject container = trfmContainer.gameObject;

			if (!fillQuadrant) {
				foreach (Transform cube in trfmContainer) {
					cube.gameObject.SetActive (false);
					_numCubes--;
				}
			} else if (isEdgeQuadrant) {
				Renderer renderer;
				foreach (Transform cube in trfmContainer) {
					renderer = cube.GetComponent<Renderer> ();
					if (renderer != null) {
						renderer.material = _levelEditor.materialEdge;
					}
					cube.gameObject.tag = "Untagged";
				}
			}

			/*float fRockSize = _levelEditor.fRockSize;
			Vector3 pos = Vector3.zero;
			int len = _levelEditor.cubesPerQuadrant;
			float startPos = 0;
			int id = 0;
			string sName = "";

			pos.x = startPos;
			for (int x = 0; x < len; ++x) {
				pos.y = startPos;
				for (int y = 0; y < len; ++y) {
					pos.z = startPos;
					for (int z = 0; z < len; ++z) {

						id = x * (len * len) + y * len + z;
						sName = id.ToString ();
						createRock (pos, container, sName, null, isEdgeQuadrant);

						pos.z += fRockSize;
					}
					pos.y += fRockSize;
				}
				pos.x += fRockSize;
			}*/
		}

		//
		public GameObject createQuadrant(Vector3 v3CubePos, string quadrantId)
		{
			//string sPos = v3CubePos.x.ToString () + "_" + v3CubePos.y.ToString () + "_" + v3CubePos.z.ToString ();

			// cube already created at that position
			if (_quadrantFlagsNew.ContainsKey (quadrantId)) {
				Debug.Log ("whatwhatwhat?");
				return null;
			}

			GameObject quadrant = _assetFactory.createQuadrantClone ();
			quadrant.name = Globals.containerGameObjectPrepend + quadrantId;
			quadrant.transform.SetParent(_levelEditor.goWorld.transform);
			quadrant.transform.localPosition = v3CubePos;
			quadrant.isStatic = true;

			_numCubes += 8;
				
			/*if (_levelEditor.cubePrefabCenter != null) {
				GameObject go = Instantiate(_levelEditor.cubePrefabCenter);
				go.name = "center_" + quadrantId;
				go.transform.SetParent(quadrant.transform);
				go.transform.localPosition = new Vector3(0.25f, 0.25f, 0.25f);
				//Block blockScript = go.AddComponent<Block> ();
				//blockScript.init ();
			}*/

			//_quadrantFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] = 1;

			_quadrantFlagsNew.Add(quadrantId, 1);
			//Debug.Log ("Creating Quadrant "+quadrantId);

			_visibleQuadrants.Add (quadrant, true);

			return quadrant;
		}

		/*public GameObject createContainer(Transform parent) {
			
			GameObject container = new GameObject ();
			container.name = "container";
			container.transform.SetParent (parent);
			container.transform.localPosition = Vector3.zero;
			container.isStatic = true;

			return container;
		}*/

		//
		public void setCube(GameObject go, Material material = null, bool isEdge = false) {

			if (material != null) {
				go.GetComponent<MeshRenderer> ().material = material;
			} else {
				go.GetComponent<MeshRenderer> ().material = _levelEditor.materialsWalls [UnityEngine.Random.Range (0, _levelEditor.materialsWalls.Count)];
			}

			if (isEdge) {
				go.tag = "Untagged";
			}
		}

		//
		/*public GameObject createRock(Vector3 pos, GameObject parent, string name, Material material = null, bool isEdge = false) {

			GameObject go = null;
			GameObject prefab = null;
			Vector3 rotation = Vector3.zero;

			prefab = (isEdge ? _levelEditor.cubePrefabEdge : _levelEditor.cubePrefab);
			if (prefab) {
				go = Instantiate(prefab);
				go.name = name;
				go.transform.SetParent(parent.transform);
				go.transform.localPosition = pos;
				go.transform.localRotation = Quaternion.Euler (rotation);
				if (!isEdge) {
					if (material != null) {
						go.GetComponent<MeshRenderer> ().material = material;
					} else {
						go.GetComponent<MeshRenderer> ().material = _levelEditor.materialsWalls [UnityEngine.Random.Range (0, _levelEditor.materialsWalls.Count)];
					}
				}
				_numCubes++;
			}

			return go;
		}*/

		//
		public GameObject createProp(propDef prop, Vector3 v3Pos, string name, Transform parent, bool useCollider = true, bool useGravity = true)
		{
			GameObject goNew = Instantiate (prop.prefab);
			goNew.transform.SetParent (parent);
			goNew.transform.position = v3Pos;
			goNew.name = name;

			Collider collider = goNew.GetComponent<Collider> ();
			if (collider != null) {
				collider.enabled = useCollider;
			}

			Rigidbody rigidBody = goNew.GetComponent<Rigidbody> ();
			if (rigidBody != null) {
				rigidBody.useGravity = useGravity;
			}

			return goNew;
		}
	}
}