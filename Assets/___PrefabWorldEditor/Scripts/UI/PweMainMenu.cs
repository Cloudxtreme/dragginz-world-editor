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
	public class PweMainMenu : MonoSingleton<PweMainMenu>
    {
		public GameObject goTransformSelection;
		public GameObject goAssetTypeSelection;
		public GameObject goPlacementToolButtons;
		//public GameObject goItemsSelection;
		//public GameObject goDigSettings;

		public UISelectionBox selectionBoxRailgun;

		public Transform panelTools;
        public Transform panelFileMenu;
		public Transform panelLevelMenu;

		public Transform panelAssetInfo;
		public UIAssetInfo assetInfo;

		public Transform blocker;
        public Transform panelPopup;

		public Button btnModePlay;
		public Button btnModeBuild;
		public Button btnModeSelect;
		public Button btnUNDO;

		public Image imgMaterialHilight;
		//public RawImage imgSelectedMaterial;
		public RawImage imgSelectedItem;

		//public Slider sliderDigWidth;
		//public Slider sliderDigHeight;
		//public Slider sliderDigDepth;

		public Button btnAssetFloors;
		public Button btnAssetWalls;
		public Button btnAssetChunks;
		public Button btnAssetProps;
		public Button btnAssetDungeons;

		public Button btnPlacementToolCircle;
		public Button btnPlacementToolQuad;
		public Button btnPlacementToolMount;

		public Text txtFileInfo;
		public Text txtLevelName;
		public Text txtCubeCount;
		public Text txtInstructions;
		public Text txtAssetName;
		public Text txtSpecialHelp;

        public Image imgMove;
        public Image imgRotate;
        public Image imgScale;
        public Image imgVolume;

        //private EditorGizmoSystem _gizmoSystem;
            
        private Dropdown _trfmDropDownFile = null;
		private Text _txtPanelFile = null;
        private int _iDropDownFileOptions = 0;

		private Dropdown _trfmDropDownLevel = null;
		private Text _txtPanelLevel = null;
		private int _iDropDownLevelOptions = 0;
		//private int _iSelectedLevel = -1;

        private int _iSelectedTool = -1;
		private int _iSelectedAssetType = -1;

		private Vector3 _v3DigSettings = new Vector3(1,1,1);
		public Vector3 v3DigSettings {
			get { return _v3DigSettings; }
		}

		//private float _lastMouseWheelUpdate;

		private int _iMatHiliteIndex   = 0;
		//private int _iMaterialBoxIndex = 0;
		private int _iSelectedMaterial = 0;
		public int iSelectedMaterial {
			get { return _iSelectedMaterial; }
		}

		private PwePopup _popup;
		public PwePopup popup {
            get { return _popup; }
        }

		public int iSelectedRailgunMaterialIndex {
			get { return selectionBoxRailgun.iSelected; }
		}

		public int iSelectedTool {
			get { return _iSelectedTool; }
		}

		public int iSelectedAssetType {
			get { return _iSelectedAssetType; }
		}

		#region SystemMethods

		/// <summary>
		/// Instantiate vars and hide game objects
		/// </summary>
        void Awake() {

			if (txtFileInfo) {
				txtFileInfo.text = Globals.version;
			}

            if (blocker) {
                blocker.gameObject.SetActive(false);
            }
            if (panelPopup) {
                panelPopup.gameObject.SetActive(false);
				_popup = panelPopup.GetComponent<PwePopup>();
            }

			Transform trfmMenu;
			Transform trfmText;

            if (panelFileMenu != null) {
				trfmText = panelFileMenu.Find("Text");
				if (trfmText != null) {
					_txtPanelFile = trfmText.GetComponent<Text> ();
				}
                trfmMenu = panelFileMenu.Find("DropdownFile");
                if (trfmMenu) {
                    _trfmDropDownFile = trfmMenu.GetComponent<Dropdown>();
                    if (_trfmDropDownFile) {
                        _iDropDownFileOptions = _trfmDropDownFile.options.Count;
						//Debug.Log ("_iDropDownFileOptions: "+_iDropDownFileOptions);
                    }
                }
            }

			if (panelLevelMenu != null) {
				trfmText = panelLevelMenu.Find("Text");
				if (trfmText != null) {
					_txtPanelLevel = trfmText.GetComponent<Text> ();
				}
				trfmMenu = panelLevelMenu.Find("DropdownFile");
				if (trfmMenu) {
					_trfmDropDownLevel = trfmMenu.GetComponent<Dropdown>();
					if (_trfmDropDownLevel) {
						_iDropDownLevelOptions = _trfmDropDownLevel.options.Count;
						//Debug.Log ("_iDropDownLevelOptions: "+_iDropDownLevelOptions);
					}
				}
				panelLevelMenu.gameObject.SetActive (false);
			}

			/*if (sliderDigWidth != null) {
				sliderDigWidth.value = sliderDigWidth.minValue = 1;
				sliderDigWidth.maxValue = 2 / VoxelUtils.CHUNK_SIZE;
			}
			if (sliderDigHeight != null) {
				sliderDigHeight.value = sliderDigHeight.minValue = 1;
				sliderDigHeight.maxValue = 2 / VoxelUtils.CHUNK_SIZE;
			}
			if (sliderDigDepth != null) {
				sliderDigDepth.value = sliderDigDepth.minValue = 1;
				sliderDigDepth.maxValue = 2 / VoxelUtils.CHUNK_SIZE;
			}*/

			//_lastMouseWheelUpdate = 0;
        }

		/*void OnEnable() {
			_gizmoSystem = EditorGizmoSystem.Instance;
			if (_gizmoSystem != null) {
				_gizmoSystem.ActiveGizmoTypeChanged += onGizmoChanged;
                EditorObjectSelection.Instance.GameObjectClicked += onGameObjectClicked;
				EditorObjectSelection.Instance.SelectionChanged += onSelectionChanged;
            }
		}

		void OnDisable() {
			if (_gizmoSystem != null) {
				_gizmoSystem.ActiveGizmoTypeChanged -= onGizmoChanged;
                EditorObjectSelection.Instance.GameObjectClicked -= onGameObjectClicked;
				EditorObjectSelection.Instance.SelectionChanged -= onSelectionChanged;
            }
		}*/

		#endregion

		#region PublicMethods

		public void init()
		{
			selectionBoxRailgun.init (changeRailgunSelection, Globals.materialsRailgun, "Railgun/");

			onSelectTransformTool(0);

			//onSelectMaterial (0);
			//onSelectItem (0);

			setLevelNameText ("New Level");
			setUndoButton (false);

			_txtPanelFile.color = Color.gray;
			_trfmDropDownFile.interactable = false;

			_txtPanelLevel.color = Color.gray;
			_trfmDropDownLevel.interactable = false;
		}

		//
		public void addLevelToMenu(string name) {
			if (_trfmDropDownLevel != null) {
				_trfmDropDownLevel.options.Add(new Dropdown.OptionData() { text = name });
				_iDropDownLevelOptions++;
			}
		}

		public void setLevelNameText(string name) {
			if (txtLevelName != null) {
				txtLevelName.text = name;
			}
		}

		public void setInstructionsText(string s) {
			if (txtInstructions != null) {
				txtInstructions.text = s;
			}
		}

		public void setAssetNameText(string s) {
			if (txtAssetName != null) {
				txtAssetName.text = s;
			}
		}

		public void setSpecialHelpText(string s) {
			if (txtSpecialHelp != null) {
				txtSpecialHelp.text = s;
			}
		}

		public void setCubeCountText(int count) {
			if (txtCubeCount != null) {
				txtCubeCount.text = "Objects: " + String.Format("{0:0,0}", count);
			}
		}
		public void setCubeCountText(string s) {
			if (txtCubeCount != null) {
				txtCubeCount.text = s;
			}
		}

		public void onButtonModePlayClicked() {
			PrefabLevelEditor.Instance.setEditMode(PrefabLevelEditor.EditMode.Play);
		}
		public void onButtonModeBuildClicked() {
			PrefabLevelEditor.Instance.setEditMode(PrefabLevelEditor.EditMode.Place);
		}
		public void onButtonModeSelectClicked() {
			PrefabLevelEditor.Instance.setEditMode(PrefabLevelEditor.EditMode.Transform);
		}

		//
		public void setModeButtons(PrefabLevelEditor.EditMode mode)
		{
			btnModePlay.interactable   = (mode != PrefabLevelEditor.EditMode.Play);
			btnModeBuild.interactable  = (mode != PrefabLevelEditor.EditMode.Place);
			btnModeSelect.interactable = (mode != PrefabLevelEditor.EditMode.Transform);
		}

		//
		public void setAssetTypeButtons(PrefabLevelEditor.AssetType type)
		{
			btnAssetFloors.interactable   = (type != PrefabLevelEditor.AssetType.Floor);
			btnAssetWalls.interactable    = (type != PrefabLevelEditor.AssetType.Wall);
			btnAssetChunks.interactable   = (type != PrefabLevelEditor.AssetType.Chunk);
			btnAssetProps.interactable    = (type != PrefabLevelEditor.AssetType.Prop);
			btnAssetDungeons.interactable = (type != PrefabLevelEditor.AssetType.Dungeon);
		}

		//
		public void setPlacementToolButtons(PlacementTool.PlacementMode mode)
		{
			btnPlacementToolCircle.interactable = (mode != PlacementTool.PlacementMode.Circle);
			btnPlacementToolQuad.interactable   = (mode != PlacementTool.PlacementMode.Quad);
			btnPlacementToolMount.interactable   = (mode != PlacementTool.PlacementMode.Mount);
		}

		/*public void setMenuPanels(AppState mode)
		{
			panelTools.gameObject.SetActive(mode != AppState.Play && mode != AppState.Null);
			panelFileMenu.gameObject.SetActive(mode != AppState.Play && mode != AppState.Null);
			panelLevelMenu.gameObject.SetActive(mode != AppState.Play && mode != AppState.Null);
			panelLevelMap.gameObject.SetActive(mode != AppState.Play && mode != AppState.Null);

			_txtPanelFile.color = (AppController.Instance.editorIsInOfflineMode ? Color.black : Color.gray);
			_trfmDropDownFile.interactable = AppController.Instance.editorIsInOfflineMode;

			_txtPanelLevel.color = (!AppController.Instance.editorIsInOfflineMode ? Color.black : Color.gray);
			_trfmDropDownLevel.interactable = !AppController.Instance.editorIsInOfflineMode;
		}*/

		//
		public void onButtonUNDOClicked() {
			//PrefabLevelEditor.Instance.undoLastActions ();
		}
		public void setUndoButton(bool state) {
			if (btnUNDO != null) {
				btnUNDO.interactable = state;
			}
		}

		//
		public void showTransformBox(bool state) {
			if (goTransformSelection != null) {
				goTransformSelection.SetActive (false);
			}
		}

		public void showPlacementToolBox(bool state) {
			if (goPlacementToolButtons != null) {
				goPlacementToolButtons.SetActive (state);
			}
		}

		public void showAssetTypeBox(bool state) {
			if (goAssetTypeSelection != null) {
				goAssetTypeSelection.SetActive (state);
			}
		}

		/*public void showMaterialBox(bool state) {
			if (goMaterialSelection != null) {
				goMaterialSelection.SetActive (state);
			}
		}*/

		public void showRailgunBox(bool state) {
			if (selectionBoxRailgun != null) {
				selectionBoxRailgun.show (state);
			}
		}

		/*public void showItemsBox(bool state) {
			if (goItemsSelection != null) {
				goItemsSelection.SetActive (state);
			}
		}*/

		/*public void showDigButtons(bool state) {
			if (goDigSettings != null) {
				goDigSettings.SetActive (state);
			}
		}*/

		public void showAssetInfoPanel(bool state) {
			if (panelAssetInfo != null) {
				panelAssetInfo.gameObject.SetActive(state);
			}
		}

		#endregion

		#region PrivateMethods

        private void selectTool(int toolId) {

			if (_iSelectedTool != toolId) {

				setToolButtonImage (imgMove,   (toolId == 0 ? "Textures/Tools/icon-move-selected" : "Textures/Tools/icon-move"));
				setToolButtonImage (imgRotate, (toolId == 1 ? "Textures/Tools/icon-rotate-selected" : "Textures/Tools/icon-rotate"));
				setToolButtonImage (imgScale,  (toolId == 2 ? "Textures/Tools/icon-scale-selected" : "Textures/Tools/icon-scale"));
				setToolButtonImage (imgVolume, (toolId == 3 ? "Textures/Tools/icon-volume-selected" : "Textures/Tools/icon-volume"));

				_iSelectedTool = toolId;

				PrefabLevelEditor.Instance.selectTransformTool (_iSelectedTool);
			}
        }

		//
        private void setToolButtonImage(Image img, string spriteName) {

            if (img) {
                img.sprite = Resources.Load<Sprite>(spriteName);
            }
        }

		// ---------------------------------------------------------------------------------------------
		private void selectAssetType(int value, PrefabLevelEditor.AssetType type) {

			if (_iSelectedAssetType != value) {

				setAssetTypeButtons (type);
				//btnAssetFloors.interactable = (value != 0);
				//btnAssetWalls.interactable  = (value != 1);
				//btnAssetChunks.interactable = (value != 2);
				//btnAssetProps.interactable  = (value != 3);

				_iSelectedAssetType = value;

				PrefabLevelEditor.Instance.selectAssetType (type);
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void selectPlacementTool(PlacementTool.PlacementMode mode)
		{
			setPlacementToolButtons (mode);

			PrefabLevelEditor.Instance.selectPlacementTool (mode);
		}

		// ---------------------------------------------------------------------------------------------
		/// <summary>
		/// Hack to set selected option to an invalid value!
		/// </summary>
        private void resetDropDown(Dropdown dropDown) {

            if (dropDown) {
                dropDown.options.Add(new Dropdown.OptionData() { text = "" });
                int last = dropDown.options.Count - 1;
                dropDown.value = last;
                dropDown.options.RemoveAt(last);
            }
        }

		// ---------------------------------------------------------------------------------------------
		// New Level
		// ---------------------------------------------------------------------------------------------
		private void showNewLevelDialog()
		{
			//EditorObjectSelection.Instance.ClearSelection(false);

			_popup.showPopup (Globals.PopupMode.Confirmation, "New Level", "Are you sure?\nAll unsaved changes will be lost!", createNewLevel);
		}

		//
		private void createNewLevel(int buttonId) {

			_popup.hide();
			if (buttonId == 1) {
				//PrefabLevelEditor.Instance.createNewLevel ();
			}
		}

		// ---------------------------------------------------------------------------------------------
		// Load File
		// ---------------------------------------------------------------------------------------------
        private void showLoadFileDialog() {

			//EditorObjectSelection.Instance.ClearSelection(false);

			/*
			#if UNITY_WEBGL
				AppController.Instance.showPopup(
				PopupMode.Notification,
				"Sorry!",
				"Loading level files is not available in the web version of the editor!",
				webGLPopupCallbackLoad
				);
				return;
			#endif
			*/

			_popup.showPopup (Globals.PopupMode.Confirmation, "Load Level", "Are you sure?\nAll unsaved changes will be lost!", showLoadFileBrowser);
        }

		//
		private void webGLPopupCallbackLoad(int buttonId) {

			//AppController.Instance.hidePopup ();
		}

		//
		private void showLoadFileBrowser(int buttonId) {

			_popup.hide();

			if (buttonId == 2) {
				return;
			}

			FileBrowser.OpenFilePanel("Open file Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new string[] { "json" }, null, (bool canceled, string filePath) => {
				if (!canceled) {
					//LevelData.Instance.loadLevelDataFromFile(filePath);
				}
			});
		}

		// ---------------------------------------------------------------------------------------------
		// Save Level
		// ---------------------------------------------------------------------------------------------
        private void showSaveFileDialog() {

			//EditorObjectSelection.Instance.ClearSelection(false);

			/*
			#if UNITY_WEBGL
				AppController.Instance.showPopup(
				PopupMode.Notification,
				"Sorry!",
				"Saving level files is not available in the web version of the editor!",
				webGLPopupCallbackSave
				);
				return;
			#endif
			*/

			_popup.showPopup(Globals.PopupMode.Input, "Save Level", "Level Name: (50 chars max)", "Enter Level Name...", showSaveFileDialogContinue);
        }

		//
		private void webGLPopupCallbackSave(int buttonId) {

			//AppController.Instance.hidePopup ();
		}

		//
		private void showSaveFileDialogContinue(int buttonId) {

			if (buttonId == 1)
			{
				string levelName = _popup.inputText;
				_popup.hide ();
				if (levelName == null || levelName == "") {
					levelName = Globals.defaultLevelName;
				}
				setLevelNameText (levelName);

				FileBrowser.SaveFilePanel ("Save Level", "Save Level", Environment.GetFolderPath (Environment.SpecialFolder.Desktop), levelName, new string[] { "json" }, null, (bool canceled, string filePath) => {
					if (!canceled) {
						//LevelData.Instance.saveLevelData (filePath, levelName);
					}
				});
			}
			else
			{
				_popup.hide ();
			}
		}

		// ---------------------------------------------------------------------------------------------
		// Load Level
		// ---------------------------------------------------------------------------------------------
		/*private void showLoadLevelDialog(int value, string name) {

			_iSelectedLevel = value;

			EditorObjectSelection.Instance.ClearSelection(false);

			int levelId = LevelManager.Instance.getLevelIdByIndex (value);
			if (levelId == LevelData.Instance.currentLevelId) {
				_popup.showPopup (PopupMode.Confirmation, "Load Level '"+name+"'", "Level already loaded!\nReload Level?", showLoadLevelDialogContinue);
			}
			else {
				_popup.showPopup (PopupMode.Confirmation, "Load Level '"+name+"'", "Are you sure?\nAll unsaved changes will be lost!", showLoadLevelDialogContinue);
			}
		}

		private void showLoadLevelDialogContinue(int buttonId) {

			_popup.hide ();

			if (buttonId == 1)
			{
				LevelManager.Instance.loadLevelByIndex(_iSelectedLevel);
				_iSelectedLevel = -1;
			}
		}*/

		/*
		public void toggleMaterial(int toggle)
		{
			//Debug.Log ("_iSelectedMaterial before: " + _iSelectedMaterial);
			int materialIndex = _iSelectedMaterial;
			if (toggle < 0) {
				materialIndex = (materialIndex > 0 ? materialIndex - 1 : 0);
			} else {
				materialIndex = (materialIndex < (Globals.materials.Length - 1) ? materialIndex + 1 : (Globals.materials.Length - 1));
			}
			//Debug.Log ("_iSelectedMaterial after: " + materialIndex);

			//Debug.Log ("_iMaterialBoxIndex before: " + _iMaterialBoxIndex);
			//Debug.Log ("_iMatHiliteIndex before: " + _iMatHiliteIndex);

			_iMatHiliteIndex += toggle;
			if (_iMatHiliteIndex < 0) {
				_iMatHiliteIndex = 0;
				_iMaterialBoxIndex = (_iMaterialBoxIndex > 0 ? _iMaterialBoxIndex - 1 : 0);
			}
			else if (_iMatHiliteIndex > 5) {
				_iMatHiliteIndex = 5;
				int maxBoxIndex = Globals.materials.Length - 6;
				_iMaterialBoxIndex = (_iMaterialBoxIndex < maxBoxIndex ? _iMaterialBoxIndex + 1 : maxBoxIndex);
			}

			//Debug.Log ("_iMaterialBoxIndex after: " + _iMaterialBoxIndex);
			//Debug.Log ("_iMatHiliteIndex after: " + _iMatHiliteIndex);

			imgMaterialHilight.transform.localPosition = new Vector2 (_iMatHiliteIndex * 38, 0);

			int i;
			for (i = 1; i <= 6; ++i) {
				Transform child = goMaterialSelection.transform.Find ("Material-" + i.ToString ());
				if (child != null) {
					child.GetComponent<RawImage> ().texture = LevelEditor.Instance.aTextures[_iMaterialBoxIndex + (i-1)];
				}
			}

			changeMaterial (materialIndex);
		}

		//
		private void changeMaterial(int materialIndex) {

			if (materialIndex >= 0 && materialIndex < Globals.materials.Length) {

				_iSelectedMaterial = materialIndex;

				LevelEditor.Instance.newMaterialSelected (_iSelectedMaterial);
			}
		}*/

		//
		private void changeRailgunSelection()
		{
			//PrefabLevelEditor.Instance.newRailgunSelected (selectionBoxRailgun.iSelected);
		}

		/*public void toggleItem(float toggle)
		{
			if (Time.realtimeSinceStartup > _lastMouseWheelUpdate) {

				_lastMouseWheelUpdate = Time.realtimeSinceStartup + 0.2f;

				int itemIndex = _iSelectedItem;
				if (toggle < 0) {
					itemIndex = (itemIndex > 0 ? itemIndex - 1 : 0);//Globals.items.Count - 1);
				} else {
					itemIndex = (itemIndex < (Globals.items.Count - 1) ? itemIndex + 1 : (Globals.items.Count - 1));//0);
				}

				changeItem (itemIndex);
			}
		}

		//
		private void changeItem(int itemIndex) {

			if (itemIndex >= 0 && itemIndex < Globals.items.Count) {

				_iSelectedItem = itemIndex;

				if (goItemsSelection != null && imgSelectedItem != null) {

					Transform child = goItemsSelection.transform.Find ("Item-" + (itemIndex + 1).ToString ());
					if (child != null) {
						imgSelectedItem.texture = child.GetComponent<RawImage> ().texture;
					}
				}

				LevelEditor.Instance.newItemSelected (_iSelectedItem);
			}
		}*/

		/*public void resetDigSettings(Vector3 v3Set)
		{
			_v3DigSettings = v3Set;
			sliderDigWidth.value  = _v3DigSettings.x;
			sliderDigHeight.value = _v3DigSettings.y;
			sliderDigDepth.value  = _v3DigSettings.z;
			updateSliderValueText (sliderDigWidth.transform.parent,  _v3DigSettings.x);
			updateSliderValueText (sliderDigHeight.transform.parent, _v3DigSettings.y);
			updateSliderValueText (sliderDigDepth.transform.parent,  _v3DigSettings.z);
		}

		//
		public void setDigSliders(AppState mode)
		{
			//if (sliderDigDepth != null) {
			//	sliderDigDepth.transform.parent.gameObject.SetActive (mode != AppState.Paint);
			//}
		}*/

		/*
		public void toggleDigSize(float toggle)
		{
			if (Time.realtimeSinceStartup > _lastMouseWheelUpdate) {

				_lastMouseWheelUpdate = Time.realtimeSinceStartup + 0.2f;

				float value;

				if (sliderDigWidth != null) {
					value = sliderDigWidth.value + (toggle < 0 ? -1 : 1);
					if (value > sliderDigWidth.maxValue) {
						value = sliderDigWidth.maxValue;//sliderDigWidth.minValue;
					} else if (value < sliderDigWidth.minValue) {
						value = sliderDigWidth.minValue;//sliderDigWidth.maxValue;
					}
					sliderDigWidth.value = value;
					_v3DigSettings.x = (int)sliderDigWidth.value;
					updateSliderValueText (sliderDigWidth.transform.parent, sliderDigWidth.value);
				}

				if (sliderDigHeight != null) {
					value = sliderDigHeight.value + (toggle < 0 ? -1 : 1);
					if (value > sliderDigHeight.maxValue) {
						value = sliderDigHeight.maxValue;//sliderDigHeight.minValue;
					} else if (value < sliderDigHeight.minValue) {
						value = sliderDigHeight.minValue;//sliderDigHeight.maxValue;
					}
					sliderDigHeight.value = value;
					_v3DigSettings.y = (int)sliderDigHeight.value;
					updateSliderValueText (sliderDigHeight.transform.parent, sliderDigHeight.value);
				}

				if (sliderDigDepth != null) {
					value = sliderDigDepth.value + (toggle < 0 ? -1 : 1);
					if (value > sliderDigDepth.maxValue) {
						value = sliderDigDepth.maxValue;//sliderDigHeight.minValue;
					} else if (value < sliderDigDepth.minValue) {
						value = sliderDigDepth.minValue;//sliderDigHeight.maxValue;
					}
					sliderDigDepth.value = value;
					_v3DigSettings.z = (int)sliderDigDepth.value;
					updateSliderValueText (sliderDigDepth.transform.parent, sliderDigDepth.value);
				}

				LevelEditor.Instance.updateDigSettings(_v3DigSettings);
			}
		}*/

		//
		/*private void onGizmoChanged(GizmoType newGizmoType) {

			if (newGizmoType == GizmoType.Translation) {
				selectTool (0);
			} else if (newGizmoType == GizmoType.Rotation) {
				selectTool (1);
			} else if (newGizmoType == GizmoType.Scale) {
				selectTool (2);
			} else if (newGizmoType == GizmoType.VolumeScale) {
				selectTool (3);
			}
		}*/

        private void onGameObjectClicked(GameObject clickedObject) {

            //Debug.Log("onGameObjectClicked "+clickedObject.name);
			if (clickedObject.GetComponent<Rigidbody> () != null) {
				clickedObject.GetComponent<Rigidbody> ().useGravity = false;
			}
        }

		//private void onSelectionChanged(ObjectSelectionChangedEventArgs selectionChangedEventArgs) {
			//PrefabLevelEditor.Instance.setSelectedObjects (selectionChangedEventArgs.SelectedObjects);
		//}

        #endregion

        /*public void onButtonPlayClicked() {

            if (_gizmoSystem) {
                _gizmoSystem.TurnOffGizmos();
            }

            AppController.Instance.switchToPlayMode();
        }*/

		//
        public void onSelectTransformTool(int value) {
            if (value == 0) {
                //_gizmoSystem.ActiveGizmoType = GizmoType.Translation;
                selectTool(0);
            }
            else if (value == 1) {
                //_gizmoSystem.ActiveGizmoType = GizmoType.Rotation;
                selectTool(1);
            }
            else if (value == 2) {
                //_gizmoSystem.ActiveGizmoType = GizmoType.Scale;
                selectTool(2);
            }
            else if (value == 3) {
                //_gizmoSystem.ActiveGizmoType = GizmoType.VolumeScale;
                selectTool(3);
            }
        }

		//
		public void onSelectAssetType(int value) {
				
			if (value == 0) {
				selectAssetType(value, PrefabLevelEditor.AssetType.Floor);
			}
			else if (value == 1) {
				selectAssetType(value, PrefabLevelEditor.AssetType.Wall);
			}
			else if (value == 2) {
				selectAssetType(value, PrefabLevelEditor.AssetType.Chunk);
			}
			else if (value == 3) {
				selectAssetType(value, PrefabLevelEditor.AssetType.Prop);
			}
			else if (value == 4) {
				selectAssetType(value, PrefabLevelEditor.AssetType.Dungeon);
			}
		}

		//
		public void onSelectPlacementTool(int value) {

			if (value == 0) {
				selectPlacementTool(PlacementTool.PlacementMode.Circle);
			}
			else if (value == 1) {
				selectPlacementTool(PlacementTool.PlacementMode.Quad);
			}
			else if (value == 2) {
				selectPlacementTool(PlacementTool.PlacementMode.Mount);
			}
		}

		// -------------------------------------------------------------------------------------
		public void onSelectMaterial(int value)
		{
			_iMatHiliteIndex = value;
			imgMaterialHilight.transform.localPosition = new Vector2 (_iMatHiliteIndex * 38, 0);

			//int newMatIndex = _iMaterialBoxIndex + _iMatHiliteIndex;
			//changeMaterial (newMatIndex);
		}

		public void onSelectItem(int value) {
			//changeItem (value);
		}

		// -------------------------------------------------------------------------------------
		/*public void onSliderWidthChange(Single value) {
			if (sliderDigWidth != null) {
				updateSliderValueText (sliderDigWidth.transform.parent, sliderDigWidth.value);
				_v3DigSettings.x = (int)sliderDigWidth.value;
				LevelEditor.Instance.updateDigSettings(_v3DigSettings);
				//Debug.Log ("slider width value: "+(int)sliderDigWidth.value);
			}
		}
		public void onSliderHeightChange(Single value) {
			if (sliderDigHeight != null) {
				updateSliderValueText (sliderDigHeight.transform.parent, sliderDigHeight.value);
				_v3DigSettings.y = (int)sliderDigHeight.value;
				LevelEditor.Instance.updateDigSettings(_v3DigSettings);
				//Debug.Log ("slider height value: "+(int)sliderDigHeight.value);
			}
		}
		public void onSliderDepthChange(Single value) {
			if (sliderDigDepth != null) {
				updateSliderValueText (sliderDigDepth.transform.parent, sliderDigDepth.value);
				_v3DigSettings.z = (int)sliderDigDepth.value;
				LevelEditor.Instance.updateDigSettings(_v3DigSettings);
				//Debug.Log ("slider depth value: "+(int)sliderDigDepth.value);
			}
		}
		//
		private void updateSliderValueText(Transform parent, float value) {
			if (parent == null) {
				return;
			}
			Transform trfmText = parent.Find ("Value");
			if (trfmText != null) {
				trfmText.GetComponent<Text> ().text = ((int)value).ToString ();
			}
		}*/

		// -------------------------------------------------------------------------------------
        public void onPointerDownFile(BaseEventData data) {
            if (_trfmDropDownFile) {
                resetDropDown(_trfmDropDownFile);
				//LevelEditor.Instance.setMode (AppState.Select);
            }
        }

		public void onPointerDownLevel(BaseEventData data) {
			if (_trfmDropDownLevel) {
				resetDropDown(_trfmDropDownLevel);
				//LevelEditor.Instance.setMode (AppState.Select);
			}
		}

		//
        public void onDropDownFileValueChanged(int value) {
            if (_trfmDropDownFile && value < _iDropDownFileOptions) {
                if (value == 0) {
                    showNewLevelDialog();
                } else if (value == 1) {
					showLoadFileDialog();
				} else if (value == 2) {
					showSaveFileDialog();
				}
            }
        }

		public void onDropDownLevelValueChanged(int value) {
			if (_trfmDropDownLevel && value < _iDropDownLevelOptions) {
				//_iSelectedLevel = value;
				//LevelEditor.Instance.teleportToLevelWithIndex(_iSelectedLevel);
			}
		}
    }
}