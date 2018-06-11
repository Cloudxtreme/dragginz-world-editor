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
	public class DungeonTool
    {
		public enum DungeonPreset {
			None,
			Room,
			Maze,
			Random
		};

		private static bool _initialised = false;

		protected static DungeonPreset _dungeonPreset;

		protected static GameObject _container;

		protected static List<PrefabLevelEditor.LevelElement> _dungeonElements;

		protected static PrefabLevelEditor.Part _curPart;

		protected static int _size;
		protected static int _unused1;
		protected static int _unused2;
		protected static bool _unused3;

		//

		#region Getters

		public DungeonPreset dungeonPreset {
			get { return _dungeonPreset; }
		}

		public int interval {
			get { return _unused1; }
		}

		public bool inverse {
			get { return _unused3; }
		}

		public List<PrefabLevelEditor.LevelElement> dungeonElements {
			get { return _dungeonElements; }
		}

		#endregion

		//
		// CONSTRUCTOR
		//
		public DungeonTool(GameObject container)
		{
			if (!_initialised)
			{
				_initialised = true;

				_container = container;

				_dungeonElements = new List<PrefabLevelEditor.LevelElement> ();

				reset ();
			}
        }

		// ------------------------------------------------------------------------
		// Public Methods
		// ------------------------------------------------------------------------
		public void reset()
		{
			foreach (Transform childTransform in _container.transform) {
				GameObject.Destroy(childTransform.gameObject);
			}

			_dungeonElements.Clear ();

			_size    = 1;
			_unused1 = 1;
			_unused2 = 1;

			_unused3 = false;

			setDungeonPreset (DungeonPreset.None);
		}

		// ------------------------------------------------------------------------
		public void activate(DungeonPreset preset, Vector3 posOrigin, PrefabLevelEditor.Part part)
		{
			reset (); // just in case

			_curPart = part;

			setDungeonPreset (preset);
		}

		// ------------------------------------------------------------------------
		public void update(int valueId, int value)
		{
			if (valueId == 0) {
				_size = value;
			} else if (valueId == 1) {
				_unused1 = value;
			} else if (valueId == 2) {
				_unused2 = value;
			} else if (valueId == 3) {
				_unused3 = (value == 1);
			}

			removeAll ();

			createObjects ();
		}

		// ------------------------------------------------------------------------
		public virtual void createObjects()
		{
			// OVERRIDE ME
		}

		// ------------------------------------------------------------------------
		public void customUpdate(Vector3 posOrigin)
		{
			if (_dungeonPreset != DungeonPreset.None) {
				_container.transform.position = posOrigin;
			}
		}

		// ------------------------------------------------------------------------
		// Private Methods
		// ------------------------------------------------------------------------
		private void setDungeonPreset(DungeonPreset preset)
		{
			if (preset != _dungeonPreset) {

				_dungeonPreset = preset;

				PweDungeonTools.Instance.showToolPanels (preset);
			}
		}

		// ------------------------------------------------------------------------
		// Protected Methods
		// ------------------------------------------------------------------------
		protected void removeAll ()
		{
			foreach (Transform childTransform in _container.transform) {
				GameObject.Destroy(childTransform.gameObject);
			}

			_dungeonElements.Clear ();
		}
	}
}