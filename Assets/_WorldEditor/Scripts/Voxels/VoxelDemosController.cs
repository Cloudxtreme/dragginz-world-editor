//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace VoxelChunks
{
	public class VoxelDemosController : MonoBehaviour
	{
		public Button butPrev;
		public Text txtTitle;
		public Button butNext;

		private string[] _demoScenes;
		private string[] _sceneDesc;
		private int _curSceneIndex;

		//
		void Awake()
		{
			_demoScenes = new string[]{"voxels_demo_1"};
			_sceneDesc  = new string[]{"Voxel Chunk Splitting"};

			_curSceneIndex = -1;

			loadNextScene ();
		}

		//
		public void loadNextScene()
		{
			loadScene (_curSceneIndex + 1);
		}

		//
		public void loadPreviousScene()
		{
			loadScene (_curSceneIndex - 1);
		}

		//
		private void loadScene(int sceneIndex)
		{
			if (sceneIndex < 0 || sceneIndex >= _demoScenes.Length) {
				return;
			}

			if (_curSceneIndex != -1) {
				SceneManager.UnloadSceneAsync (_demoScenes [_curSceneIndex]);
			}

			_curSceneIndex = sceneIndex;
			SceneManager.LoadScene (_demoScenes [_curSceneIndex], LoadSceneMode.Additive);

			txtTitle.text = "Demo " + _curSceneIndex.ToString() + ": " + _sceneDesc[_curSceneIndex];
		}
	}
}