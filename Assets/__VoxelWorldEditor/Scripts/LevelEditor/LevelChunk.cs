//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AssetsShared;

namespace DragginzVoxelWorldEditor
{
	public class LevelChunk : MonoSingleton<LevelChunk>
	{
		private Transform _trfmPlaceholder;
		private Transform _trfmCubes;
		private Transform _trfmProps;

		private int _levelId;

		private LevelEditor _levelEditor;
		private AssetFactory _assetFactory;

		private Dictionary<string, int> _quadrantFlags;

		private Dictionary<GameObject, worldProp> _worldProps;

		private int _numCubes;

		private Vector3 _chunkPos;
		private Bounds _chunkBounds;

		private Vector3 _startPos;
		private Vector3 _startRotation;

		private bool _isVisible;

		#region Getters

		public Transform trfmCubes {
			get { return _trfmCubes; }
		}

		public Transform trfmProps {
			get { return _trfmProps; }
		}

		public Vector3 chunkPos {
			get { return _chunkPos; }
		}

		public Bounds chunkBounds {
			get { return _chunkBounds; }
		}

		public int levelId {
			get { return _levelId; }
		}

		public Dictionary<GameObject, worldProp> worldProps {
			get { return _worldProps; }
		}

		public int numCubes {
			get { return _numCubes; }
			set { _numCubes = value; }
		}

		#endregion

		#region SystemMethods

		void Awake()
		{
			_trfmPlaceholder = transform.Find ("placeholderContainer");
			_trfmCubes = transform.Find ("cubesContainer");
			_trfmProps = transform.Find ("propsContainer");
		}

		#endregion

		public void init(Vector3 chunkPos)
		{
			_chunkPos = chunkPos;

			_isVisible = false;

			_levelId = -1;

			_levelEditor = LevelEditor.Instance;
			_assetFactory = AssetFactory.Instance;

			_quadrantFlags = new Dictionary<string, int> ();

			_worldProps = new Dictionary<GameObject, worldProp> ();

			_numCubes = 0;
		}

		//
		public void setLevelData(LevelStruct level)
		{
			_levelId = level.id;

			GameObject levelChunk = _assetFactory.createLevelChunkClone ();
			levelChunk.transform.parent = _trfmPlaceholder;
			levelChunk.transform.localScale = new Vector3 (Globals.LEVEL_WIDTH, Globals.LEVEL_HEIGHT, Globals.LEVEL_DEPTH);
			levelChunk.transform.localPosition = new Vector3 (Globals.LEVEL_WIDTH / 2, Globals.LEVEL_HEIGHT / 2, Globals.LEVEL_DEPTH / 2);

			_chunkBounds = levelChunk.GetComponent<BoxCollider>().bounds;
			Debug.Log ("bounds for level id " + _levelId + ": " + _chunkBounds);

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
		}

		//
		public void setStartPos(Vector3 pos, Vector3 rot)
		{
			_startPos = pos;
			_startRotation = rot;
		}

		public Vector3 getStartPos()
		{
			return _chunkPos + _startPos;
		}

		public Vector3 getStartRotation()
		{
			return _startRotation;
		}

		//
		public void reset()
		{
			foreach (Transform child in _trfmCubes) {
				Destroy (child.gameObject);
			}

			foreach (Transform child in _trfmProps) {
				Destroy (child.gameObject);
			}

			_worldProps.Clear ();

			_quadrantFlags.Clear ();

			_numCubes = 0;
		}

		//
		public void activate(bool state, bool forceUpdate = false)
		{
			if (state) {
				if (forceUpdate || !_isVisible) {
					_isVisible = true;
					showPlaceHolder (false);
					showLevel (true);
				}
			}
			else {
				if (forceUpdate || _isVisible) {
					_isVisible = false;
					showPlaceHolder (true);
					showLevel (false);
				}
			}
		}

		//
		private void showPlaceHolder(bool state)
		{
			_trfmPlaceholder.gameObject.SetActive (state);
		}

		//
		private void showLevel(bool state)
		{
			_trfmCubes.gameObject.SetActive (state);
			_trfmProps.gameObject.SetActive (state);
		}

		//
		public void createOfflineLevel() {

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
			Debug.Log ("Offline Level - quadrants: "+count.ToString());
			Debug.Log ("Offline Level - cubes: "+_numCubes.ToString());

			MainMenu.Instance.setCubeCountText (_numCubes);
		}

		#region Cubes

		public void createRockCube (Vector3 v3CubePos, bool fillQuadrant = true)
		{
			int qX = (int)v3CubePos.x;
			int qY = (int)v3CubePos.y;
			int qZ = (int)v3CubePos.z;

			// out of level bounds?
			if (qX <= -1 || qY <= -1 || qZ <= -1) {
				return;
			}
			else if (qX >= Globals.LEVEL_WIDTH || qY >= Globals.LEVEL_HEIGHT || qZ >= Globals.LEVEL_DEPTH) {
				return;
			}

			string quadrantId = qX.ToString() + "_" + qY.ToString() + "_" + qZ.ToString();

			bool isEdgeQuadrant = ((qX == -1 || qY == -1 || qZ == -1) || (qX == Globals.LEVEL_WIDTH || qY == Globals.LEVEL_HEIGHT || qZ == Globals.LEVEL_DEPTH));
			if (isEdgeQuadrant) {
				Debug.Log (quadrantId + ":isEdgeQuadrant: " + isEdgeQuadrant);
			}

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
				//Debug.Log ("whatwhatwhat?");
				return null;
			}

			GameObject quadrant = _assetFactory.createQuadrantClone ();
			quadrant.name = Globals.containerGameObjectPrepend + quadrantId;
			quadrant.transform.SetParent(_trfmCubes);
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

		#endregion

		#region Props

		public void addWorldProp(int id, GameObject go)
		{
			_worldProps.Add (go, new worldProp (id, go.name, go));
		}

		//
		public void removeWorldProp(GameObject go)
		{
			if (_worldProps.ContainsKey (go)) {
				_worldProps.Remove (go);
			}
		}

		public propDef getPropDefForGameObject(GameObject go)
		{
			propDef p = new propDef();
			p.id = -1;

			if (_worldProps.ContainsKey (go)) {
				p = PropsManager.Instance.getPropDefForId(_worldProps [go].id);
			}

			return p;
		}

		#endregion
	}
}