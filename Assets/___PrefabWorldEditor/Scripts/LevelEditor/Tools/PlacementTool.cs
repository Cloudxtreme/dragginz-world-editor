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
			Mount
		};

		private static bool _initialised = false;

		protected static PlacementMode  _placementMode;

		protected static GameObject _container;

		protected static List<List<GameObject>> _gameObjects;

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

		public List<List<GameObject>> gameObjects {
			get { return _gameObjects; }
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

				_gameObjects = new List<List<GameObject>> ();

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

			_gameObjects.Clear ();

			_radius   = 1;
			_interval = 1;
			_density  = 1;

			_inverse = false;

			//_step = 0;

			setPlacementMode (PlacementMode.None);
		}

		// ------------------------------------------------------------------------
		public void activate(PlacementMode mode, Vector3 posOrigin, PrefabLevelEditor.Part part)
		{
			reset (); // just in case

			_curPart = part;

			setPlacementMode (mode);
		}

		// ------------------------------------------------------------------------
		public void update(int valueId, int value)
		{
			if (valueId == 0) {
				_radius = value;
				removeAll ();
			} else if (valueId == 1) {
				_interval = value;
				removeAll ();
			} else if (valueId == 2) {
				_density = value;
				removeAll ();
			} else if (valueId == 3) {
				_inverse = (value == 1);
				removeAll ();
			}

			if (_gameObjects.Count < _interval) {
				while (_gameObjects.Count < _interval) {
					createStep ();
				}
			} else if (_gameObjects.Count > _interval) {
				while (_gameObjects.Count > _interval) {
					removeLastStep ();
				}
			}

			int i;
			for (i = 1; i < _interval; ++i) {
				if (_gameObjects [i].Count <= 0) {
					createObjects (i);
				}
			}	
		}

		// ------------------------------------------------------------------------
		public virtual void createObjects(int step)
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
		protected void createStep()
		{
			_gameObjects.Add (new List<GameObject> ());
		}

		// ------------------------------------------------------------------------
		protected void removeLastStep()
		{
			int count = _gameObjects.Count;
			if (count > 0) {

				int i, len = _gameObjects [count-1].Count;
				for (i = 0; i < len; ++i) {
					GameObject.Destroy (_gameObjects [count-1][i]);
				}

				_gameObjects [count-1].Clear ();
				_gameObjects.RemoveAt (count-1);
			}
		}

		// ------------------------------------------------------------------------
		protected void removeAll ()
		{
			foreach (Transform childTransform in _container.transform) {
				GameObject.Destroy(childTransform.gameObject);
			}

			_gameObjects.Clear ();
		}
	}
}