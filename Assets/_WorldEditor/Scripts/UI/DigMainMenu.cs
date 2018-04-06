//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System;

using AssetsShared;

namespace DragginzWorldEditor
{
	/// <summary>
	/// ...
	/// </summary>
	public class DigMainMenu : MonoSingleton<DigMainMenu>
	{
		public Button btnShapeTypeCube;
		public Button btnShapeTypeSphere;
		public Button btnShapeTypeRock;

		public Button btnShapeSizeExtraSmall;
		public Button btnShapeSizeSmall;
		public Button btnShapeSizeMedium;
		public Button btnShapeSizeLarge;

		public Button btnDigSizeBlock;
		public Button btnDigSizeSmall;
		public Button btnDigSizeMedium;
		public Button btnDigSizeLarge;

		public void setShapeTypeButtons(int type) {
			btnShapeTypeCube.interactable   = (type != 0);
			btnShapeTypeSphere.interactable = (type != 1);
			btnShapeTypeRock.interactable   = (type != 2);
		}

		public void setShapeSizeButtons(int size) {
			btnShapeSizeExtraSmall.interactable = (size != 0);
			btnShapeSizeSmall.interactable      = (size != 1);
			btnShapeSizeMedium.interactable     = (size != 2);
			btnShapeSizeLarge.interactable      = (size != 3);
		}

		public void setDigSizeButtons(int size) {
			btnDigSizeBlock.interactable  = (size != -1);
			btnDigSizeSmall.interactable  = (size != 0);
			btnDigSizeMedium.interactable = (size != 1);
			btnDigSizeLarge.interactable  = (size != 2);
		}

		/*
		public void onButtonShapeTypeCubeClicked() {
			Debug.Log ("onButtonShapeTypeCubeClicked");
			DigInitialise.Instance.setShapeType(0);
		}
		public void onButtonShapeTypeSphereClicked() {
			DigInitialise.Instance.setShapeType(1);
		}
		public void onButtonShapeTypeRockClicked() {
			DigInitialise.Instance.setShapeType(2);
		}

		//
		public void onButtonShapeExtraSmallClicked() {
			DigInitialise.Instance.setShapeSize(0);
		}
		public void onButtonShapeSmallClicked() {
			DigInitialise.Instance.setShapeSize(1);
		}
		public void onButtonShapeMediumClicked() {
			DigInitialise.Instance.setShapeSize(2);
		}
		public void onButtonShapeLargeClicked() {
			DigInitialise.Instance.setShapeSize(3);
		}

		//
		public void onButtonDigBlockClicked() {
			DigInitialise.Instance.setDigSize(-1);
		}
		public void onButtonDigSmallClicked() {
			DigInitialise.Instance.setDigSize(0);
		}
		public void onButtonDigMediumClicked() {
			DigInitialise.Instance.setDigSize(1);
		}
		public void onButtonDigLargeClicked() {
			DigInitialise.Instance.setDigSize(2);
		}
		*/
	}
}