//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

using AssetsShared;

//using RTEditor;

namespace PrefabWorldEditor
{
	public class PweDungeonTools : MonoSingleton<PweDungeonTools>
    {
		// Room Tool Panel
		public Transform roomToolPanel;
		public Slider roomSliderWidth;
		public Text roomWidthValue;
		public Slider roomSliderDepth;
		public Text roomDepthValue;
		public Slider roomSliderHeight;
		public Text roomHeightValue;
		//public Toggle roomToggleRandom;

		// Maze Tool Panel
		public Transform mazeToolPanel;
		public Slider mazeSliderWidth;
		public Text mazeWidthValue;
		public Slider mazeSliderDepth;
		public Text mazeDepthValue;
		//public Slider mazeSliderDensity;
		//public Toggle mazeToggleRandom;

		// Random Tool Panel
		public Transform randomToolPanel;
		public Slider randomSliderWidth;
		public Text randomWidthValue;
		public Slider randomSliderDepth;
		public Text randomDepthValue;
		//public Slider randomSliderDensity;
		//public Toggle randomToggleRandom;

		#region SystemMethods

        void Awake() {

			showToolPanels (DungeonTool.DungeonPreset.None);
        }

		#endregion

		#region PublicMethods

		public void init()
		{
			roomSliderWidth.minValue  = 1;
			roomSliderWidth.maxValue  = 36;
			roomSliderDepth.minValue  = 1;
			roomSliderDepth.maxValue  = 36;
			roomSliderHeight.minValue = 1;
			roomSliderHeight.maxValue = 36;

			mazeSliderWidth.minValue = 1;
			mazeSliderWidth.maxValue = 36;
			mazeSliderDepth.minValue = 1;
			mazeSliderDepth.maxValue = 36;
			//mazeSliderDensity.minValue = 1;
			//mazeSliderDensity.maxValue = 10;

			randomSliderWidth.minValue = 1;
			randomSliderWidth.maxValue = 36;
			randomSliderDepth.minValue = 1;
			randomSliderDepth.maxValue = 36;
			//randomSliderDensity.minValue = 1;
			//randomSliderDensity.maxValue = 10;

			reset ();
		}

		//
		public void reset()
		{
			roomSliderWidth.value  = 1;
			roomSliderDepth.value  = 1;
			roomSliderHeight.value = 1;
			//roomToggleRandom.isOn    = false;

			mazeSliderWidth.value = 1;
			mazeSliderDepth.value = 1;
			//mazeSliderDensity.value  = 1;
			//mazeToggleRandom.isOn    = false;

			randomSliderWidth.value = 1;
			randomSliderDepth.value = 1;
			//randomSliderDensity.value  = 1;
			//randomToggleRandom.isOn    = false;
		}

		//
		public void showToolPanels(DungeonTool.DungeonPreset mode) {

			roomToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Room);
			mazeToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Maze);
			randomToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Random);
		}

		//
		// Events
		//

		public void onSliderRoomWidthChange(Single value)
		{
			roomWidthValue.text = ((int)roomSliderWidth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(0, (int)roomSliderWidth.value);
		}
		public void onSliderRoomDepthChange(Single value)
		{
			roomDepthValue.text = ((int)roomSliderDepth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(1, (int)roomSliderDepth.value);
		}
		public void onSliderRoomHeightChange(Single value)
		{
			roomHeightValue.text = ((int)roomSliderHeight.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(2, (int)roomSliderHeight.value);
		}
		/*public void onToggleRoomRandomChange(Boolean value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(3, (roomToggleRandom.isOn ? 1 : 0));
		}*/

		public void onSliderMazeWidthChange(Single value)
		{
			mazeWidthValue.text = ((int)mazeSliderWidth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(0, (int)mazeSliderWidth.value);
		}
		public void onSliderMazeDepthChange(Single value)
		{
			mazeDepthValue.text = ((int)mazeSliderDepth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(1, (int)mazeSliderDepth.value);
		}
		/*public void onSliderMazeDensityChange(Single value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(2, (int)mazeSliderDensity.value);
		}
		public void onToggleMazeRandomChange(Boolean value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(3, (mazeToggleRandom.isOn ? 1 : 0));
		}*/

		public void onSliderRandomWidthChange(Single value)
		{
			randomWidthValue.text = ((int)randomSliderWidth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(0, (int)randomSliderWidth.value);
		}
		public void onSliderRandomDepthChange(Single value)
		{
			randomDepthValue.text = ((int)randomSliderDepth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(1, (int)randomSliderDepth.value);
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}