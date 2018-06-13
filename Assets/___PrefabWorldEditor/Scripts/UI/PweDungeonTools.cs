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
		public Toggle roomToggleCeiling;

		// Maze Tool Panel
		public Transform mazeToolPanel;
		public Slider mazeSliderWidth;
		public Text mazeWidthValue;
		public Slider mazeSliderDepth;
		public Text mazeDepthValue;

		// Random Tool Panel
		public Transform randomToolPanel;
		public Slider randomSliderWidth;
		public Text randomWidthValue;
		public Slider randomSliderDepth;
		public Text randomDepthValue;

		// Staircase Tool Panel
		public Transform stairsToolPanel;
		public Slider stairsSliderWidth;
		public Text stairsWidthValue;
		public Slider stairsSliderDepth;
		public Text stairsDepthValue;
		public Slider stairsSliderHeight;
		public Text stairsHeightValue;
		public Toggle stairsToggleCeiling;

		#region SystemMethods

        void Awake() {

			showToolPanels (DungeonTool.DungeonPreset.None);
        }

		#endregion

		#region PublicMethods

		public void init()
		{
			roomSliderWidth.minValue  = 2;
			roomSliderWidth.maxValue  = 18;
			roomSliderDepth.minValue  = 2;
			roomSliderDepth.maxValue  = 18;
			roomSliderHeight.minValue = 1;
			roomSliderHeight.maxValue = 18;

			mazeSliderWidth.minValue = 2;
			mazeSliderWidth.maxValue = 18;
			mazeSliderDepth.minValue = 2;
			mazeSliderDepth.maxValue = 18;

			randomSliderWidth.minValue = 2;
			randomSliderWidth.maxValue = 18;
			randomSliderDepth.minValue = 2;
			randomSliderDepth.maxValue = 18;

			stairsSliderWidth.minValue  = 3;
			stairsSliderWidth.maxValue  = 18;
			stairsSliderDepth.minValue  = 3;
			stairsSliderDepth.maxValue  = 18;
			stairsSliderHeight.minValue = 1;
			stairsSliderHeight.maxValue = 18;

			reset ();
		}

		//
		public void reset()
		{
			roomSliderWidth.value  = 2;
			roomSliderDepth.value  = 2;
			roomSliderHeight.value = 1;
			roomToggleCeiling.isOn  = false;

			mazeSliderWidth.value = 2;
			mazeSliderDepth.value = 2;

			randomSliderWidth.value = 2;
			randomSliderDepth.value = 2;

			stairsSliderWidth.value  = 3;
			stairsSliderDepth.value  = 3;
			stairsSliderHeight.value = 1;
			stairsToggleCeiling.isOn  = false;
		}

		//
		public void showToolPanels(DungeonTool.DungeonPreset mode) {

			roomToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Room);
			mazeToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Maze);
			randomToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Random);
			stairsToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Staircase);
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
		public void onToggleRoomCeilingChange(Boolean value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(3, (roomToggleCeiling.isOn ? 1 : 0));
		}

		//

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

		//

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

		//

		public void onSliderStairsWidthChange(Single value)
		{
			stairsWidthValue.text = ((int)stairsSliderWidth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(0, (int)stairsSliderWidth.value);
		}
		public void onSliderStairsDepthChange(Single value)
		{
			stairsDepthValue.text = ((int)stairsSliderDepth.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(1, (int)stairsSliderDepth.value);
		}
		public void onSliderStairsHeightChange(Single value)
		{
			stairsHeightValue.text = ((int)stairsSliderHeight.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(2, (int)stairsSliderHeight.value);
		}
		public void onToggleStairsCeilingChange(Boolean value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(3, (stairsToggleCeiling.isOn ? 1 : 0));
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}