//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace DragginzWorldEditor
{
	public class LevelData : Singleton<LevelData> {

		//
		public void loadLevelData(GameObject parent, string filename) {
			
			//BinaryFormatter bf = new BinaryFormatter();
			//FileStream file = File.Open(filename, FileMode.Open);
			string json = File.ReadAllText(filename);

			LevelFile levelFile = null;
			try {
				//levelFile = bf.Deserialize(file) as LevelFile;
				levelFile = createDataFromJson(json);
				if (levelFile != null) {
					createLevel (levelFile, parent);
				}
			}
			catch (System.Exception e) {
				Debug.LogWarning (e.Message);
				AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningInvalidFileFormat);
			}

			//file.Close();
			//file.Dispose();
		}

		//
		private LevelFile createDataFromJson(string json) {

			LevelFile levelFile = new LevelFile ();

			levelFile.parseJson (json);

			return levelFile;
		}

		//
		private void createLevel(LevelFile levelFile, GameObject parent) {

			if (levelFile.fileFormatVersion != Globals.levelSaveFormatVersion) {
				AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningObsoleteFileFormat);
				return;
			}

			MainMenu.Instance.setLevelNameText(levelFile.levelName);

			LevelEditor levelEditor = LevelEditor.Instance;
			PropsManager propsManager = PropsManager.Instance;
			World world = World.Instance;

			levelEditor.resetAll ();

			Vector3 savedPos = new Vector3 (levelFile.playerPosition.x, levelFile.playerPosition.y, levelFile.playerPosition.z);
			Vector3 savedRot = new Vector3 (levelFile.playerEuler.x, levelFile.playerEuler.y, levelFile.playerEuler.z);
			FlyCam.Instance.setNewInitialPosition (savedPos, savedRot);
			FlyCam.Instance.reset ();

			GameObject goQuadrant;
			GameObject container;
			Vector3 pos = Vector3.zero;
			int i, len = levelFile.levelQuadrants.Count;
			for (i = 0; i < len; ++i)
			{
				pos.x = (int)levelFile.levelQuadrants[i].position.x;
				pos.y = (int)levelFile.levelQuadrants[i].position.y;
				pos.z = (int)levelFile.levelQuadrants[i].position.z;
			
				string quadrantId = (int)pos.x + "_" + (int)pos.y + "_" + (int)pos.z;
				goQuadrant = world.createQuadrant (pos, quadrantId);

				container = world.createContainer (goQuadrant.transform);

				Vector3 pos2 = Vector3.zero;
				Material material;
				bool isEdge = false;
				int j, len2 = levelFile.levelQuadrants [i].levelObjects.Count;
				for (j = 0; j < len2; ++j)
				{
					pos2.x = levelFile.levelQuadrants [i].levelObjects [j].position.x;
					pos2.y = levelFile.levelQuadrants [i].levelObjects [j].position.y;
					pos2.z = levelFile.levelQuadrants [i].levelObjects [j].position.z;
					if (levelFile.levelQuadrants [i].isEdge == 1) {
						material = null;
						isEdge = true;
					} else {
						material = levelEditor.aDictMaterials [levelFile.levelQuadrants [i].levelObjects [j].material];
						isEdge = false;
					}
					world.createRock (pos2, container, levelFile.levelQuadrants [i].levelObjects [j].name, material, isEdge);
				}
			}

			MainMenu.Instance.setCubeCountText (World.Instance.numCubes);

			if (levelFile.levelProps != null) {
				
				LevelProp levelProp;
				GameObject goProp;
				Quaternion rotation = Quaternion.identity;
				propDef prop;

				len = levelFile.levelProps.Count;
				for (i = 0; i < len; ++i) {
					levelProp = levelFile.levelProps [i];

					pos.x = levelProp.position.x;
					pos.y = levelProp.position.y;
					pos.z = levelProp.position.z;

					prop = propsManager.getPropDefForId(levelProp.id);
					if (prop.id != -1) {
					
						goProp = world.createProp (prop, pos, levelProp.name, levelEditor.goProps.transform, prop.useCollider, prop.useGravity);

						rotation.w = levelProp.rotation.w;
						rotation.x = levelProp.rotation.x;
						rotation.y = levelProp.rotation.y;
						rotation.z = levelProp.rotation.z;
						goProp.transform.rotation = rotation;

						PropsManager.Instance.addWorldProp (prop.id, goProp);
					}
				}
			}
		}

		//
		public void saveLevelData(string filename, string levelName) {

			LevelFile levelFile = createLevelData (levelName);
			if (levelFile == null) {
				return;
			}

			string json = levelFile.getJsonString();

			//BinaryFormatter bf = new BinaryFormatter();

			//FileStream file = File.Open(filename, FileMode.OpenOrCreate);

			File.WriteAllText (filename, json);

			//StreamWriter writer = new StreamWriter (file, System.Text.Encoding.ASCII);
			//writer.Write (json);
			//writer.Flush ();

			//bf.Serialize(file, levelFile);

			//file.Flush ();
			//file.Close();
			//file.Dispose();

			//writer.Close ();
			//writer.Dispose ();
		}

		//
		private LevelFile createLevelData(string levelName) {

			GameObject parent = LevelEditor.Instance.goWorld;
			if (parent == null) {
				return null;
			}

			LevelFile levelFile         = new LevelFile ();
			levelFile.fileFormatVersion = Globals.levelSaveFormatVersion;
			levelFile.levelName         = levelName;

			levelFile.playerPosition   = new DataTypeVector3 ();
			levelFile.playerPosition.x = FlyCam.Instance.player.position.x;
			levelFile.playerPosition.y = FlyCam.Instance.player.position.y;
			levelFile.playerPosition.z = FlyCam.Instance.player.position.z;

			levelFile.playerEuler   = new DataTypeVector3 ();
			levelFile.playerEuler.x = FlyCam.Instance.player.eulerAngles.x;
			levelFile.playerEuler.y = FlyCam.Instance.player.eulerAngles.y;
			levelFile.playerEuler.z = FlyCam.Instance.player.eulerAngles.z;

			levelFile.levelQuadrants    = new List<LevelQuadrant> ();

			foreach (Transform child in parent.transform) {

				if (!child.gameObject.activeSelf) {
					continue;
				}

				LevelQuadrant quadrant = new LevelQuadrant ();
				quadrant.name = child.name;
				//Debug.LogWarning ("quadrant "+quadrant.name);

				quadrant.position   = new DataTypeVector3 ();
				quadrant.position.x = child.localPosition.x;
				quadrant.position.y = child.localPosition.y;
				quadrant.position.z = child.localPosition.z;

				int qX = (int)quadrant.position.x;
				int qY = (int)quadrant.position.y;
				int qZ = (int)quadrant.position.z;
				bool isEdgeQuadrant = ((qX == -1 || qY == -1 || qZ == -1) || (qX == Globals.LEVEL_WIDTH || qY == Globals.LEVEL_HEIGHT || qZ == Globals.LEVEL_DEPTH));
				quadrant.isEdge = (isEdgeQuadrant ? 1 : 0);

				quadrant.levelObjects = new List<LevelObject> ();

				Transform container = child.Find ("container");
				if (container != null)
				{
					foreach (Transform cube in container) {

						if (!cube.gameObject.activeSelf) {
							continue;
						}

						LevelObject cubeObject = new LevelObject ();
						cubeObject.name = cube.name;
						//Debug.Log ("    ->cube "+cubeObject.name);

						cubeObject.position   = new DataTypeVector3 ();
						cubeObject.position.x = cube.localPosition.x;
						cubeObject.position.y = cube.localPosition.y;
						cubeObject.position.z = cube.localPosition.z;

						if (isEdgeQuadrant) {
							cubeObject.material = "";
						}
						else {
							MeshRenderer renderer = cube.GetComponent<MeshRenderer> ();
							if (renderer != null) {
								cubeObject.material = renderer.material.name.Replace (" (Instance)", "");
							}
						}

						quadrant.levelObjects.Add (cubeObject);
					}
				}

				levelFile.levelQuadrants.Add (quadrant);
			}

			// PROPS

			List<LevelProp> levelProps = new List<LevelProp> ();

			Dictionary<GameObject, worldProp> worldProps = PropsManager.Instance.worldProps;
			foreach (KeyValuePair<GameObject, worldProp> p in worldProps) {

				worldProp prop = p.Value;

				if (!prop.go.activeSelf) {
					continue;
				}

				int propId = prop.id;
				if (propId <= 0) {
					continue;
				}

				LevelProp levelProp = new LevelProp ();
				levelProp.id   = propId;
				levelProp.name = prop.name;

				levelProp.position   = new DataTypeVector3 ();
				levelProp.position.x = prop.go.transform.position.x;
				levelProp.position.y = prop.go.transform.position.y;
				levelProp.position.z = prop.go.transform.position.z;

				levelProp.rotation = new DataTypeQuaternion ();
				levelProp.rotation.w = prop.go.transform.rotation.w;
				levelProp.rotation.x = prop.go.transform.rotation.x;
				levelProp.rotation.y = prop.go.transform.rotation.y;
				levelProp.rotation.z = prop.go.transform.rotation.z;

				levelProps.Add (levelProp);	
			}

			levelFile.levelProps = levelProps;

			/*parent = LevelEditor.Instance.goProps;
			if (parent != null) {

				List<LevelProp> levelProps = new List<LevelProp> ();

				foreach (Transform prop in parent.transform) {

					if (!prop.gameObject.activeSelf) {
						continue;
					}

					int propId = -1;//Globals.getItemIndexFromName (item.name);
					if (propId == -1) {
						continue;
					}

					LevelProp levelProp = new LevelProp ();
					levelProp.id   = propId;
					levelProp.name = prop.name;

					levelProp.position   = new DataTypeVector3 ();
					levelProp.position.x = prop.position.x;
					levelProp.position.y = prop.position.y;
					levelProp.position.z = prop.position.z;

					levelProp.rotation = new DataTypeQuaternion ();
					levelProp.rotation.w = prop.rotation.w;
					levelProp.rotation.x = prop.rotation.x;
					levelProp.rotation.y = prop.rotation.y;
					levelProp.rotation.z = prop.rotation.z;

					levelProps.Add (levelProp);
				}

				levelFile.levelProps = levelProps;
			}*/

			return levelFile;
		}
	}
}