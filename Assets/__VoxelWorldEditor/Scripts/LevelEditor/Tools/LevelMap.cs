//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using AssetsShared;

namespace DragginzVoxelWorldEditor
{
	public class LevelMap : MonoSingleton<LevelMap>
	{
		public Transform container;
		public GameObject prefabCube;

		private int _numCubesPerAxis;

		private List<GameObject> _aCubes;

		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Awake ()
		{
			_numCubesPerAxis = Mathf.CeilToInt((float)VoxelUtils.MAX_CHUNK_UNITS / VoxelUtils.CHUNK_SIZE);
		}

		#region PrivateMethods


		#endregion

		#region PublicMethods

		public void reset()
		{
			_aCubes = new List<GameObject> ();

			foreach (Transform child in container) {
				Destroy (child.gameObject);
			}
		}

		// ---------------------------------------------------------------------------------------------
		public void show(bool state)
		{
			if (!state) {
				gameObject.SetActive (false);
				return;
			}

			gameObject.SetActive (true);
		}

		// ---------------------------------------------------------------------------------------------
		public void addCube(VoxelUtils.VoxelVector3Int pos, Vector3 size)
		{
			GameObject go = Instantiate (prefabCube);
			go.name = "x" + pos.x.ToString () + "-" + "y" + pos.y.ToString () + "-" + "z" + pos.z.ToString ();
			go.transform.SetParent (container);

			float width  = size.x * VoxelUtils.CHUNK_SIZE;
			float height = size.y * VoxelUtils.CHUNK_SIZE;
			float depth  = size.z * VoxelUtils.CHUNK_SIZE;

			go.transform.localScale = new Vector3 (width, height, depth);

			Vector3 posNew = new Vector3 ((pos.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (pos.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (pos.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));
			go.transform.localPosition = posNew;

			_aCubes.Add (go);
		}

		#endregion
	}
}