//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;

namespace DragginzWorldEditor
{
	public class AssetFactory : MonoSingleton<AssetFactory>
	{
		public GameObject prefabQuadrant;
		public GameObject prefabLevelChunk;
		public GameObject prefabLevelContainer;

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