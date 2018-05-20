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

		// Quad Tool Panel
		public Transform quadToolPanel;
		public Slider quadSliderRadius;

		#region SystemMethods

        void Awake() {

			showToolPanels (PlacementTool.PlacementMode.None);
        }

		#endregion

		#region PublicMethods

		public void init()
		{
			//
		}

		//
		public void showToolPanels(PlacementTool.PlacementMode mode) {

			circleToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Circle);
			quadToolPanel.gameObject.SetActive (mode == PlacementTool.PlacementMode.Quad);
		}

		#endregion

		#region PrivateMethods

		#endregion
    }
}