//
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
		public GameObject goLights;

        public Transform playerEdit;
		public Transform playerPlay;

        [SerializeField]
        public Vector3Int levelSize;
        public float gridSize;

        public Transform trfmWalls;

		public Material matElementMarker;

		public GizmoTranslateScript gizmoTranslateScript;
		public GizmoRotateScript gizmoRotateScript;

		//

		public enum EditMode {
			None,
			Place,
			Transform,
			Play
		};

		public enum AssetType {
			Floor,
			Wall,
			Tunnel,
			Chunk,
			Prop,
			Dungeon
		};

		public enum PartList {
			Floor_1,
			Floor_2,
			Floor_3,
			Wall_Z,
			Wall_X,
			Path_1,
			Path_2,
			Path_3,
			Path_4,
			Pillar_1,
			Pillar_2,
			Pillar_3,
			Chunk_Rock_1,
			Chunk_Rock_2,
			Chunk_Stalagmite_1,
			Chunk_Stalagmite_2,
			Chunk_Stalagmite_3,
			Chunk_Cliff_1,
			Chunk_Cliff_2,
			Chunk_WallEdge,
			Chunk_LargeBricks,
			Chunk_Block,
			Chunk_Corner,
			Chunk_Base,
			Prop_BonePile,
			Prop_Debris,
			Prop_Grave_1,
			Prop_TombStone,
			Dungeon_Floor,
			Dungeon_Wall_L,
			Dungeon_Wall_LR,
			Dungeon_Corner,
			Dungeon_DeadEnd,
			Dungeon_Turn,
			Dungeon_T,
			Dungeon_Stairs_1,
			Dungeon_Stairs_2,
			Dungeon_Ramp_1,
			Dungeon_Ramp_2,
			Dungeon_Wall_L_NF,
			Dungeon_Corner_NF,
			End_Of_List
		};

		public struct Part
        {
			public PartList id;
			public AssetType type;
            public GameObject prefab;

            public float w;
            public float h;
            public float d;

			public Vector3Int canRotate;
			public bool usesGravity;

			public string name;
			public string extra;
        }

		public struct LevelElement
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
		private Dictionary<string, LevelElement> _levelElements;
		private int _iCounter;

		private Part _curEditPart;
        private GameObject _goEditPart;
        private Vector3 _v3EditPartPos;

		private LevelElement _selectedElement;
		private Bounds _selectedElementBounds;
		private List<MeshRenderer> _selectedMeshRenderers;

		private Dictionary<AssetType, List<Part>> _assetTypeList;
		private Dictionary<AssetType, int> _assetTypeIndex;

		private List<GameObject> _listOfChildren;

		private List<PlacementTool> _aPlacementTools;
		private PlacementTool _curPlacementTool;

		private List<DungeonTool> _aDungeonTools;
		private DungeonTool _curDungeonTool;

        private float _rayDistance;
        private Ray _ray;
		private LayerMask _layermask;
        private RaycastHit _hit;
        private GameObject _goHit;

		private AssetType _assetType;
		private EditMode  _editMode;

        private float _timer;
        private float _lastMouseWheelUpdate;

		#region Getters

		public GameObject container {
			get { return _container.gameObject; }
		}

		public EditMode editMode {
			get { return _editMode; }
		}

		public Dictionary<PartList, Part> parts {
			get { return _parts; }
		}

		public Bounds selectedElementBounds {
			get { return _selectedElementBounds; }
		}

		#endregion

		// ------------------------------------------------------------------------
		// System Methods
		// ------------------------------------------------------------------------
        void Start()
        {
			_parts = new Dictionary<PartList, Part>();

			createPart(PartList.Floor_1,  AssetType.Floor, "MDC/Floors/Floor_1",  4.00f,  0.10f,  4.00f, Vector3Int.zero, false, "Floor 1");
			createPart(PartList.Floor_2,  AssetType.Floor, "MDC/Floors/Floor_2",  6.00f,  0.10f,  6.00f, Vector3Int.zero, false, "Floor 2");
			createPart(PartList.Floor_3,  AssetType.Floor, "MDC/Floors/Floor_3",  6.00f,  0.25f,  6.00f, Vector3Int.zero, false, "Floor 3");

			createPart(PartList.Wall_Z,   AssetType.Wall,  "MDC/WallsZ/Wall_Z",   3.00f,  3.00f,  0.50f, Vector3Int.zero, false, "Wall Left",  "Z");
			createPart(PartList.Wall_X,   AssetType.Wall,  "MDC/WallsX/Wall_X",   0.50f,  3.00f,  3.00f, Vector3Int.zero, false, "Wall Right", "X");

			createPart(PartList.Chunk_Rock_1,       AssetType.Chunk, "MDC/Chunks/Chunk_Rock_1",        4.00f,  3.50f,  4.00f, Vector3Int.one, false, "Rock 1");
			createPart(PartList.Chunk_Rock_2,       AssetType.Chunk, "MDC/Chunks/Chunk_Rock_2",        4.00f,  2.40f,  4.00f, Vector3Int.one, false, "Rock 2");
			createPart(PartList.Chunk_Stalagmite_1, AssetType.Chunk, "MDC/Chunks/Chunk_Stalagmite_1",  2.75f,  4.50f,  2.75f, Vector3Int.one, false, "Stalagmite 1");
			createPart(PartList.Chunk_Stalagmite_2, AssetType.Chunk, "MDC/Chunks/Chunk_Stalagmite_2",  4.30f,  6.00f,  3.60f, Vector3Int.one, false, "Stalagmite 2");
			createPart(PartList.Chunk_Stalagmite_3, AssetType.Chunk, "MDC/Chunks/Chunk_Stalagmite_3",  7.25f,  8.80f,  6.25f, Vector3Int.one, false, "Stalagmite 3");
			createPart(PartList.Chunk_Cliff_1,      AssetType.Chunk, "MDC/Chunks/Chunk_Cliff_1",       8.00f,  8.00f,  4.00f, Vector3Int.one, false, "Cliff 1");
			createPart(PartList.Chunk_Cliff_2,      AssetType.Chunk, "MDC/Chunks/Chunk_Cliff_2",      10.00f,  8.00f,  7.00f, Vector3Int.one, false, "Cliff 2");

			createPart(PartList.Chunk_WallEdge,     AssetType.Chunk, "MDC/Chunks/Chunk_WallEdge",      0.25f,  3.00f,  0.30f, Vector3Int.one, false, "Wall Edge");
			createPart(PartList.Chunk_LargeBricks,  AssetType.Chunk, "MDC/Chunks/Chunk_LargeBricks",   6.00f,  0.75f,  0.75f, Vector3Int.one, false, "Large Bricks");
			createPart(PartList.Chunk_Block,        AssetType.Chunk, "MDC/Chunks/Chunk_Block",         2.00f,  0.75f,  2.00f, Vector3Int.one, false, "Weird Block");
			createPart(PartList.Chunk_Corner,       AssetType.Chunk, "MDC/Chunks/Chunk_Corner",        4.00f,  2.00f,  4.00f, Vector3Int.one, false, "Corner Chunk");
			createPart(PartList.Chunk_Base,         AssetType.Chunk, "MDC/Chunks/Chunk_Base",          4.00f,  2.00f,  4.00f, Vector3Int.one, false, "Rounded Base");

			createPart(PartList.Prop_BonePile, AssetType.Prop, "MDC/Props/Prop_BonePile",  2.00f,  0.75f,  2.00f, Vector3Int.one,  false, "Bone Pile");
			createPart(PartList.Prop_Debris,   AssetType.Prop, "MDC/Props/Prop_Debris",    3.30f,  1.20f,  3.70f, Vector3Int.one,  false, "Debris");

			// Gravity Props
			createPart(PartList.Prop_Grave_1,   AssetType.Prop, "MDC/Props/Prop_Grave_1",    1.00f,  0.88f,  3.00f, Vector3Int.one,  true, "Grave");
			createPart(PartList.Prop_TombStone, AssetType.Prop, "MDC/Props/Prop_TombStone",  6.00f,  3.20f,  0.25f, Vector3Int.one,  true, "Tomb Stone");

			createPart(PartList.Pillar_1,     AssetType.Prop, "MDC/Props/Pillar_1",      2.00f,  3.00f,  2.00f, Vector3Int.one,  true, "Pillar 1");
			createPart(PartList.Pillar_2,     AssetType.Prop, "MDC/Props/Pillar_2",      1.50f,  1.50f,  4.75f, Vector3Int.one,  true, "Pillar 2");
			createPart(PartList.Pillar_3,     AssetType.Prop, "MDC/Props/Pillar_3",      1.50f,  1.50f,  1.50f, Vector3Int.one,  true, "Pillar Base");

			// Dungeons
			createPart(PartList.Dungeon_Floor,     AssetType.Dungeon, "Dungeons/Dungeon_Floor",     2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Floor");
			createPart(PartList.Dungeon_Wall_L,    AssetType.Dungeon, "Dungeons/Dungeon_Wall_L",    2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Wall");
			createPart(PartList.Dungeon_Wall_LR,   AssetType.Dungeon, "Dungeons/Dungeon_Wall_LR",   2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Walls");
			createPart(PartList.Dungeon_Corner,    AssetType.Dungeon, "Dungeons/Dungeon_Corner",    2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Corner");
			createPart(PartList.Dungeon_DeadEnd,   AssetType.Dungeon, "Dungeons/Dungeon_DeadEnd",   2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Dead End");
			createPart(PartList.Dungeon_Turn,      AssetType.Dungeon, "Dungeons/Dungeon_Turn",      2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Turn");
			createPart(PartList.Dungeon_T,         AssetType.Dungeon, "Dungeons/Dungeon_T",         2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon T Intersection");
			createPart(PartList.Dungeon_Stairs_1,  AssetType.Dungeon, "Dungeons/Dungeon_Stairs_1",  2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Stairs Lower");
			createPart(PartList.Dungeon_Stairs_2,  AssetType.Dungeon, "Dungeons/Dungeon_Stairs_2",  2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Stairs Upper");
			createPart(PartList.Dungeon_Ramp_1,    AssetType.Dungeon, "Dungeons/Dungeon_Ramp_1",    2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Ramp Lower");
			createPart(PartList.Dungeon_Ramp_2,    AssetType.Dungeon, "Dungeons/Dungeon_Ramp_2",    2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Ramp Upper");
			createPart(PartList.Dungeon_Wall_L_NF, AssetType.Dungeon, "Dungeons/Dungeon_Wall_L_NF", 2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Wall No Floor");
			createPart(PartList.Dungeon_Corner_NF, AssetType.Dungeon, "Dungeons/Dungeon_Corner_NF", 2.00f, 2.00f, 2.00f, Vector3Int.one, false, "Dungeon Corner No Floor");

			//

			createAssetTypeCount ();

            _container = new GameObject("[Container]").transform;

            setWalls();

			_levelElements = new Dictionary<string, LevelElement>();
			_iCounter = 0;

			_listOfChildren = new List<GameObject> ();

			_layermask = 1 << 12;

            _v3EditPartPos = Vector3.zero;

			_selectedMeshRenderers = new List<MeshRenderer> ();
			resetSelectedElement ();

			_assetType = AssetType.Floor;
			_editMode = EditMode.None;

			GameObject toolContainer = new GameObject("[ContainerTools]");
			_aPlacementTools = new List<PlacementTool> ();
			_aPlacementTools.Add (new PlacementToolCircle (toolContainer));
			_aPlacementTools.Add (new PlacementToolQuad (toolContainer));
			_aPlacementTools.Add (new PlacementToolMount (toolContainer));
			_aPlacementTools.Add (new PlacementToolCube (toolContainer));

			_aDungeonTools = new List<DungeonTool> ();
			_aDungeonTools.Add (new DungeonToolRoom (toolContainer));
			_aDungeonTools.Add (new DungeonToolMaze (toolContainer));
			_aDungeonTools.Add (new DungeonToolRandom (toolContainer));
			_aDungeonTools.Add (new DungeonToolStaircase (toolContainer));

			PweMainMenu.Instance.init ();
			PwePlacementTools.Instance.init ();
			PweDungeonTools.Instance.init ();

			setNewEditPart(_assetTypeList[_assetType][0]);

			setEditMode (EditMode.Place);
        }

		// ------------------------------------------------------------------------
		void Update()
		{
			_timer = Time.realtimeSinceStartup;

			if (_editMode == EditMode.Play)
			{
				updatePlayMode ();
			}
			else
			{
				updateEditMode ();

				if (_selectedElement.part != PartList.End_Of_List) {
					if (_selectedMeshRenderers.Count > 0) {
						_selectedElementBounds = _selectedMeshRenderers [0].bounds;
						int i, len = _selectedMeshRenderers.Count;
						for (i = 1; i < len; ++i) {
							_selectedElementBounds.Encapsulate (_selectedMeshRenderers [i].bounds);
						}
						GLTools.drawBoundingBox (_selectedElementBounds, matElementMarker);
					}
				}
			}
		}

		// ------------------------------------------------------------------------
		// Public Methods
		// ------------------------------------------------------------------------
		public void setEditMode(EditMode mode)
		{
			if (mode != _editMode) {

				_editMode = mode;

				goLights.SetActive (_editMode != EditMode.Play);

				resetElementComponents ();
				resetSelectedElement ();
				resetCurPlacementTool ();
				resetCurDungeonTool ();

				PweMainMenu.Instance.setModeButtons (_editMode);
				PweMainMenu.Instance.showTransformBox (_editMode == EditMode.Transform);
				PweMainMenu.Instance.showAssetTypeBox (_editMode == EditMode.Place);
				PweMainMenu.Instance.showPlacementToolBox (_editMode == EditMode.Place && _assetType != AssetType.Dungeon);
				PweMainMenu.Instance.showDungeonToolBox (_editMode == EditMode.Place && _assetType == AssetType.Dungeon);

				if (_editMode == EditMode.Place) {
					PweMainMenu.Instance.setAssetTypeButtons (_assetType);
					showAssetInfo (_curEditPart);
				} else {
					PweMainMenu.Instance.setAssetNameText ("");
					PweMainMenu.Instance.setSpecialHelpText ("");
				}
				PweMainMenu.Instance.showAssetInfoPanel (_editMode == EditMode.Place);

				playerEdit.gameObject.SetActive (_editMode != EditMode.Play);
				playerPlay.gameObject.SetActive (!playerEdit.gameObject.activeSelf);

				_goEditPart.SetActive (_editMode == EditMode.Place);
				setMarkerActive (_goEditPart.activeSelf);

				if (_goEditPart.activeSelf) {
					setMarkerScale (_curEditPart);
				}

				// Instructions
				if (_editMode == EditMode.Place) {
					PweMainMenu.Instance.setInstructionsText ("Use Mousewheel to toggle through assets");
				} else if (_editMode == EditMode.Play) {
					PweMainMenu.Instance.setInstructionsText ("Press Esc to exit play mode");
				} else if (_editMode == EditMode.Transform) {
					PweMainMenu.Instance.setInstructionsText ("Click object to select");
				} else {
					PweMainMenu.Instance.setInstructionsText ("");
				}
			}
		}

		// ------------------------------------------------------------------------
		public void selectTransformTool(int toolId)
		{
			gizmoTranslateScript.gameObject.SetActive (toolId == 0);
			gizmoRotateScript.gameObject.SetActive (toolId == 1);

			if (gizmoTranslateScript.gameObject.activeSelf && gizmoTranslateScript.translateTarget == null) {
				gizmoTranslateScript.gameObject.SetActive (false);
			}
			if (gizmoRotateScript.gameObject.activeSelf && gizmoRotateScript.rotateTarget == null) {
				gizmoRotateScript.gameObject.SetActive (false);
			}
		}

		// ------------------------------------------------------------------------
		public void selectAssetType(AssetType type)
		{
			_assetType = type;

			PweMainMenu.Instance.showPlacementToolBox (_editMode == EditMode.Place && _assetType != AssetType.Dungeon);
			PweMainMenu.Instance.showDungeonToolBox (_editMode == EditMode.Place && _assetType == AssetType.Dungeon);

			resetCurPlacementTool ();
			resetCurDungeonTool ();

			int index = _assetTypeIndex [_assetType];
			setNewEditPart(_assetTypeList[_assetType][index]);
		}

		// ------------------------------------------------------------------------
		public void selectPlacementTool(PlacementTool.PlacementMode mode)
		{
			if (_curPlacementTool == null || _curPlacementTool.placementMode != mode)
			{
				if (_editMode == EditMode.Transform)
				{
					if (_selectedElement.go != null) {

						activatePlacementTool (mode, _selectedElement.go.transform.position, _parts[_selectedElement.part]);
					}
				}
				else if (_goEditPart != null)
				{
					if (_curEditPart.type != AssetType.Floor && _curEditPart.type != AssetType.Wall)
					{
						activatePlacementTool (mode, _goEditPart.transform.position, _curEditPart);
					}
				}
			}
		}

		// ------------------------------------------------------------------------
		public void placementToolValueChange(int valueId, int value)
		{
			if (_curPlacementTool != null) {
				_curPlacementTool.update (valueId, value);
			}
		}

		// ------------------------------------------------------------------------
		public void dungeonToolValueChange(int valueId, int value)
		{
			if (_curDungeonTool != null) {
				_curDungeonTool.update (valueId, value);
			}
		}

		// ------------------------------------------------------------------------
		public void selectDungeonTool(DungeonTool.DungeonPreset preset)
		{
			if (_curDungeonTool == null || _curDungeonTool.dungeonPreset != preset)
			{
				if (_editMode == EditMode.Place)
				{
					if (_curEditPart.type == AssetType.Dungeon)
					{
						setNewEditPart(_assetTypeList[_assetType][0]); // reset to floor tile
						activateDungeonTool (preset, _goEditPart.transform.position, _curEditPart);
					}
				}
			}
		}

		// ------------------------------------------------------------------------
		// Private Methods
		// ------------------------------------------------------------------------
		private void updatePlayMode()
		{
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (PweMainMenu.Instance.popup.isVisible ()) {
					PweMainMenu.Instance.popup.hide ();
				} else {
					setEditMode (EditMode.Transform);
				}
			}
		}

		// ------------------------------------------------------------------------
		private void updateEditMode()
		{
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (PweMainMenu.Instance.popup.isVisible ()) {
					PweMainMenu.Instance.popup.hide ();
				} else if (_curPlacementTool != null) {
					resetCurPlacementTool ();
				} else if (_curDungeonTool != null) {
					resetCurDungeonTool ();
				}
				else {
					setEditMode (EditMode.Transform);
				}
			}
			else if (Input.GetKeyDown (KeyCode.P)) {
				setEditMode (EditMode.Play);
			}

			if (Camera.main == null) {
				return;
			}

			_rayDistance = 5f;
			_goHit = null;
			_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_ray, out _hit, 100)) {
				_goHit = _hit.collider.gameObject;
				_rayDistance = _hit.distance;
			}

			if (_editMode == EditMode.Transform)
			{
				if (Input.GetMouseButtonDown (0))
				{
					if (!EventSystem.current.IsPointerOverGameObject ()) {
						if (_goHit != null) {
							selectElement (_goHit.transform); //.parent);
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
			}
			else if (_editMode == EditMode.Place)
			{
				positionEditPart ();
			}
		}

		// ------------------------------------------------------------------------
		private void resetCurPlacementTool()
		{
			if (_curPlacementTool != null) {
				_curPlacementTool.reset ();
				_curPlacementTool = null;
			}
			PweMainMenu.Instance.setPlacementToolButtons (PlacementTool.PlacementMode.None);
			PwePlacementTools.Instance.showToolPanels (PlacementTool.PlacementMode.None);

			showAssetInfo (_curEditPart);
		}

		// ------------------------------------------------------------------------
		private void activatePlacementTool(PlacementTool.PlacementMode mode, Vector3 pos, Part part)
		{
			if (mode == PlacementTool.PlacementMode.Circle) {
				_curPlacementTool = _aPlacementTools [0];
			} else if (mode == PlacementTool.PlacementMode.Quad) {
				_curPlacementTool = _aPlacementTools [1];
			} else if (mode == PlacementTool.PlacementMode.Mount) {
				_curPlacementTool = _aPlacementTools [2];
			} else {
				_curPlacementTool = _aPlacementTools [3];
			}

			PwePlacementTools.Instance.reset ();
			PwePlacementTools.Instance.showToolPanels (mode);

			_curPlacementTool.activate (mode, pos, part);

			showAssetInfo (part);
		}

		// ------------------------------------------------------------------------
		private void resetCurDungeonTool()
		{
			if (_curDungeonTool != null) {
				_curDungeonTool.reset ();
				_curDungeonTool = null;
			}
			PweMainMenu.Instance.setDungeonToolButtons (DungeonTool.DungeonPreset.None);
			PweDungeonTools.Instance.showToolPanels (DungeonTool.DungeonPreset.None);

			showAssetInfo (_curEditPart);
		}

		// ------------------------------------------------------------------------
		private void activateDungeonTool(DungeonTool.DungeonPreset preset, Vector3 pos, Part part)
		{
			if (preset == DungeonTool.DungeonPreset.Room) {
				_curDungeonTool = _aDungeonTools [0];
			}
			else if (preset == DungeonTool.DungeonPreset.Maze) {
				_curDungeonTool = _aDungeonTools [1];
			}
			else if (preset == DungeonTool.DungeonPreset.Random) {
				_curDungeonTool = _aDungeonTools [2];
			}
			else if (preset == DungeonTool.DungeonPreset.Staircase) {
				_curDungeonTool = _aDungeonTools [3];
			}

			PweDungeonTools.Instance.reset ();
			PweDungeonTools.Instance.showToolPanels (preset);

			_curDungeonTool.activate (preset, pos, part);

			showAssetInfo (part);
		}

		// ------------------------------------------------------------------------
		private void positionEditPart()
		{
			//
			// Positioning
			//
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

			//
			// Tools
			//
			if (_curPlacementTool != null) {
				if (_editMode == EditMode.Transform && _selectedElement.go != null) {
					_curPlacementTool.customUpdate (_selectedElement.go.transform.position);
				} else if (_goEditPart != null) {
					_curPlacementTool.customUpdate (_goEditPart.transform.position);
				}
			}
			else if (_curDungeonTool != null) {
				if (_goEditPart != null) {

					// check boundaries
					float half = _curDungeonTool.cubeSize / 2f;
					int xStart = _curDungeonTool.width / 2;
					if (_v3EditPartPos.x - (xStart * _curDungeonTool.cubeSize + half) < 0) {
						_v3EditPartPos.x = (xStart * _curDungeonTool.cubeSize + half);
					}
					int zStart = _curDungeonTool.depth / 2;
					if (_v3EditPartPos.z - (zStart * _curDungeonTool.cubeSize + half) < 0) {
						_v3EditPartPos.z = (zStart * _curDungeonTool.cubeSize + half);
					}
					_goEditPart.transform.position = _v3EditPartPos;
					setMarkerPosition (_goEditPart.transform);

					_curDungeonTool.customUpdate (_goEditPart.transform.position);
				}
			}

			//
			// Mousewheel
			//
			if (Input.GetAxis ("Mouse ScrollWheel") != 0)
			{
				if (_timer > _lastMouseWheelUpdate)
				{
					_lastMouseWheelUpdate = _timer + 0.2f;
					float dir = (Input.GetAxis ("Mouse ScrollWheel") > 0 ? 1 : -1);

					if (_curDungeonTool != null) {
					
						if (Input.GetKey (KeyCode.Alpha1)) {
							PweDungeonTools.Instance.updateWidthValue ((int)dir, _curDungeonTool.dungeonPreset);
						} else if (Input.GetKey (KeyCode.Alpha2)) {
							PweDungeonTools.Instance.updateDepthValue ((int)dir, _curDungeonTool.dungeonPreset);
						} else if (Input.GetKey (KeyCode.Alpha3)) {
							PweDungeonTools.Instance.updateHeightValue ((int)dir, _curDungeonTool.dungeonPreset);
						}
					}
					else if (_curPlacementTool != null) {
						
						if (Input.GetKey (KeyCode.Alpha1)) {
							PwePlacementTools.Instance.updateRadiusValue ((int)dir, _curPlacementTool.placementMode);
						}
						else if (Input.GetKey (KeyCode.Alpha2)) {
							PwePlacementTools.Instance.updateIntervalValue ((int)dir, _curPlacementTool.placementMode);
						}
						else if (Input.GetKey (KeyCode.Alpha3)) {
							PwePlacementTools.Instance.updateDensityValue ((int)dir, _curPlacementTool.placementMode);
						}
					}
					else {
						
						float multiply = (_curEditPart.type == AssetType.Dungeon ? 90f * dir : 15f * dir);

						if (Input.GetKey (KeyCode.X)) {
							if (_curEditPart.canRotate.x == 1) {
								_goEditPart.transform.Rotate (Vector3.right * multiply);
							}
						} else if (Input.GetKey (KeyCode.Y)) {
							if (_curEditPart.canRotate.y == 1) {
								_goEditPart.transform.Rotate (Vector3.up * multiply);
							}
						} else if (Input.GetKey (KeyCode.Z)) {
							if (_curEditPart.canRotate.z == 1) {
								_goEditPart.transform.Rotate (Vector3.forward * multiply);
							}
						}
						else if (Input.GetKey (KeyCode.C)) {
							Vector3 scale = _goEditPart.transform.localScale;
							scale += new Vector3 (.025f * dir, .025f * dir, .025f * dir);
							_goEditPart.transform.localScale = scale;
						} else {
							toggleEditPart ();
						}
					}
				}
			}

			if (Input.GetMouseButtonDown (0))
			{
				if (!EventSystem.current.IsPointerOverGameObject ()) {
					if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.type == AssetType.Floor) {
						fillY (_goEditPart.transform.position);
					} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.extra == "Z") {
						fillZ (_goEditPart.transform.position);
					} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && _curEditPart.extra == "X") {
						fillX (_goEditPart.transform.position);
					} else {
						placePart (_goEditPart.transform.position);
					}
				}
			}
        }

		// ------------------------------------------------------------------------
		private void selectElement(Transform trfmHit)
		{
			// gizmo?
			if (trfmHit.gameObject.layer == 11) {
				return;
			}

			Transform trfmParent = trfmHit;

			while (trfmParent.parent != null && trfmParent.tag != "PartContainer") {
				trfmParent = trfmParent.parent;
			}

			// not an asset?
			if (trfmParent.tag != "PartContainer") {
				resetElementComponents ();
				resetSelectedElement ();
				resetCurPlacementTool ();
				resetCurDungeonTool ();
				return;
			}

			if (_levelElements.ContainsKey (trfmParent.gameObject.name))
			{
				if (_selectedElement.go != trfmParent.gameObject)
				{
					resetElementComponents ();

					_selectedElement = _levelElements [trfmParent.gameObject.name];
					setMeshCollider (_selectedElement.go, false);
					setRigidBody (_selectedElement.go, false);

					getSelectedMeshRenderers (_selectedElement.go);

					Part part = _parts [_selectedElement.part];
					setMarkerScale (part);

					gizmoTranslateScript.translateTarget = _selectedElement.go;
					gizmoTranslateScript.init();
					gizmoRotateScript.rotateTarget = _selectedElement.go;
					gizmoRotateScript.init();

					selectTransformTool (PweMainMenu.Instance.iSelectedTool);

					PweMainMenu.Instance.showAssetInfoPanel (true);
					showAssetInfo (_parts [_selectedElement.part]);

					PweMainMenu.Instance.showPlacementToolBox (true);
					PweMainMenu.Instance.showDungeonToolBox (false);
				}
			}
			else
			{
				resetElementComponents ();
				resetSelectedElement ();
				resetCurPlacementTool ();
				resetCurDungeonTool ();
			}

			setMarkerActive (_selectedElement.go != null);
		}

		// ------------------------------------------------------------------------
		private void positionSelectedElement()
		{
			if (Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				if (_timer > _lastMouseWheelUpdate)
				{
					Part part = _parts [_selectedElement.part];

					_lastMouseWheelUpdate = _timer + 0.2f;
					float dir = (Input.GetAxis ("Mouse ScrollWheel") > 0 ? 1 : -1);
					float multiply = 15f * dir;

					if (Input.GetKey (KeyCode.X)) {
						if (part.canRotate.x == 1) {
							_selectedElement.go.transform.Rotate (Vector3.right * multiply);
						}
					}
					else if (Input.GetKey (KeyCode.Y)) {
						if (part.canRotate.y == 1) {
							_selectedElement.go.transform.Rotate (Vector3.up * multiply);
						}
					}
					else if (Input.GetKey (KeyCode.Z)) {
						if (part.canRotate.z == 1) {
							_selectedElement.go.transform.Rotate (Vector3.forward * multiply);
						}
					}
					// Scale
					else if (Input.GetKey (KeyCode.C)) {
						Vector3 scale = _selectedElement.go.transform.localScale;
						scale += new Vector3(.025f * dir, .025f * dir, .025f * dir);
						_selectedElement.go.transform.localScale = scale;
					}
				}
			}

			setMarkerPosition (_selectedElement.go.transform);
		}

		// ------------------------------------------------------------------------
		private void deleteSelectedElement()
		{
			if (_selectedElement.go != null) {
				if (_levelElements.ContainsKey (_selectedElement.go.name)) {
					_levelElements.Remove (_selectedElement.go.name);
				}
				Destroy (_selectedElement.go);
				_selectedElement.go = null;
			}

			PweMainMenu.Instance.setCubeCountText (_levelElements.Count);

			resetSelectedElement ();
			resetCurPlacementTool ();


			setMarkerActive (_goEditPart.activeSelf);
		}

		// ------------------------------------------------------------------------
        private void toggleEditPart()
		{
			float direction = Input.GetAxis ("Mouse ScrollWheel");

			int index = _assetTypeIndex [_assetType];
			int max = _assetTypeList [_assetType].Count;
						
			if (direction > 0) {
				if (++index >= max) {
					index = 0;
				}
			} else {
				if (--index < 0) {
					index = max - 1;
				}
			}
			_assetTypeIndex [_assetType] = index;

			setNewEditPart(_assetTypeList[_assetType][index]);
        }

		// ------------------------------------------------------------------------
		private void setNewEditPart(Part part)
		{
			_curEditPart = part;

			if (_goEditPart != null) {
				Destroy (_goEditPart);
			}
			_goEditPart = null;

			_goEditPart = createPartAt(_curEditPart.id, -10, -10, -10);
			setMarkerScale (_curEditPart);
			setMeshCollider(_goEditPart, false);

			showAssetInfo (_curEditPart);
		}

		// ------------------------------------------------------------------------
		private void showAssetInfo(Part part)
		{
			PweMainMenu.Instance.setAssetNameText ((_assetTypeIndex [_assetType]+1).ToString() + " / " + _assetTypeList [_assetType].Count.ToString());
			PweMainMenu.Instance.assetInfo.init (_curEditPart);

			string s = "";

			if (_assetType == AssetType.Floor) {
				s = "Press left mouse button + shift key for a 'Floor Fill'!";
			}else if (_assetType == AssetType.Wall) {
				s = "Press left mouse button + shift key for a 'Wall Fill'!";
			}
			else {
				s = "Mousewheel + ";
				if (_curDungeonTool != null) {
					s += "'1'/'2'/'3' = change preset settings";
				} else if (_curPlacementTool != null) {
					s += "'1'/'2'/'3' = change pattern settings";
				}
				else {
					if (part.canRotate != Vector3Int.zero) {
						s = "Mousewheel + ";
						s += (part.canRotate.x == 1 ? "'X'" : "");
						s += (part.canRotate.y == 1 ? "/ 'Y'" : "");
						s += (part.canRotate.z == 1 ? "/ 'Z'" : "");
						s += " = rotate object";
					}
					s += "\nMousewheel + 'C' = scale object";
				}
			}

			PweMainMenu.Instance.setSpecialHelpText (s);
		}

		// ------------------------------------------------------------------------
		//
		// ------------------------------------------------------------------------
		private void getSelectedMeshRenderers (GameObject go)
		{
			_selectedMeshRenderers.Clear ();

			_listOfChildren.Clear ();
			getChildrenRecursive (go);

			int i, len = _listOfChildren.Count;
			for (i = 0; i < len; ++i) {
				if (_listOfChildren [i].GetComponent<MeshRenderer> ()) {
					_selectedMeshRenderers.Add(_listOfChildren [i].GetComponent<MeshRenderer> ());
				}
			}
		}

		// ------------------------------------------------------------------------
		public void setMeshCollider (GameObject go, bool state) {

			_listOfChildren.Clear ();
			getChildrenRecursive (go);

			int i, len = _listOfChildren.Count;
			for (i = 0; i < len; ++i) {
				if (_listOfChildren [i].GetComponent<Collider> ()) {
					_listOfChildren [i].GetComponent<Collider> ().enabled = state;
				}
			}
        }

		// ------------------------------------------------------------------------
		private void setMeshColliders (bool state)
		{
			_listOfChildren.Clear ();

			foreach (KeyValuePair<string, LevelElement> element in _levelElements)
			{
				getChildrenRecursive (element.Value.go);
			}

			int i, len = _listOfChildren.Count;
			for (i = 0; i < len; ++i) {
				if (_listOfChildren [i].GetComponent<Collider> ()) {
					_listOfChildren [i].GetComponent<Collider> ().enabled = state;
				}
			}
		}

		private void getChildrenRecursive(GameObject go)
		{
			if (go == null) {
				return;
			}

			_listOfChildren.Add (go);

			foreach (Transform child in go.transform)
			{
				if (child != null) {
					_listOfChildren.Add (child.gameObject);
					getChildrenRecursive (child.gameObject);
				}
			}
		}

		// ------------------------------------------------------------------------
		public void setRigidBody (GameObject go, bool state) {

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

		// ------------------------------------------------------------------------
        private void placePart(Vector3 pos)
		{
			// Tools
			if (_curPlacementTool != null && _curPlacementTool.placementMode == PlacementTool.PlacementMode.Mount) {
				if (!_curPlacementTool.inverse) {
					pos.y += _curPlacementTool.interval * 0.5f;
				}
			}

			if (_curPlacementTool == null && _curDungeonTool == null)
			{
				LevelElement element = new LevelElement ();
				element.part = _curEditPart.id;
				element.go = createPartAt (_curEditPart.id, pos.x, pos.y, pos.z);
				element.go.transform.rotation = _goEditPart.transform.rotation;

				setMeshCollider (element.go, true);
				setRigidBody (element.go, _curEditPart.usesGravity);

				_levelElements.Add (element.go.name, element);
			}

			// add tool objects
			if (_curPlacementTool != null) {
				if (_curPlacementTool.placementMode != PlacementTool.PlacementMode.None) {
					placePattern ();
				}
				resetCurPlacementTool ();
			}
			else if (_curDungeonTool != null) {
				if (_curDungeonTool.dungeonPreset != DungeonTool.DungeonPreset.None) {
					placeDungeon ();
				}
				resetCurDungeonTool ();
			}

			PweMainMenu.Instance.setCubeCountText (_levelElements.Count);
        }

		// ------------------------------------------------------------------------
		private void placePattern()
		{
			int i, len = _curPlacementTool.elements.Count;
			for (i = 0; i < len; ++i) {

				GameObject go = _curPlacementTool.elements [i].go;
				if (go != null) {
					go.transform.SetParent (_container);
					go.name = "part_" + (_iCounter++).ToString ();

					setMeshCollider (go, true);
					setRigidBody (go, _curEditPart.usesGravity);

					LevelElement elementTool = new LevelElement ();
					elementTool.part = _curEditPart.id;
					elementTool.go = go;

					_levelElements.Add (go.name, elementTool);
				}
			}
		}

		// ------------------------------------------------------------------------
		private void placeDungeon()
		{
			int i, len = _curDungeonTool.dungeonElements.Count;
			for (i = 0; i < len; ++i) {

				GameObject go = _curDungeonTool.dungeonElements [i].go;
				if (go != null) {
					go.transform.SetParent (_container);
					go.name = "part_" + (_iCounter++).ToString ();

					setMeshCollider (go, true);
					setRigidBody (go, false);

					LevelElement elementTool = new LevelElement ();
					elementTool.part = _curDungeonTool.dungeonElements [i].part;
					elementTool.go = go;

					_levelElements.Add (go.name, elementTool);
				}
			}
		}

		// ------------------------------------------------------------------------
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

		// ------------------------------------------------------------------------
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

		// ------------------------------------------------------------------------
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

		// ------------------------------------------------------------------------
		private void createPart(
			PartList id, AssetType type, string prefab,
			float w, float h, float d, Vector3Int cr, bool ug, string n, string e = "")
		{

            Part p = new Part();

            p.id     = id;
			p.type   = type;
			p.prefab = Resources.Load<GameObject>("Prefabs/" + prefab);

			p.w = w;
            p.h = h;
            p.d = d;

			p.canRotate   = cr;
			p.usesGravity = ug;

			p.name  = n;
			p.extra = e;

            _parts.Add(id, p);
        }

		// ------------------------------------------------------------------------
		public GameObject createPartAt(PartList partId, float x, float y, float z)
		{
            GameObject go = null;

            if (!_parts.ContainsKey(partId)) {
                return go;
            }

            go = Instantiate(_parts[partId].prefab);
            if (go != null) {
				go.name = "part_" + (_iCounter++).ToString (); //(_container.childCount+1).ToString();
                go.transform.SetParent(_container);
				go.transform.position = new Vector3 (x, y, z); //new Vector3(x * gridSize, y * gridSize, z * gridSize);
            }

            return go;
        }

		// ------------------------------------------------------------------------
		private void createAssetTypeCount()
		{
			_assetTypeList = new Dictionary<AssetType, List<Part>> ();
			_assetTypeIndex = new Dictionary<AssetType, int> ();

			foreach (KeyValuePair<PartList, Part> part in _parts) {

				if (!_assetTypeList.ContainsKey (part.Value.type)) {
					_assetTypeList.Add (part.Value.type, new List<Part>());
					_assetTypeIndex.Add (part.Value.type, 0);
				}

				_assetTypeList[part.Value.type].Add(part.Value);
			}

			foreach (KeyValuePair<AssetType, List<Part>> pair in _assetTypeList) {
				Debug.Log ("num assets for type "+pair.Key+" = "+pair.Value.Count);
			}
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
		private void resetSelectedElement()
		{
			_selectedElement = new LevelElement();
			_selectedElement.part = PartList.End_Of_List;

			_selectedMeshRenderers.Clear ();
			_selectedElementBounds = new Bounds();

			gizmoTranslateScript.translateTarget = null;
			gizmoTranslateScript.gameObject.SetActive (false);

			gizmoRotateScript.rotateTarget = null;
			gizmoRotateScript.gameObject.SetActive (false);

			PweMainMenu.Instance.showAssetInfoPanel (false);
			PweMainMenu.Instance.setSpecialHelpText ("");
		}

		// ------------------------------------------------------------------------
		private void resetElementComponents()
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