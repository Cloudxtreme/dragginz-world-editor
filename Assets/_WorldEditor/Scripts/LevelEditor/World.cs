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

		private Dictionary<int, Dictionary<int, Dictionary<int, int>>> _quadrantFlags;
		private int _iMinLevelCoord;
		private int _iMaxLevelCoord;

		private Dictionary<GameObject, bool> _visibleQuadrants;
		private List<GameObject> _aQuadrantChangedVisibility;

		private int _numCubes;

		private bool _coroutineIsRunning;

		#region Getters

		public int numCubes {
			get { return _numCubes; }
			set { _numCubes = value; }
		}

		#endregion

		public void init()
		{
			_iMinLevelCoord = -50;
			_iMaxLevelCoord = 50;

			_quadrantFlags = new Dictionary<int, Dictionary<int, Dictionary<int, int>>> ();

			for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				_quadrantFlags.Add(x, new Dictionary<int, Dictionary<int, int>> ());
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					_quadrantFlags [x].Add(y, new Dictionary<int, int> ());
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_quadrantFlags [x] [y].Add(z, 0);
					}
				}
			}

			_visibleQuadrants = new Dictionary<GameObject, bool> ();
			_aQuadrantChangedVisibility = new List<GameObject> ();

			_numCubes = 0;

			_coroutineIsRunning = false;

			createWorld ();
		}

		//
		public void resetAll() {

			resetWorld();

			if (_coroutineIsRunning) {
				StopCoroutine("updateQuadrantVisibility");
				_coroutineIsRunning = false;
			}
		}

		//
		public void customUpdate()
		{
			if (!_coroutineIsRunning && _visibleQuadrants.Count > 0) {
				StartCoroutine("updateQuadrantVisibility");
			}
		}

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
		private IEnumerator updateQuadrantVisibility()
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
		}

		//
		private void createWorld() {

			resetWorld ();

			float fQuadrantSize = LevelEditor.Instance.fQuadrantSize;
			int count = 0;

			// create hollow cube of cubes :)
			int size = 2; // actual size will be size*2+1
			int height = 3;
			Vector3 pos = Vector3.zero;
			for (int x = -size; x <= size; ++x) {
				for (int y = -1; y <= height; ++y) {
					for (int z = -size; z <= size; ++z) {

						pos = new Vector3 (x * fQuadrantSize, y * fQuadrantSize, z * fQuadrantSize);

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

			MainMenu.Instance.setCubeCountText (_numCubes);
		}

		//
		private void resetWorld()
		{
			_numCubes = 0;

			foreach (Transform child in LevelEditor.Instance.goWorld.transform) {
				Destroy (child.gameObject);
			}

			_visibleQuadrants.Clear ();
			_aQuadrantChangedVisibility.Clear ();

			for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_quadrantFlags [x] [y] [z] = 0;
					}
				}
			}
		}

		//
		public void createRockCube (Vector3 v3CubePos, bool fillQuadrant = true)
		{
			// cube already created at that position
			if (_quadrantFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] == 1) {
				return;
			}

			GameObject cubeParent = createQuadrant (v3CubePos);
			GameObject container = createContainer (cubeParent.transform);

			if (!fillQuadrant) {
				return;
			}

			float fRockSize = LevelEditor.Instance.fRockSize;

			Vector3 pos = Vector3.zero;

			int len = LevelEditor.Instance.cubesPerQuadrant;
			float startPos = 0;
			string sName = "";

			pos.x = startPos;
			for (int x = 0; x < len; ++x) {
				pos.y = startPos;
				for (int y = 0; y < len; ++y) {
					pos.z = startPos;
					for (int z = 0; z < len; ++z) {
						sName = "r-" + x.ToString () + "-" + y.ToString () + "-" + z.ToString ();
						createRock (pos, container, sName);
						pos.z += fRockSize;
					}
					pos.y += fRockSize;
				}
				pos.x += fRockSize;
			}
		}

		//
		public GameObject createQuadrant(Vector3 v3CubePos)
		{
			string sPos = v3CubePos.x.ToString () + "_" + v3CubePos.y.ToString () + "_" + v3CubePos.z.ToString ();

			GameObject quadrant = new GameObject(Globals.containerGameObjectPrepend + sPos);
			quadrant.transform.SetParent(LevelEditor.Instance.goWorld.transform);
			quadrant.transform.localPosition = v3CubePos;

			if (LevelEditor.Instance.cubePrefabCenter != null) {
				GameObject go = GameObject.Instantiate(LevelEditor.Instance.cubePrefabCenter);
				go.name = "center_" + sPos;
				go.transform.SetParent(quadrant.transform);
				go.transform.localPosition = new Vector3(0.25f, 0.25f, 0.25f);
				Block blockScript = go.AddComponent<Block> ();
				blockScript.init ();
			}

			_quadrantFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] = 1;

			_visibleQuadrants.Add (quadrant, true);

			return quadrant;
		}

		public GameObject createContainer(Transform parent) {
			
			GameObject container = new GameObject ();
			container.name = "container";
			container.transform.SetParent (parent);
			container.transform.localPosition = Vector3.zero;

			return container;
		}

		//
		public GameObject createRock(Vector3 pos, GameObject parent, string name, Material material = null) {

			GameObject go = null;
			GameObject prefab = null;
			Vector3 rotation = Vector3.zero;

			prefab = LevelEditor.Instance.cubePrefab;//(_cubeIndex == 0 ? cubePrefab : cubePrefab2);
			if (prefab) {
				go = GameObject.Instantiate(prefab);
				go.name = name;
				go.transform.SetParent(parent.transform);
				go.transform.localPosition = pos;
				go.transform.localRotation = Quaternion.Euler (rotation);
				if (material != null) {
					go.GetComponent<MeshRenderer> ().material = material;
				} else {
					go.GetComponent<MeshRenderer> ().material = LevelEditor.Instance.materialsWalls [UnityEngine.Random.Range (0, LevelEditor.Instance.materialsWalls.Count)];
				}
				_numCubes++;
			}

			return go;
		}
	}
}