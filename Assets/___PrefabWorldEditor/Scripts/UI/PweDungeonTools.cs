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
		public Slider roomSliderSize;
		public Text roomSizeValue;
		//public Slider roomSliderInterval;
		//public Slider roomSliderDensity;
		//public Toggle roomToggleRandom;

		// Maze Tool Panel
		public Transform mazeToolPanel;
		public Slider mazeSliderSize;
		public Text mazeSizeValue;
		//public Slider mazeSliderInterval;
		//public Slider mazeSliderDensity;
		//public Toggle mazeToggleRandom;

		// Random Tool Panel
		public Transform randomToolPanel;
		public Slider randomSliderSize;
		public Text randomSizeValue;
		//public Slider randomSliderInterval;
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
			roomSliderSize.minValue = 1;
			roomSliderSize.maxValue = 36;
			//roomSliderInterval.minValue = 1;
			//roomSliderInterval.maxValue = 10;
			//roomSliderDensity.minValue = 1;
			//roomSliderDensity.maxValue = 10;

			mazeSliderSize.minValue = 1;
			mazeSliderSize.maxValue = 36;
			//mazeSliderInterval.minValue = 1;
			//mazeSliderInterval.maxValue = 10;
			//mazeSliderDensity.minValue = 1;
			//mazeSliderDensity.maxValue = 10;

			randomSliderSize.minValue = 1;
			randomSliderSize.maxValue = 36;
			//randomSliderInterval.minValue = 1;
			//randomSliderInterval.maxValue = 10;
			//randomSliderDensity.minValue = 1;
			//randomSliderDensity.maxValue = 10;

			reset ();
		}

		//
		public void reset()
		{
			roomSliderSize.value = 1;
			//roomSizeValue.text = roomSliderSize.value.ToString ();
			//roomSliderInterval.value = 1;
			//roomSliderDensity.value  = 1;
			//roomToggleRandom.isOn    = false;

			mazeSliderSize.value     = 1;
			//mazeSliderInterval.value = 1;
			//mazeSliderDensity.value  = 1;
			//mazeToggleRandom.isOn    = false;

			randomSliderSize.value     = 1;
			//randomSliderInterval.value = 1;
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

		public void onSliderRoomSizeChange(Single value)
		{
			roomSizeValue.text = ((int)roomSliderSize.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(0, (int)roomSliderSize.value);
		}
		/*public void onSliderRoomIntervalChange(Single value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(1, (int)roomSliderInterval.value);
		}
		public void onSliderRoomDensityChange(Single value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(2, (int)roomSliderDensity.value);
		}
		public void onToggleRoomRandomChange(Boolean value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(3, (roomToggleRandom.isOn ? 1 : 0));
		}*/

		public void onSliderMazeSizeChange(Single value)
		{
			mazeSizeValue.text = ((int)mazeSliderSize.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(0, (int)mazeSliderSize.value);
		}
		/*public void onSliderMazeIntervalChange(Single value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(1, (int)mazeSliderInterval.value);
		}
		public void onSliderMazeDensityChange(Single value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(2, (int)mazeSliderDensity.value);
		}
		public void onToggleMazeRandomChange(Boolean value)
		{
			PrefabLevelEditor.Instance.dungeonToolValueChange(3, (mazeToggleRandom.isOn ? 1 : 0));
		}*/

		public void onSliderRandomSizeChange(Single value)
		{
			randomSizeValue.text = ((int)randomSliderSize.value).ToString ();
			PrefabLevelEditor.Instance.dungeonToolValueChange(0, (int)randomSliderSize.value);
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}