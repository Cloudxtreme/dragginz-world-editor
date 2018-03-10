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

using RTEditor;

namespace DragginzWorldEditor
{
	public class MainMenu : MonoSingleton<MainMenu>
    {
		public GameObject goTransformSelection;
		public GameObject goMaterialSelection;
		public GameObject goItemsSelection;
		public GameObject goDigSettings;

		public Transform panelTools;
        public Transform panelFileMenu;
		public Transform panelLevelMenu;
        public Transform blocker;
        public Transform panelPopup;

		public Button btnModePlay;
		public Button btnModeLook;
		public Button btnModeDig;
		public Button btnModePaint;
		public Button btnModeBuild;
		public Button btnModeProps;

		public Button btnUNDO;

		public RawImage imgSelectedMaterial;
		public RawImage imgSelectedItem;

		public Slider sliderDigWidth;
		public Slider sliderDigHeight;
		public Slider sliderDigDepth;

		public Text txtFileInfo;
		public Text txtLevelName;
		public Text txtCubeCount;
		public Text txtMovementSpeed;
		public Text txtCameraPosition;

        public Image imgMove;
        public Image imgRotate;
        public Image imgScale;
        public Image imgVolume;

        private EditorGizmoSystem _gizmoSystem;
            
        private Dropdown _trfmDropDownFile = null;
        private int _iDropDownFileOptions = 0;

		private Dropdown _trfmDropDownLevel = null;
		private int _iDropDownLevelOptions = 0;
		private int _iSelectedLevel = -1;

        private int _iSelectedTool = -1;

		private Vector3 _v3DigSettings = new Vector3(1,1,1);
		public Vector3 v3DigSettings {
			get { return _v3DigSettings; }
		}

		private float _lastMouseWheelUpdate;

		private int _iSelectedMaterial = 0;
		public int iSelectedMaterial {
			get { return _iSelectedMaterial; }
		}

		/*private int _iSelectedItem = 0;
		public int iSelectedItem {
			get { return _iSelectedItem; }
		}*/

        private Popup _popup;
        public Popup popup {
            get { return _popup; }
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
                _popup = panelPopup.GetComponent<Popup>();
            }

			Transform trfmMenu;
            if (panelFileMenu) {
                trfmMenu = panelFileMenu.Find("DropdownFile");
                if (trfmMenu) {
                    _trfmDropDownFile = trfmMenu.GetComponent<Dropdown>();
                    if (_trfmDropDownFile) {
                        _iDropDownFileOptions = _trfmDropDownFile.options.Count;
                    }
                }
            }

			if (panelLevelMenu) {
				trfmMenu = panelLevelMenu.Find("DropdownFile");
				if (trfmMenu) {
					_trfmDropDownLevel = trfmMenu.GetComponent<Dropdown>();
					if (_trfmDropDownLevel) {
						_iDropDownLevelOptions = _trfmDropDownLevel.options.Count;
					}
				}
			}

			if (sliderDigWidth != null) {
				sliderDigWidth.value = sliderDigWidth.minValue = 1;
				sliderDigWidth.maxValue = 5;
			}
			if (sliderDigHeight != null) {
				sliderDigHeight.value = sliderDigHeight.minValue = 1;
				sliderDigHeight.maxValue = 5;
			}
			if (sliderDigDepth != null) {
				sliderDigDepth.value = sliderDigDepth.minValue = 1;
				sliderDigDepth.maxValue = 5;
			}

			_lastMouseWheelUpdate = 0;
        }

		void OnEnable() {
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
		}

        void Start() {
			onSelectTransformTool(0);
			onSelectMaterial (0);
			onSelectItem (0);
			setLevelNameText ("New Level");
        }

		#endregion

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

		public void setCameraPositionText(Vector3 pos) {
			if (txtCameraPosition != null) {
				txtCameraPosition.text = "Position: x" + pos.x.ToString("F2") + ", y"+ pos.y.ToString("F2") + ", z" + pos.z.ToString("F2");
			}
		}

		public void setMovementSpeedText(float speed) {
			if (txtMovementSpeed != null) {
				txtMovementSpeed.text = "Movement Speed: " + speed.ToString("F2");
			}
		}

		public void setCubeCountText(int count)
		{
			if (txtCubeCount != null) {
				txtCubeCount.text = "Cubes: " + String.Format("{0:0,0}", count);//count.ToString();
			}
		}

		//
		public void onButtonModePlayClicked() {
			LevelEditor.Instance.setMode(AppState.Play);
		}
		public void onButtonModeLookClicked() {
			LevelEditor.Instance.setMode(AppState.Select);
		}
		public void onButtonModeDigClicked() {
			LevelEditor.Instance.setMode(AppState.Dig);
		}
		public void onButtonModePaintClicked() {
			LevelEditor.Instance.setMode(AppState.Paint);
		}
		public void onButtonModeBuildClicked() {
			LevelEditor.Instance.setMode(AppState.Build);
		}
		public void onButtonModePropsClicked() {
			LevelEditor.Instance.setMode(AppState.Props);
		}

		//
		public void setModeButtons(AppState mode)
		{
			btnModePlay.interactable  = (mode != AppState.Play);
			btnModeLook.interactable  = (mode != AppState.Select);
			btnModeDig.interactable   = (mode != AppState.Dig);
			btnModePaint.interactable = (mode != AppState.Paint);
			btnModeBuild.interactable = (mode != AppState.Build);
			btnModeProps.interactable = (mode != AppState.Props);
		}

		public void setMenuPanels(AppState mode)
		{
			panelTools.gameObject.SetActive(mode != AppState.Play && mode != AppState.Null);
			panelFileMenu.gameObject.SetActive(mode != AppState.Play && mode != AppState.Null);
			panelLevelMenu.gameObject.SetActive(mode != AppState.Play && mode != AppState.Null);
		}

		//
		public void onButtonUNDOClicked() {
			LevelEditor.Instance.undoLastActions ();
		}
		public void setUndoButton(bool state) {
			btnUNDO.interactable = state;
		}

		//
		public void showTransformBox(bool state) {
			if (goTransformSelection != null) {
				goTransformSelection.SetActive (state);
			}
		}

		public void showMaterialBox(bool state) {
			if (goMaterialSelection != null) {
				goMaterialSelection.SetActive (state);
			}
		}

		public void showItemsBox(bool state) {
			if (goItemsSelection != null) {
				goItemsSelection.SetActive (state);
			}
		}

		public void showDigButtons(bool state) {
			if (goDigSettings != null) {
				goDigSettings.SetActive (state);
			}
		}

		#region PrivateMethods

        private void selectTool(int toolId) {

            if (_iSelectedTool == toolId) {
                return;
            }

            setToolButtonImage(imgMove,   (toolId == 0 ? "Sprites/icon-move-selected"   : "Sprites/icon-move"));
            setToolButtonImage(imgRotate, (toolId == 1 ? "Sprites/icon-rotate-selected" : "Sprites/icon-rotate"));
            setToolButtonImage(imgScale,  (toolId == 2 ? "Sprites/icon-scale-selected"  : "Sprites/icon-scale"));
            setToolButtonImage(imgVolume, (toolId == 3 ? "Sprites/icon-volume-selected" : "Sprites/icon-volume"));

            _iSelectedTool = toolId;
        }

		//
        private void setToolButtonImage(Image img, string spriteName) {

            if (img) {
                img.sprite = Resources.Load<Sprite>(spriteName);
            }
        }

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

		//
		private void showNewLevelDialog()
		{
			EditorObjectSelection.Instance.ClearSelection(false);

			if (_popup) {
				_popup.showPopup (PopupMode.Confirmation, "New Level", "Are you sure?\nAll unsaved changes will be lost!", createNewLevel);
			} else {
				showLoadFileBrowser (1);
			}
		}

		//
		private void createNewLevel(int buttonId) {

			_popup.hide();
			if (buttonId == 1) {
				LevelEditor.Instance.createNewLevel ();
			}
		}

		//
        private void showLoadFileDialog() {

			EditorObjectSelection.Instance.ClearSelection(false);

			if (_popup) {
				_popup.showPopup (PopupMode.Confirmation, "Load Level", "Are you sure?\nAll unsaved changes will be lost!", showLoadFileBrowser);
			} else {
				showLoadFileBrowser (1);
			}
        }

		//
		private void showLoadFileBrowser(int buttonId) {

			_popup.hide();

			if (buttonId == 2) {
				return;
			}

			FileBrowser.OpenFilePanel("Open file Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new string[] { "json" }, null, (bool canceled, string filePath) => {
				if (!canceled) {
					LevelData.Instance.loadLevelDataFromFile(LevelEditor.Instance.goWorld, filePath);
				}
			});
		}

		//
        private void showSaveFileDialog() {

			EditorObjectSelection.Instance.ClearSelection(false);

			_popup.showPopup(PopupMode.Input, "Save Level", "Level Name: (50 chars max)", "Enter Level Name...", showSaveFileDialogContinue);
        }

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
						LevelData.Instance.saveLevelData (filePath, levelName);
					}
				});
			}
			else
			{
				_popup.hide ();
			}
		}

		//
		private void showLoadLevelDialog(int value, string name) {

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
		}

		//
		public void toggleMaterial(float toggle)
		{
			if (Time.realtimeSinceStartup > _lastMouseWheelUpdate) {

				_lastMouseWheelUpdate = Time.realtimeSinceStartup + 0.2f;

				int materialIndex = _iSelectedMaterial;
				if (toggle < 0) {
					materialIndex = (materialIndex > 0 ? materialIndex - 1 : 0);//Globals.materials.Length - 1);
				} else {
					materialIndex = (materialIndex < (Globals.materials.Length - 1) ? materialIndex + 1 : (Globals.materials.Length - 1));//0);
				}

				changeMaterial (materialIndex);
			}
		}

		//
		private void changeMaterial(int materialIndex) {

			if (materialIndex >= 0 && materialIndex < Globals.materials.Length) {

				_iSelectedMaterial = materialIndex;

				if (goMaterialSelection != null && imgSelectedMaterial != null) {

					Transform child = goMaterialSelection.transform.Find ("Material-" + (materialIndex + 1).ToString ());
					if (child != null) {
						imgSelectedMaterial.texture = child.GetComponent<RawImage> ().texture;
					}
				}

				LevelEditor.Instance.newMaterialSelected (_iSelectedMaterial);

				/*List<GameObject> allSelectedObjects = new List<GameObject> (EditorObjectSelection.Instance.SelectedGameObjects);
				foreach (GameObject selectedObject in allSelectedObjects) {

					MeshRenderer renderer = selectedObject.GetComponent<MeshRenderer> ();
					if (renderer != null) {
						renderer.material = Resources.Load<Material> ("Materials/" + Globals.materials [materialIndex]);
						//Debug.Log ("new material: *" + renderer.material.name.Replace(" (Instance)","") + "*");
					}
				}*/
			}
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

		public void resetDigSettings(Vector3 v3Set)
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
			if (sliderDigDepth != null) {
				sliderDigDepth.transform.parent.gameObject.SetActive (mode != AppState.Paint);
			}
		}

		//
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
		}

		//
		private void onGizmoChanged(GizmoType newGizmoType) {

			if (newGizmoType == GizmoType.Translation) {
				selectTool (0);
			} else if (newGizmoType == GizmoType.Rotation) {
				selectTool (1);
			} else if (newGizmoType == GizmoType.Scale) {
				selectTool (2);
			} else if (newGizmoType == GizmoType.VolumeScale) {
				selectTool (3);
			}
		}

        private void onGameObjectClicked(GameObject clickedObject) {

            //Debug.Log("onGameObjectClicked "+clickedObject.name);
			if (clickedObject.GetComponent<Rigidbody> () != null) {
				clickedObject.GetComponent<Rigidbody> ().useGravity = false;
			}
        }

		private void onSelectionChanged(ObjectSelectionChangedEventArgs selectionChangedEventArgs) {
			
			LevelEditor.Instance.setSelectedObjects (selectionChangedEventArgs.SelectedObjects);
		}

        #endregion

        /*public void onButtonPlayClicked() {

            if (_gizmoSystem) {
                _gizmoSystem.TurnOffGizmos();
            }

            AppController.Instance.switchToPlayMode();
        }*/

		/// <summary>
		/// ...
		/// </summary>
        public void onSelectTransformTool(int value) {
            if (_gizmoSystem) {
                if (value == 0) {
                    _gizmoSystem.ActiveGizmoType = GizmoType.Translation;
                    selectTool(0);
                }
                else if (value == 1) {
                    _gizmoSystem.ActiveGizmoType = GizmoType.Rotation;
                    selectTool(1);
                }
                else if (value == 2) {
                    _gizmoSystem.ActiveGizmoType = GizmoType.Scale;
                    selectTool(2);
                }
                else if (value == 3) {
                    _gizmoSystem.ActiveGizmoType = GizmoType.VolumeScale;
                    selectTool(3);
                }
            }
        }

		// -------------------------------------------------------------------------------------
		public void onSelectMaterial(int value) {
			changeMaterial (value);
		}

		public void onSelectItem(int value) {
			//changeItem (value);
		}

		// -------------------------------------------------------------------------------------
		public void onSliderWidthChange(Single value) {
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
		}

		// -------------------------------------------------------------------------------------
        public void onPointerDownFile(BaseEventData data) {
            if (_trfmDropDownFile) {
                resetDropDown(_trfmDropDownFile);
				LevelEditor.Instance.setMode (AppState.Select);
            }
        }

		public void onPointerDownLevel(BaseEventData data) {
			if (_trfmDropDownLevel) {
				resetDropDown(_trfmDropDownLevel);
				LevelEditor.Instance.setMode (AppState.Select);
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
				showLoadLevelDialog (value, _trfmDropDownLevel.options [value].text);
			}
		}
    }
}