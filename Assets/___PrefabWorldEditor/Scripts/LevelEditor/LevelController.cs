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
	public class LevelController : Singleton<LevelController>
    {
		public struct ElementGroup
		{
			public string groupType;
			public PrefabLevelEditor.Part part;
			public List<GameObject> gameObjects;
			//
			public PlacementTool.PlacementMode placement;
			public DungeonTool.DungeonPreset dungeon;
			public RoomTool.RoomPattern room;
			//
			public int width;
			public int height;
			public int depth;
			public bool ceiling;
			//
			public int radius;
			public int interval;
			public int density;
			public bool inverse;
		}

		private Transform _container;

		private List<ElementGroup> _aElementGroups;
		private int _iSelectedGroupIndex;

		#region Getters

		public List<ElementGroup> aElementGroups {
			get { return _aElementGroups; }
		}

		public int iSelectedGroupIndex {
			get { return _iSelectedGroupIndex; }
			set { _iSelectedGroupIndex = value; }
		}

		#endregion

		#region PublicMethods

		// ------------------------------------------------------------------------
		// Public Methods
		// ------------------------------------------------------------------------
		public void init(Transform container)
		{
			_container = container;

			_aElementGroups = new List<ElementGroup> ();
			_iSelectedGroupIndex = -1;
		}

		public void customUpdate()
		{
			
		}

		#endregion

		#region PrivateMethods

		// ------------------------------------------------------------------------
		// Private Methods
		// ------------------------------------------------------------------------
		private void something()
		{
		}

		#endregion
	}
}