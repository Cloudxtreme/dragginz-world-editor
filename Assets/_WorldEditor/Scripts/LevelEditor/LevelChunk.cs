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
	public class LevelChunk : MonoSingleton<LevelChunk>
	{
		private int _levelId;

		private LevelEditor _levelEditor;
		private AssetFactory _assetFactory;

		private Dictionary<string, int> _quadrantFlags;

		private int _numCubes;

		#region Getters

		public int numCubes {
			get { return _numCubes; }
			set { _numCubes = value; }
		}

		#endregion

		public void init(LevelStruct level)
		{
			_levelId = level.id;

			_levelEditor = LevelEditor.Instance;
			_assetFactory = AssetFactory.Instance;

			_quadrantFlags = new Dictionary<string, int> ();

			_numCubes = 0;

			GameObject levelChunk = _assetFactory.createLevelChunkClone ();
			levelChunk.transform.parent = transform;
			levelChunk.transform.localScale = new Vector3 (Globals.LEVEL_WIDTH, Globals.LEVEL_HEIGHT, Globals.LEVEL_DEPTH);
			levelChunk.transform.position = new Vector3 (level.x * Globals.LEVEL_WIDTH, -level.y * Globals.LEVEL_HEIGHT, level.z * Globals.LEVEL_DEPTH);

			int i;
			Transform txt;
			TextMesh textMesh;
			for (i = 1; i <= 6; ++i) {
				txt = levelChunk.transform.Find ("txt" + i.ToString ());
				if (txt != null) {
					textMesh = txt.GetComponent<TextMesh> ();
					if (textMesh != null) {
						textMesh.text = "Level id: " + _levelId.ToString () + "\n" + "'" + level.name + "'";
					}
				}
			}

			//LevelManager.Instance.loadLevelByIndex (_levelId);
		}

		//
		public void resetAll() {

			resetWorld();
		}

		public void createEmptyLevel() {
			
			createEmptyWorld ();
		}

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

			_quadrantFlags.Clear ();
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

			bool isEdgeQuadrant = ((qX == -1 || qY == -1 || qZ == -1) || (qX == Globals.LEVEL_WIDTH || qY == Globals.LEVEL_HEIGHT || qZ == Globals.LEVEL_DEPTH));
			//Debug.Log (quadrantId + ":isEdgeQuadrant: " + isEdgeQuadrant);

			GameObject cubeParent = createQuadrant (v3CubePos, quadrantId);
			if (cubeParent == null) {
				return;
			}

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
		}

		//
		public GameObject createQuadrant(Vector3 v3CubePos, string quadrantId)
		{
			// cube already created at that position
			if (_quadrantFlags.ContainsKey (quadrantId)) {
				Debug.Log ("whatwhatwhat?");
				return null;
			}

			GameObject quadrant = _assetFactory.createQuadrantClone ();
			quadrant.name = Globals.containerGameObjectPrepend + quadrantId;
			quadrant.transform.SetParent(_levelEditor.goWorld.transform);
			quadrant.transform.localPosition = v3CubePos;
			quadrant.isStatic = true;

			_numCubes += 8;
				
			_quadrantFlags.Add(quadrantId, 1);

			return quadrant;
		}

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