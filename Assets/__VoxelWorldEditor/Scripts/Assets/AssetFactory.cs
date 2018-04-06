//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;

using AssetsShared;

namespace DragginzVoxelWorldEditor
{
	public class AssetFactory : MonoSingleton<AssetFactory>
	{
		[SerializeField]
		private GameObject prefabQuadrant;
		[SerializeField]
		private GameObject prefabLevelChunk;
		[SerializeField]
		private GameObject prefabLevelContainer;

		#region SystemMethods

		void Awake()
		{
			//
		}

        #endregion

        #region PublicMethods

		public GameObject createQuadrantClone()
		{
			return (prefabQuadrant != null ? Instantiate (prefabQuadrant) : null);
		}
		public GameObject createLevelChunkClone()
		{
			return (prefabLevelChunk != null ? Instantiate (prefabLevelChunk) : null);
		}
		public GameObject createLevelContainerClone()
		{
			return (prefabLevelContainer != null ? Instantiate (prefabLevelContainer) : null);
		}

		#endregion
	}
}