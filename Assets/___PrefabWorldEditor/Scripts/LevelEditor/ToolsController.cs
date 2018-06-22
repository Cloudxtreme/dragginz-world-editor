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
	public class ToolsController : Singleton<ToolsController>
    {
		#region PublicStructs

		#endregion

		// ------------------------------------------------------------------------

		#region PrivateAttributes

		private GameObject _container;

		private List<PlacementTool> _aPlacementTools;
		private PlacementTool _curPlacementTool;

		private List<DungeonTool> _aDungeonTools;
		private DungeonTool _curDungeonTool;

		private List<RoomTool> _aRoomTools;
		private RoomTool _curRoomTool;

		#endregion

		// ------------------------------------------------------------------------

		#region Getters

		public PlacementTool curPlacementTool {
			get { return _curPlacementTool; }
		}

		public DungeonTool curDungeonTool {
			get { return _curDungeonTool; }
		}

		public RoomTool curRoomTool {
			get { return _curRoomTool; }
		}

		#endregion

		#region PublicMethods

		// ------------------------------------------------------------------------
		// Public Methods
		// ------------------------------------------------------------------------
		public void init(GameObject container)
		{
			_container = container;

			_aPlacementTools = new List<PlacementTool> ();
			_aPlacementTools.Add (new PlacementToolCircle (_container));
			_aPlacementTools.Add (new PlacementToolQuad (_container));
			_aPlacementTools.Add (new PlacementToolMount (_container));
			_aPlacementTools.Add (new PlacementToolCube (_container));

			_aDungeonTools = new List<DungeonTool> ();
			_aDungeonTools.Add (new DungeonToolRoom (_container));
			_aDungeonTools.Add (new DungeonToolMaze (_container));
			_aDungeonTools.Add (new DungeonToolRandom (_container));
			_aDungeonTools.Add (new DungeonToolStaircase (_container));

			_aRoomTools = new List<RoomTool> ();
			_aRoomTools.Add (new RoomToolDefault (_container));
		}

		// ------------------------------------------------------------------------
		public void setPlacementTool(PlacementTool.PlacementMode mode, PrefabLevelEditor.Part part)
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
		}

		// ------------------------------------------------------------------------
		public void setDungeonTool(DungeonTool.DungeonPreset preset)
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

			PweMainMenu.Instance.setAssetNameText ("");
			PweMainMenu.Instance.showAssetInfoPanel (false);
		}

		// ------------------------------------------------------------------------
		public void setRoomTool(RoomTool.RoomPattern pattern, PrefabLevelEditor.Part part)
		{
			if (pattern == RoomTool.RoomPattern.Default) {
				_curRoomTool = _aRoomTools [0];
			}

			PweRoomTools.Instance.reset ();
			PweRoomTools.Instance.showToolPanels (pattern);
		}

		// ------------------------------------------------------------------------
		public void resetCurPlacementTool()
		{
			if (_curPlacementTool != null) {
				_curPlacementTool.reset ();
				_curPlacementTool = null;
			}

			PweMainMenu.Instance.setPlacementToolButtons (PlacementTool.PlacementMode.None);
			PwePlacementTools.Instance.showToolPanels (PlacementTool.PlacementMode.None);
		}

		// ------------------------------------------------------------------------
		public void resetCurDungeonTool()
		{
			if (_curDungeonTool != null) {
				_curDungeonTool.reset ();
				_curDungeonTool = null;
				PweMainMenu.Instance.showAssetInfoPanel (true);
			}

			PweMainMenu.Instance.setDungeonToolButtons (DungeonTool.DungeonPreset.None);
			PweDungeonTools.Instance.showToolPanels (DungeonTool.DungeonPreset.None);
		}

		// ------------------------------------------------------------------------
		public void resetCurRoomTool()
		{
			if (_curRoomTool != null) {
				_curRoomTool.reset ();
				_curRoomTool = null;
			}

			PweMainMenu.Instance.setRoomToolButtons (RoomTool.RoomPattern.None);
			PweRoomTools.Instance.showToolPanels (RoomTool.RoomPattern.None);
		}

		#endregion

		#region PrivateMethods

		// ------------------------------------------------------------------------
		// Private Methods
		// ------------------------------------------------------------------------

		#endregion
	}
}