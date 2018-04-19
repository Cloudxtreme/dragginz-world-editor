//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

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

		private List<Texture> _aTextures;
		private int _iNumTextures;

		//private Transform _butPrev;
		//private Transform _butNext;

		private Action _onChangeCallback;

		#region GettersAndSetters

		public int iSelected {
			get { return _iSelected; }
		}

		#endregion

		#region PublicMethods

		// ---------------------------------------------------------------------------------------------
		public void init(Action changeCallback, string[] textureList, string sFolderPath)
		{
			_onChangeCallback = changeCallback;

			_aTextures = new List<Texture> ();
			_iNumTextures = textureList.Length;
			int i;
			for (i = 0; i < _iNumTextures; ++i) {
				_aTextures.Add (Resources.Load<Texture> ("Textures/" + sFolderPath + textureList [i]));
			}

			_iSelected    = 0;
			_iHiliteIndex = 0;
			_iBoxIndex    = 0;

			_iNumBoxes  = 6;

			_aBoxImages = new RawImage[_iNumBoxes];

			Transform child = transform.Find ("Hilight");
			if (child != null) {
				_imgHilight = child.GetComponent<Image> ();
			}

			for (i = 1; i <= _iNumBoxes; ++i) {
				child = transform.Find ("Box-" + i.ToString ());
				if (child != null) {
					_aBoxImages[i-1] = child.GetComponent<RawImage> ();
				}
			}

			onSelect (0);

			updateBoxTextures ();
		}

		// ---------------------------------------------------------------------------------------------
		public void show(bool state)
		{
			gameObject.SetActive (state);
		}

		// ---------------------------------------------------------------------------------------------
		public void toggle(int toggle)
		{
			int index = _iSelected;
			if (toggle < 0) {
				index = (index > 0 ? index - 1 : 0);
			} else {
				index = (index < (_iNumTextures - 1) ? index + 1 : (_iNumTextures - 1));
			}

			_iHiliteIndex += toggle;
			if (_iHiliteIndex < 0) {
				_iHiliteIndex = 0;
				_iBoxIndex = (_iBoxIndex > 0 ? _iBoxIndex - 1 : 0);
			}
			else if (_iHiliteIndex >= _iNumTextures) {
				_iHiliteIndex -= toggle;
			}
			else if (_iHiliteIndex > (_iNumBoxes - 1)) {
				_iHiliteIndex = (_iNumBoxes - 1);
				int maxBoxIndex = _iNumTextures - _iNumBoxes;
				_iBoxIndex = (_iBoxIndex < maxBoxIndex ? _iBoxIndex + 1 : maxBoxIndex);
			}

			if (_imgHilight != null) {
				_imgHilight.transform.localPosition = new Vector2 (_iHiliteIndex * 38, 0);
			}

			updateBoxTextures ();

			changeSelected (index);
		}

		// -------------------------------------------------------------------------------------
		public void onSelect(int value)
		{
			_iHiliteIndex = value;

			if (_imgHilight != null) {
				_imgHilight.transform.localPosition = new Vector2 (_iHiliteIndex * 38, 0);
			}

			int newIndex = _iBoxIndex + _iHiliteIndex;
			changeSelected (newIndex);
		}

		#endregion

		#region PrivateMethods

		private void updateBoxTextures()
		{
			int i;
			for (i = 0; i < _iNumBoxes; ++i)
			{
				if (_aBoxImages [i] != null)
				{
					if (i >= _iNumTextures) {
						_aBoxImages [i].gameObject.SetActive (false);
					} else {
						_aBoxImages [i].texture = _aTextures [_iBoxIndex + i];
					}
				}
			}
		}

		//
		private void changeSelected(int index)
		{
			if (index >= 0 && index < _iNumTextures) {

				_iSelected = index;

				if (_onChangeCallback != null) {
					_onChangeCallback.Invoke ();
				}
			}
		}

		#endregion
    }
}