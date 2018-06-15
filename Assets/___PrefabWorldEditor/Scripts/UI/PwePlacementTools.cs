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
		public Text circleRadiusValue;
		public Slider circleSliderInterval;
		public Text circleIntervalValue;
		public Slider circleSliderDensity;
		public Text circleDensityValue;

		// Quad Tool Panel
		public Transform quadToolPanel;
		public Slider quadSliderRadius;
		public Text quadRadiusValue;
		public Slider quadSliderInterval;
		public Text quadIntervalValue;
		public Slider quadSliderDensity;
		public Text quadDensityValue;

		// Mount Tool Panel
		public Transform mountToolPanel;
		public Slider mountSliderRadius;
		public Text mountRadiusValue;
		public Slider mountSliderInterval;
		public Text mountIntervalValue;
		public Slider mountSliderDensity;
		public Text mountDensityValue;
		public Toggle mountToggleInverse;

		// Quad Tool Panel
		public Transform cubeToolPanel;
		public Slider cubeSliderRadius;
		public Text cubeRadiusValue;
		public Slider cubeSliderInterval;
		public Text cubeIntervalValue;
		public Slider cubeSliderDensity;
		public Text cubeDensityValue;

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
			circleSliderInterval.minValue = 2;
			circleSliderInterval.maxValue = 10;
			circleSliderDensity.minValue  = 1;
			circleSliderDensity.maxValue  = 10;

			quadSliderRadius.minValue   = 1;
			quadSliderRadius.maxValue   = 10;
			quadSliderInterval.minValue = 2;
			quadSliderInterval.maxValue = 10;
			quadSliderDensity.minValue  = 1;
			quadSliderDensity.maxValue  = 10;

			mountSliderRadius.minValue   = 1;
			mountSliderRadius.maxValue   = 10;
			mountSliderInterval.minValue = 2;
			mountSliderInterval.maxValue = 10;
			mountSliderDensity.minValue  = 1;
			mountSliderDensity.maxValue  = 10;

			cubeSliderRadius.minValue   = 1;
			cubeSliderRadius.maxValue   = 10;
			cubeSliderInterval.minValue = 2;
			cubeSliderInterval.maxValue = 10;
			cubeSliderDensity.minValue  = 1;
			cubeSliderDensity.maxValue  = 10;

			reset ();
		}

		//
		public void reset()
		{
			circleSliderRadius.value   = 1;
			circleSliderInterval.value = 2;
			circleSliderDensity.value  = 1;

			quadSliderRadius.value     = 1;
			quadSliderInterval.value   = 2;
			quadSliderDensity.value    = 1;

			mountSliderRadius.value    = 1;
			mountSliderInterval.value  = 2;
			mountSliderDensity.value   = 1;
			mountToggleInverse.isOn    = false;

			cubeSliderRadius.value     = 1;
			cubeSliderInterval.value   = 2;
			cubeSliderDensity.value    = 1;
		}

		//
		public void showToolPanels(PlacementTool.PlacementMode mode) {

			circleToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Circle);
			quadToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Quad);
			mountToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Mount);
			cubeToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Cube);
		}

		//

		public void updateRadiusValue(int value, PlacementTool.PlacementMode mode)
		{
			if (mode == PlacementTool.PlacementMode.Circle) {
				circleSliderRadius.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Quad) {
				quadSliderRadius.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Mount) {
				mountSliderRadius.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Cube) {
				cubeSliderRadius.value += value;
			}
		}

		public void updateIntervalValue(int value, PlacementTool.PlacementMode mode)
		{
			if (mode == PlacementTool.PlacementMode.Circle) {
				circleSliderInterval.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Quad) {
				quadSliderInterval.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Mount) {
				mountSliderInterval.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Cube) {
				cubeSliderInterval.value += value;
			}
		}

		public void updateDensityValue(int value, PlacementTool.PlacementMode mode)
		{
			if (mode == PlacementTool.PlacementMode.Circle) {
				circleSliderDensity.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Quad) {
				quadSliderDensity.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Mount) {
				mountSliderDensity.value += value;
			}
			else if (mode == PlacementTool.PlacementMode.Cube) {
				cubeSliderDensity.value += value;
			}
		}

		//
		// Events
		//
		public void onSliderCircleRadiusChange(Single value)
		{
			circleRadiusValue.text = ((int)circleSliderRadius.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(0, (int)circleSliderRadius.value);
		}
		public void onSliderCircleIntervalChange(Single value)
		{
			circleIntervalValue.text = ((int)circleSliderInterval.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(1, (int)circleSliderInterval.value);
		}
		public void onSliderCircleDensityChange(Single value)
		{
			circleDensityValue.text = ((int)circleSliderDensity.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(2, (int)circleSliderDensity.value);
		}

		//

		public void onSliderQuadRadiusChange(Single value)
		{
			quadRadiusValue.text = ((int)quadSliderRadius.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(0, (int)quadSliderRadius.value);
		}
		public void onSliderQuadIntervalChange(Single value)
		{
			quadIntervalValue.text = ((int)quadSliderInterval.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(1, (int)quadSliderInterval.value);
		}
		public void onSliderQuadDensityChange(Single value)
		{
			quadDensityValue.text = ((int)quadSliderDensity.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(2, (int)quadSliderDensity.value);
		}

		//


		public void onSliderMountRadiusChange(Single value)
		{
			mountRadiusValue.text = ((int)mountSliderRadius.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(0, (int)mountSliderRadius.value);
		}
		public void onSliderMountIntervalChange(Single value)
		{
			mountIntervalValue.text = ((int)mountSliderInterval.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(1, (int)mountSliderInterval.value);
		}
		public void onSliderMountDensityChange(Single value)
		{
			mountDensityValue.text = ((int)mountSliderDensity.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(2, (int)mountSliderDensity.value);
		}
		public void onToggleMountInverseChange(Boolean value)
		{
			PrefabLevelEditor.Instance.placementToolValueChange(3, (mountToggleInverse.isOn ? 1 : 0));
		}

		//

		public void onSliderCubeRadiusChange(Single value)
		{
			cubeRadiusValue.text = ((int)cubeSliderRadius.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(0, (int)cubeSliderRadius.value);
		}
		public void onSliderCubeIntervalChange(Single value)
		{
			cubeIntervalValue.text = ((int)cubeSliderInterval.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(1, (int)cubeSliderInterval.value);
		}
		public void onSliderCubeDensityChange(Single value)
		{
			cubeDensityValue.text = ((int)cubeSliderDensity.value).ToString ();
			PrefabLevelEditor.Instance.placementToolValueChange(2, (int)cubeSliderDensity.value);
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}