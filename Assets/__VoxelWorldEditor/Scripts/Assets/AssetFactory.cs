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
		private GameObject prefabVoxelLevelContainer;
		[SerializeField]
		private GameObject prefabVoxelChunk;

		#region SystemMethods

		void Awake()
		{
			//
		}

        #endregion

        #region PublicMethods

		public GameObject createVoxelsLevelContainer()
		{
			return (prefabVoxelLevelContainer != null ? Instantiate (prefabVoxelLevelContainer) : null);
		}

		public GameObject createVoxelChunkClone()
		{
			return (prefabVoxelChunk != null ? Instantiate (prefabVoxelChunk) : null);
		}

		#endregion
	}
}