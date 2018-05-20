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

		protected static int _step;

		//

		#region Getters

		public PlacementMode placementMode {
			get { return _placementMode; }
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
				Debug.Log ("init");

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

			_step = 0;

			setPlacementMode (PlacementMode.None);
		}

		// ------------------------------------------------------------------------
		public void activate(PlacementMode mode, Vector3 posOrigin, PrefabLevelEditor.Part part)
		{
			reset (); // just in case

			_curPart = part;
			_step = 0;

			setPlacementMode (mode);

			createNextStep (posOrigin);
		}

		// ------------------------------------------------------------------------
		public void extend(int dir, Vector3 posOrigin)
		{
			if (dir > 0) {
				createNextStep (posOrigin);
			} else {
				removeLastStep ();
			}
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
		private void createNextStep(Vector3 posOrigin)
		{
			if (_gameObjects.Count <= _step) {
				_gameObjects.Add (new List<GameObject> ());
			}

			_container.transform.position = posOrigin;

			createObjects ();

			/*if (_placementMode == PlacementMode.Circle) {
				createCircle ();
			} else if (_placementMode == PlacementMode.Quad) {
				createQuad ();
			}*/

			_step++;
		}

		// ------------------------------------------------------------------------
		public virtual void createObjects()
		{
			// OVERRIDE ME
		}

		// ------------------------------------------------------------------------
		private void removeLastStep()
		{
			if (_step > 1) {

				_step--;

				int i, len = _gameObjects [_step].Count;
				for (i = 0; i < len; ++i) {
					GameObject.Destroy (_gameObjects [_step][i]);
				}

				_gameObjects [_step].Clear ();
			}
		}
	}
}