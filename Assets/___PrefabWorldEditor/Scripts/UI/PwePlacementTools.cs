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
	public class PwePlacementTools : MonoSingleton<PwePlacementTools>
    {
		// Circle Tool Panel
		public Transform circleToolPanel;
		public Slider circleSliderRadius;
		public Slider circleSliderInterval;
		public Slider circleSliderDensity;

		// Quad Tool Panel
		public Transform quadToolPanel;
		public Slider quadSliderRadius;
		public Slider quadSliderInterval;
		public Slider quadSliderDensity;

		#region SystemMethods

        void Awake() {

			showToolPanels (PlacementTool.PlacementMode.None);
        }

		#endregion

		#region PublicMethods

		public void init()
		{
			circleSliderRadius.minValue   = 1;
			circleSliderRadius.maxValue   = 10;

			circleSliderInterval.minValue = 1;
			circleSliderInterval.maxValue = 10;

			circleSliderDensity.minValue  = 1;
			circleSliderDensity.maxValue  = 10;

			quadSliderRadius.minValue   = 1;
			quadSliderRadius.maxValue   = 10;

			quadSliderInterval.minValue = 1;
			quadSliderInterval.maxValue = 10;

			quadSliderDensity.minValue  = 1;
			quadSliderDensity.maxValue  = 10;

			reset ();
		}

		//
		public void reset()
		{
			circleSliderRadius.value   = 1;
			circleSliderInterval.value = 1;
			circleSliderDensity.value  = 1;

			quadSliderRadius.value     = 1;
			quadSliderInterval.value   = 1;
			quadSliderDensity.value    = 1;
		}

		//
		public void showToolPanels(PlacementTool.PlacementMode mode) {

			circleToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Circle);
			quadToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Quad);
		}

		//
		// Events
		//
		public void onSliderCircleRadiusChange(Single value)
		{
			PrefabLevelEditor.Instance.placementToolValueChange(0, (int)circleSliderRadius.value);
		}
		public void onSliderCircleIntervalChange(Single value)
		{
			PrefabLevelEditor.Instance.placementToolValueChange(1, (int)circleSliderInterval.value);
		}
		public void onSliderCircleDensityChange(Single value)
		{
			PrefabLevelEditor.Instance.placementToolValueChange(2, (int)circleSliderDensity.value);
		}

		public void onSliderQuadRadiusChange(Single value)
		{
			PrefabLevelEditor.Instance.placementToolValueChange(0, (int)quadSliderRadius.value);
		}
		public void onSliderQuadIntervalChange(Single value)
		{
			PrefabLevelEditor.Instance.placementToolValueChange(1, (int)quadSliderInterval.value);
		}
		public void onSliderQuadDensityChange(Single value)
		{
			PrefabLevelEditor.Instance.placementToolValueChange(2, (int)quadSliderDensity.value);
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}