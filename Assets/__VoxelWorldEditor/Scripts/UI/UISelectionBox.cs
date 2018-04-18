//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using UnityEngine.UI;

using System;

using AssetsShared;

namespace DragginzVoxelWorldEditor
{
	public class UISelectionBox : MonoSingleton<MainMenu>
    {
		private int _iSelected;
		private int _iHiliteIndex;
		private int _iBoxIndex;

		private int _iNumBoxes;

		private RawImage[] _aBoxImages;
		private Image _imgHilight;

		private Action _onChangeCallback;

		#region GettersAndSetters

		public int iSelected {
			get { return _iSelected; }
		}

		#endregion

		#region SystemMethods

		// ---------------------------------------------------------------------------------------------
        void Awake()
		{
			_iSelected    = 0;
			_iHiliteIndex = 0;
			_iBoxIndex    = 0;

			_iNumBoxes  = 6;

			_aBoxImages = new RawImage[_iNumBoxes];

			Transform child = transform.Find ("Hilight");
			if (child != null) {
				_imgHilight = child.GetComponent<Image> ();
			}

			int i;
			for (i = 1; i <= _iNumBoxes; ++i) {
				child = transform.Find ("Box-" + i.ToString ());
				if (child != null) {
					_aBoxImages[i-1] = child.GetComponent<RawImage> ();
				}
			}
        }

		#endregion

		#region PublicMethods

		// ---------------------------------------------------------------------------------------------
		public void init(Action changeCallback)
		{
			_onChangeCallback = changeCallback;

			onSelect (0);
		}

		// ---------------------------------------------------------------------------------------------
		public void show(bool state)
		{
			gameObject.SetActive (state);
		}

		// ---------------------------------------------------------------------------------------------
		public void toggle(int toggle)
		{
			//Debug.Log ("Railgun toggle " + toggle);

			int index = _iSelected;
			if (toggle < 0) {
				index = (index > 0 ? index - 1 : 0);
			} else {
				index = (index < (Globals.materials.Length - 1) ? index + 1 : (Globals.materials.Length - 1));
			}

			_iHiliteIndex += toggle;
			if (_iHiliteIndex < 0) {
				_iHiliteIndex = 0;
				_iBoxIndex = (_iBoxIndex > 0 ? _iBoxIndex - 1 : 0);
			}
			else if (_iHiliteIndex > (_iNumBoxes - 1)) {
				_iHiliteIndex = (_iNumBoxes - 1);
				int maxBoxIndex = Globals.materials.Length - _iNumBoxes;
				_iBoxIndex = (_iBoxIndex < maxBoxIndex ? _iBoxIndex + 1 : maxBoxIndex);
			}

			if (_imgHilight != null) {
				_imgHilight.transform.localPosition = new Vector2 (_iHiliteIndex * 38, 0);
			}

			int i;
			for (i = 0; i < _iNumBoxes; ++i) {
				if (_aBoxImages [i] != null) {
					_aBoxImages [i].texture = LevelEditor.Instance.aTextures [_iBoxIndex + i];
				}
			}

			changeSelected (index);
		}

		// -------------------------------------------------------------------------------------
		public void onSelect(int value)
		{
			//Debug.Log ("Railgun onSelect " + value);

			_iHiliteIndex = value;

			if (_imgHilight != null) {
				_imgHilight.transform.localPosition = new Vector2 (_iHiliteIndex * 38, 0);
			}

			int newIndex = _iBoxIndex + _iHiliteIndex;
			changeSelected (newIndex);
		}

		#endregion

		#region PrivateMethods

		private void changeSelected(int index)
		{
			//Debug.Log ("Railgun changeSelected " + index);

			if (index >= 0 && index < Globals.materials.Length) {

				_iSelected = index;

				if (_onChangeCallback != null) {
					_onChangeCallback.Invoke ();
				}
				else {
					Debug.LogWarning ("shit");
				}
			}
			else {
				Debug.LogWarning ("fuck");
			}
		}

		#endregion
    }
}