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
		public Slider roomSliderInterval;
		public Slider roomSliderDensity;
		public Toggle roomToggleRandom;

		#region SystemMethods

        void Awake() {

			showToolPanels (DungeonTool.DungeonPreset.None);
        }

		#endregion

		#region PublicMethods

		public void init()
		{
			roomSliderSize.minValue = 1;
			roomSliderSize.maxValue = 10;

			roomSliderInterval.minValue = 1;
			roomSliderInterval.maxValue = 10;

			roomSliderDensity.minValue = 1;
			roomSliderDensity.maxValue = 10;

			reset ();
		}

		//
		public void reset()
		{
			roomSliderSize.value     = 1;
			roomSliderInterval.value = 1;
			roomSliderDensity.value  = 1;
			roomToggleRandom.isOn    = false;
		}

		//
		public void showToolPanels(DungeonTool.DungeonPreset mode) {

			roomToolPanel.gameObject.SetActive (mode == DungeonTool.DungeonPreset.Room);
		}

		//
		// Events
		//

		public void onSliderMountRadiusChange(Single value)
		{
			//PrefabLevelEditor.Instance.placementToolValueChange(0, (int)roomSliderSize.value);
		}
		public void onSliderMountIntervalChange(Single value)
		{
			//PrefabLevelEditor.Instance.placementToolValueChange(1, (int)roomSliderInterval.value);
		}
		public void onSliderMountDensityChange(Single value)
		{
			//PrefabLevelEditor.Instance.placementToolValueChange(2, (int)roomSliderDensity.value);
		}
		public void onToggleMountInverseChange(Boolean value)
		{
			//PrefabLevelEditor.Instance.placementToolValueChange(3, (roomToggleRandom.isOn ? 1 : 0));
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}