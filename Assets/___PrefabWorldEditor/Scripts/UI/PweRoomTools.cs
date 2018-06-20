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

namespace PrefabWorldEditor
{
	public class PweRoomTools : MonoSingleton<PweRoomTools>
    {
		// Default Tool Panel
		public Transform defaultToolPanel;
		public Slider defaultSliderWidth;
		public Text defaultWidthValue;
		public Slider defaultSliderHeight;
		public Text defaultHeightValue;

		#region SystemMethods

        void Awake() {

			showToolPanels (RoomTool.RoomPattern.None);
        }

		#endregion

		#region PublicMethods

		public void init()
		{
			defaultSliderWidth.minValue = 2;
			defaultSliderWidth.maxValue = 18;
			defaultSliderHeight.minValue = 2;
			defaultSliderHeight.maxValue = 18;

			reset ();
		}

		//
		public void reset()
		{
			defaultSliderWidth.value = 2;
			defaultSliderHeight.value = 2;
		}

		//
		public void showToolPanels(RoomTool.RoomPattern mode) {

			defaultToolPanel.gameObject.SetActive (mode == RoomTool.RoomPattern.Default);
		}

		//
		// UPDATE VALUES
		//

		public void updateWidthValue(int value, RoomTool.RoomPattern mode)
		{
			if (mode == RoomTool.RoomPattern.Default) {
				defaultSliderWidth.value += value;
			}
		}

		public void updateHeightValue(int value, RoomTool.RoomPattern mode)
		{
			if (mode == RoomTool.RoomPattern.Default) {
				defaultSliderHeight.value += value;
			}
		}

		//
		// SET VALUES
		//

		public void setWidthValue(int value, RoomTool.RoomPattern mode)
		{
			if (mode == RoomTool.RoomPattern.Default) {
				defaultSliderWidth.value = value;
			}
		}

		public void setHeightValue(int value, RoomTool.RoomPattern mode)
		{
			if (mode == RoomTool.RoomPattern.Default) {
				defaultSliderHeight.value = value;
			}
		}

		//
		// Events
		//

		public void onSliderDefaultWidthChange(Single value)
		{
			defaultWidthValue.text = ((int)defaultSliderWidth.value).ToString ();
			PrefabLevelEditor.Instance.roomToolValueChange(0, (int)defaultSliderWidth.value);
		}
		public void onSliderDefaultHeightChange(Single value)
		{
			defaultHeightValue.text = ((int)defaultSliderHeight.value).ToString ();
			PrefabLevelEditor.Instance.roomToolValueChange(2, (int)defaultSliderHeight.value);
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}