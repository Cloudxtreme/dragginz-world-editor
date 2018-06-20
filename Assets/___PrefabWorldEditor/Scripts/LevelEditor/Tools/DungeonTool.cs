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
			Random,
			Staircase
		};

		private static bool _initialised = false;
		private static bool _rebuildOnUpdate = true;

		protected static DungeonPreset _dungeonPreset;

		protected static GameObject _container;

		protected static List<PrefabLevelEditor.LevelElement> _dungeonElements;

		protected static int _width;
		protected static int _depth;
		protected static int _height;
		protected static bool _ceiling;

		protected static float _cubeSize = 2.0f;

		//

		#region Getters

		public DungeonPreset dungeonPreset {
			get { return _dungeonPreset; }
		}

		public float cubeSize {
			get { return _cubeSize; }
		}

		public int width {
			get { return _width; }
			set { _width = value; }
		}

		public int depth {
			get { return _depth; }
			set { _depth = value; }
		}

		public int height {
			get { return _height; }
			set { _height = value; }
		}

		public bool ceiling {
			get { return _ceiling; }
			set { _ceiling = value; }
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

			_width  = 2;
			_depth  = 2;
			_height = 1;

			_ceiling = false;

			setDungeonPreset (DungeonPreset.None);
		}

		// ------------------------------------------------------------------------
		public void activate(DungeonPreset preset)
		{
			reset (); // just in case

			setDungeonPreset (preset);

			if (preset == DungeonPreset.Staircase) {
				_width  = 3;
				_depth  = 3;
			}

			removeAll ();
			createObjects ();
		}

		// ------------------------------------------------------------------------
		public void activateAndCopy(DungeonPreset preset, int w, int h, int d, bool c)
		{
			_rebuildOnUpdate = false;

			reset (); // just in case

			setDungeonPreset (preset);

			PweDungeonTools.Instance.setWidthValue (w, preset);
			PweDungeonTools.Instance.setHeightValue (h, preset);
			PweDungeonTools.Instance.setDepthValue (d, preset);
			PweDungeonTools.Instance.setCeilingValue (c, preset);

			_rebuildOnUpdate = true;

			update (-1, -1); // force update
		}

		// ------------------------------------------------------------------------
		public void update(int valueId, int value)
		{
			if (valueId == 0) {
				_width = value;
			} else if (valueId == 1) {
				_depth = value;
			} else if (valueId == 2) {
				_height = value;
			} else if (valueId == 3) {
				_ceiling = (value == 1);
			}

			if (_rebuildOnUpdate) {
				removeAll ();
				createObjects ();
			}
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