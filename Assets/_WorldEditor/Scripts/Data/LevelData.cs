//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace DragginzWorldEditor
{
	public class LevelData : Singleton<LevelData> {

		public string lastLevelName = Globals.defaultLevelName;
		public int currentLevelId = -1;

		public void loadLevelFromJson(string json)
		{
			LevelEditor.Instance.curLevelChunk.reset ();

			LevelFile levelFile = null;
			//try {
				levelFile = createDataFromJson(json);
				if (levelFile != null) {
					createLevel (levelFile);
				}
			//}
			//catch (System.Exception e) {
			//	Debug.LogWarning (e.Message);
			//	AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningInvalidFileFormat.Replace("%1",""));
			//}
		}

		//
		public void loadLevelDataFromFile(string fileName)
		{
			LevelEditor.Instance.curLevelChunk.reset ();

			string json = File.ReadAllText(fileName);

			LevelFile levelFile = null;
			try {
				levelFile = createDataFromJson(json);
				if (levelFile != null) {
					createLevel (levelFile);
				}
			}
			catch (System.Exception e) {
				Debug.LogWarning (e.Message);
				AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningInvalidFileFormat.Replace("%1",""));
			}
		}

		//
		private LevelFile createDataFromJson(string json) {

			LevelFile levelFile = new LevelFile ();

			levelFile.parseJson (json);

			return levelFile;
		}

		//
		private void createLevel(LevelFile levelFile) {

			if (levelFile.fileFormatVersion != Globals.levelSaveFormatVersion) {
				AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningObsoleteFileFormat);
				return;
			}

			currentLevelId = levelFile.levelId;

			MainMenu.Instance.setLevelNameText(levelFile.levelName);
			lastLevelName = levelFile.levelName;

			LevelEditor levelEditor = LevelEditor.Instance;
			PropsManager propsManager = PropsManager.Instance;
			//World world = World.Instance;
			LevelChunk levelChunk = levelEditor.curLevelChunk;

			levelEditor.resetAll ();

			Vector3 savedPos = new Vector3 (levelFile.playerPosition.x, levelFile.playerPosition.y, levelFile.playerPosition.z);
			Vector3 savedRot = new Vector3 (levelFile.playerEuler.x, levelFile.playerEuler.y, levelFile.playerEuler.z);
			FlyCam.Instance.setNewInitialPosition (savedPos, savedRot);
			FlyCam.Instance.reset ();

			GameObject goQuadrant;
			Transform trfmContainer;
			GameObject container;
			Vector3 pos = Vector3.zero;

			int quadLen = levelEditor.cubesPerQuadrant;
			float fRockSize = levelEditor.fRockSize;

			int i, len = levelFile.levelQuadrants.Count;
			for (i = 0; i < len; ++i)
			{
				pos.x = (int)levelFile.levelQuadrants[i].position.x;
				pos.y = (int)levelFile.levelQuadrants[i].position.y;
				pos.z = (int)levelFile.levelQuadrants[i].position.z;
			
				string quadrantId = (int)pos.x + "_" + (int)pos.y + "_" + (int)pos.z;
				goQuadrant = levelChunk.createQuadrant (pos, quadrantId);
				if (goQuadrant == null) {
					continue;
				}

				trfmContainer = goQuadrant.transform.Find (Globals.cubesContainerName); // world.createContainer (goQuadrant.transform);
				if (trfmContainer == null) {
					continue;
				}
				container = trfmContainer.gameObject;

				Transform trfmCube;
				GameObject cube;
				//Vector3 pos2 = Vector3.zero;
				string materialName;
				Material material;

				bool isEdge = false;
				int j, len2 = levelFile.levelQuadrants [i].levelObjects.Count;
				for (j = 0; j < len2; ++j)
				{
					trfmCube = trfmContainer.Find (j.ToString());
					if (trfmCube == null) {
						continue;
					}
					cube = trfmCube.gameObject;

					if (levelFile.levelQuadrants [i].levelObjects [j].isActive == 0)
					{
						cube.SetActive (false);
						levelChunk.numCubes--;
					}
					else
					{
						/*if (levelFile.levelQuadrants [i].isEdge == 1) {
							material = levelEditor.materialEdge;
							isEdge = true;
						} else {*/
							materialName = Globals.materials[levelFile.levelQuadrants [i].levelObjects [j].materialId];
							material = levelEditor.aDictMaterials [materialName];
							isEdge = false;
						//}

						levelChunk.setCube (cube, material, isEdge);
					}
				}
			}

			Debug.Log ("Level id "+levelFile.levelId.ToString()+" - quadrants: "+len.ToString());
			Debug.Log ("Level id "+levelFile.levelId.ToString()+" - cubes: "+levelChunk.numCubes.ToString());
			MainMenu.Instance.setCubeCountText (levelChunk.numCubes);

			if (levelFile.levelProps != null) {
				
				LevelProp levelProp;
				GameObject goProp;
				Quaternion rotation = Quaternion.identity;
				propDef prop;
				string name;

				len = levelFile.levelProps.Count;
				for (i = 0; i < len; ++i) {
					levelProp = levelFile.levelProps [i];

					pos.x = levelProp.position.x;
					pos.y = levelProp.position.y;
					pos.z = levelProp.position.z;

					prop = propsManager.getPropDefForId(levelProp.id);
					if (prop.id != -1) {
					
						name = prop.name + "_" + levelChunk.trfmProps.childCount;
						goProp = propsManager.createProp (prop, pos, name, levelChunk.trfmProps, prop.useCollider, prop.useGravity);

						rotation.w = levelProp.rotation.w;
						rotation.x = levelProp.rotation.x;
						rotation.y = levelProp.rotation.y;
						rotation.z = levelProp.rotation.z;
						goProp.transform.rotation = rotation;

						levelChunk.addWorldProp (prop.id, goProp);
					}
				}
			}
		}

		//
		public void saveLevelData(string filename, string levelName) {

			lastLevelName = levelName;

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

			Transform parent = LevelEditor.Instance.curLevelChunk.trfmCubes;
			if (parent == null) {
				return null;
			}

			LevelFile levelFile = new LevelFile ();
			levelFile.fileFormatVersion = Globals.levelSaveFormatVersion;

			levelFile.levelId    = -1;
			levelFile.levelName  = levelName;

			levelFile.levelPos   = new DataTypeVector3 ();
			levelFile.levelPos.x = 0;
			levelFile.levelPos.y = 0;
			levelFile.levelPos.z = 0;

			levelFile.playerPosition   = new DataTypeVector3 ();
			levelFile.playerPosition.x = FlyCam.Instance.player.position.x;
			levelFile.playerPosition.y = FlyCam.Instance.player.position.y;
			levelFile.playerPosition.z = FlyCam.Instance.player.position.z;

			levelFile.playerEuler   = new DataTypeVector3 ();
			levelFile.playerEuler.x = FlyCam.Instance.player.eulerAngles.x;
			levelFile.playerEuler.y = FlyCam.Instance.player.eulerAngles.y;
			levelFile.playerEuler.z = FlyCam.Instance.player.eulerAngles.z;

			levelFile.levelQuadrants = new List<LevelQuadrant> ();

			foreach (Transform child in parent) {

				if (!child.gameObject.activeSelf) {
					continue;
				}

				LevelQuadrant quadrant = new LevelQuadrant ();

				quadrant.position   = new DataTypeVector3 ();
				quadrant.position.x = child.localPosition.x;
				quadrant.position.y = child.localPosition.y;
				quadrant.position.z = child.localPosition.z;

				int qX = (int)quadrant.position.x;
				int qY = (int)quadrant.position.y;
				int qZ = (int)quadrant.position.z;

				// sanity check!
				bool isEdgeQuadrant = ((qX <= -1 || qY <= -1 || qZ <= -1) || (qX >= Globals.LEVEL_WIDTH || qY >= Globals.LEVEL_HEIGHT || qZ >= Globals.LEVEL_DEPTH));
				if (isEdgeQuadrant) {
					continue;
				}

				//quadrant.isEdge = (isEdgeQuadrant ? 1 : 0);

				quadrant.levelObjects = new List<LevelObject> ();

				Transform container = child.Find ("container");
				if (container != null)
				{
					string materialName;

					foreach (Transform cube in container) {

						//if (!cube.gameObject.activeSelf) {
						//	continue;
						//}

						LevelObject cubeObject = new LevelObject ();

						cubeObject.isActive = (cube.gameObject.activeSelf ? 1 : 0);
						cubeObject.materialId = 0;

						//cubeObject.name = cube.name;
						//Debug.Log ("    ->cube "+cubeObject.name);

						/*cubeObject.position   = new DataTypeVector3 ();
						cubeObject.position.x = cube.localPosition.x;
						cubeObject.position.y = cube.localPosition.y;
						cubeObject.position.z = cube.localPosition.z;*/

						if (cubeObject.isActive == 1)
						{
							if (isEdgeQuadrant) {
								cubeObject.materialId = -1;
							} else {
								MeshRenderer renderer = cube.GetComponent<MeshRenderer> ();
								if (renderer != null) {
									materialName = renderer.material.name.Replace (" (Instance)", "");
									cubeObject.materialId = Array.IndexOf (Globals.materials, materialName);
								}
							}
						}

						quadrant.levelObjects.Add (cubeObject);
					}
				}

				levelFile.levelQuadrants.Add (quadrant);
			}

			// PROPS

			List<LevelProp> levelProps = new List<LevelProp> ();

			Dictionary<GameObject, worldProp> worldProps = LevelEditor.Instance.curLevelChunk.worldProps;
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
				//levelProp.name = prop.name;

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