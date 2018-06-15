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
	public class PlacementTool
    {
		public enum PlacementMode {
			None,
			Circle,
			Quad,
			Mount,
			Cube
		};

		private static bool _initialised = false;

		protected static PlacementMode  _placementMode;

		protected static GameObject _container;

		protected static List<PrefabLevelEditor.LevelElement> _elements;

		protected static PrefabLevelEditor.Part _curPart;

		protected static int _radius;
		protected static int _interval;
		protected static int _density;
		protected static bool _inverse;

		//

		#region Getters

		public PlacementMode placementMode {
			get { return _placementMode; }
		}

		public int interval {
			get { return _interval; }
		}

		public bool inverse {
			get { return _inverse; }
		}

		public List<PrefabLevelEditor.LevelElement> elements {
			get { return _elements; }
		}

		#endregion

		//
		// CONSTRUCTOR
		//
		public PlacementTool(GameObject container)
		{
			if (!_initialised)
			{
				_initialised = true;

				_container = container;

				_elements = new List<PrefabLevelEditor.LevelElement> ();

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

			_elements.Clear ();

			_radius   = 1;
			_interval = 2;
			_density  = 1;

			_inverse = false;

			setPlacementMode (PlacementMode.None);
		}

		// ------------------------------------------------------------------------
		public void activate(PlacementMode mode, Vector3 posOrigin, PrefabLevelEditor.Part part)
		{
			reset (); // just in case

			_curPart = part;

			setPlacementMode (mode);

			update (-1, -1);
		}

		// ------------------------------------------------------------------------
		public void update(int valueId, int value)
		{
			if (valueId == 0) {
				_radius = value;
			} else if (valueId == 1) {
				_interval = value;
			} else if (valueId == 2) {
				_density = value;
			} else if (valueId == 3) {
				_inverse = (value == 1);
			}

			removeAll ();
			createObjects ();
		}

		// ------------------------------------------------------------------------
		public virtual void createObjects() //int step)
		{
			// OVERRIDE ME
		}

		// ------------------------------------------------------------------------
		public void customUpdate(Vector3 posOrigin)
		{
			if (_placementMode != PlacementMode.None) {
				_container.transform.position = posOrigin;
			}
		}

		// ------------------------------------------------------------------------
		// Private Methods
		// ------------------------------------------------------------------------
		private void setPlacementMode(PlacementMode mode)
		{
			if (mode != _placementMode) {

				_placementMode = mode;

				PwePlacementTools.Instance.showToolPanels (mode);
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

			_elements.Clear ();
		}
	}
}