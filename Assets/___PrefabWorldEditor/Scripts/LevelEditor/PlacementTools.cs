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
	public class PlacementTools
    {
		public enum PlacementMode {
			None,
			Circle,
			Mount
		};

		private PlacementMode  _placementMode;

		private GameObject _container;

		private List<List<GameObject>> _gameObjects;

		private PrefabLevelEditor.Part _curPart;

		private int _step;

		#region Getters

		public PlacementMode placementMode {
			get { return _placementMode; }
		}

		#endregion

		// ------------------------------------------------------------------------
		// Public Methods
		// ------------------------------------------------------------------------
		public void init(GameObject container)
        {
			_container = container;

			_gameObjects = new List<List<GameObject>> ();

			reset ();
        }

		// ------------------------------------------------------------------------
		public void reset()
		{
			foreach (Transform childTransform in _container.transform) {
				GameObject.Destroy(childTransform.gameObject);
			}

			_step = 0;

			setPlacementMode (PlacementMode.None);
		}

		// ------------------------------------------------------------------------
		public void activate(PlacementMode mode, Vector3 posOrigin, PrefabLevelEditor.Part part)
		{
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
			_container.transform.position = posOrigin;
		}

		// ------------------------------------------------------------------------
		// Private Methods
		// ------------------------------------------------------------------------
		private void setPlacementMode(PlacementMode mode)
		{
			if (mode != _placementMode) {

				_placementMode = mode;
			}
		}

		// ------------------------------------------------------------------------
		private void createNextStep(Vector3 posOrigin)
		{
			if (_gameObjects.Count <= _step) {
				_gameObjects.Add (new List<GameObject> ());
			}

			_container.transform.position = posOrigin;

			GameObject go;
			if (_placementMode == PlacementMode.Circle)
			{
				float radius = (float)(_step+1) * 2f;
				int i, len = 5 + ((_step+1) * 2);
				for (i = 0; i < len; ++i)     
				{
					float angle = (float)i * Mathf.PI * 2f / (float)len;
					Vector3 pos = new Vector3 (Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

					go = PrefabLevelEditor.Instance.createPartAt (_curPart.id, 0, 0, 0);
					if (go != null)
					{
						go.name = "temp_part_" + _container.transform.childCount.ToString();
						go.transform.SetParent(_container.transform);
						go.transform.localPosition = pos;

						PrefabLevelEditor.Instance.setMeshCollider (go, false);

						_gameObjects [_step].Add (go);
					}
				}
			}

			_step++;
			//Debug.Log ("step: " + _step);
		}

		// ------------------------------------------------------------------------
		private void removeLastStep()
		{
			if (_step > 1) {

				_step--;
				//Debug.Log ("step: " + _step);

				int i, len = _gameObjects [_step].Count;
				for (i = 0; i < len; ++i) {
					GameObject.Destroy (_gameObjects [_step][i]);
				}

				_gameObjects [_step].Clear ();
			}
		}
	}
}