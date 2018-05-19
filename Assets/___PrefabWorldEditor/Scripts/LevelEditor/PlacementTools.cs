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
			Quad,
			Mount
		};

		private PlacementMode  _placementMode;

		private GameObject _container;

		private List<List<GameObject>> _gameObjects;

		private PrefabLevelEditor.Part _curPart;

		private int _step;

		//

		#region Getters

		public PlacementMode placementMode {
			get { return _placementMode; }
		}

		public List<List<GameObject>> gameObjects {
			get { return _gameObjects; }
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

			_gameObjects.Clear ();

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
			}
		}

		// ------------------------------------------------------------------------
		private void createNextStep(Vector3 posOrigin)
		{
			if (_gameObjects.Count <= _step) {
				_gameObjects.Add (new List<GameObject> ());
			}

			_container.transform.position = posOrigin;

			if (_placementMode == PlacementMode.Circle) {
				createCircle ();
			} else if (_placementMode == PlacementMode.Quad) {
				createQuad ();
			}

			_step++;
		}

		// ------------------------------------------------------------------------
		private void createCircle()
		{
			GameObject go;
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
					PrefabLevelEditor.Instance.setRigidBody (go, false);

					_gameObjects [_step].Add (go);
				}
			}
		}

		// ------------------------------------------------------------------------
		private void createQuad()
		{
			GameObject go;
			float distance = (_curPart.w + _curPart.d) / 2.5f;
			int x, z, len = _step + 1;
			for (x = -len; x <= len; ++x)
			{
				for (z = -len; z <= len; ++z)
				{
					if (x > -len && x < len && z > -len && z < len) {
						continue;
					}

					Vector3 pos = new Vector3 (x * distance, 0, z * distance);

					go = PrefabLevelEditor.Instance.createPartAt (_curPart.id, 0, 0, 0);
					if (go != null) {
						go.name = "temp_part_" + _container.transform.childCount.ToString ();
						go.transform.SetParent (_container.transform);
						go.transform.localPosition = pos;

						PrefabLevelEditor.Instance.setMeshCollider (go, false);
						PrefabLevelEditor.Instance.setRigidBody (go, false);

						_gameObjects [_step].Add (go);
					}
				}
			}
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