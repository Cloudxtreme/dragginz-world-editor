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
	/// <summary>
	/// ...
	/// </summary>
	public class LevelData : Singleton<LevelData> {

		/// <summary>
		/// ...
		/// </summary>
		public void loadLevelData(GameObject parent, string filename) {
			
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(filename, FileMode.Open);

			LevelFile levelFile = null;
			try {
				levelFile = bf.Deserialize(file) as LevelFile;
				if (levelFile != null) {
					createLevel (levelFile, parent);
				}
			}
			catch (System.Exception e) {
				Debug.LogWarning (e.Message);
				AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningInvalidFileFormat);
			}

			file.Close();
			file.Dispose();
		}

		/// <summary>
		/// ...
		/// </summary>
		private void createLevel(LevelFile levelFile, GameObject parent) {
			
			if (levelFile.fileFormatVersion != Globals.levelSaveFormatVersion) {
				AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningObsoleteFileFormat);
				return;
			}

			LevelEditor levelEditor = LevelEditor.Instance;
			World world = World.Instance;

			levelEditor.resetAll ();

			GameObject goQuadrant;
			GameObject container;
			Vector3 pos = Vector3.zero;
			int i, len = levelFile.levelQuadrants.Count;
			for (i = 0; i < len; ++i)
			{
				pos.x = (int)levelFile.levelQuadrants[i].position.x;
				pos.y = (int)levelFile.levelQuadrants[i].position.y;
				pos.z = (int)levelFile.levelQuadrants[i].position.z;
			
				goQuadrant = world.createQuadrant (pos);

				container = world.createContainer (goQuadrant.transform);

				Vector3 pos2 = Vector3.zero;
				Material material;
				int j, len2 = levelFile.levelQuadrants [i].levelObjects.Count;
				for (j = 0; j < len2; ++j)
				{
					pos2.x = levelFile.levelQuadrants [i].levelObjects [j].position.x;
					pos2.y = levelFile.levelQuadrants [i].levelObjects [j].position.y;
					pos2.z = levelFile.levelQuadrants [i].levelObjects [j].position.z;
					material = levelEditor.aDictMaterials [levelFile.levelQuadrants [i].levelObjects [j].material];
					world.createRock (pos2, container, levelFile.levelQuadrants [i].levelObjects [j].name, material);
				}
			}

			MainMenu.Instance.setCubeCountText (World.Instance.numCubes);
		}

		/// <summary>
		/// ...
		/// </summary>
		public void saveLevelData(string filename) {

			LevelFile levelFile = createLevelInfo (LevelEditor.Instance.goWorld);
			if (levelFile == null) {
				return;
			}

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(filename, FileMode.OpenOrCreate);

			bf.Serialize(file, levelFile);

			file.Close();
			file.Dispose();
		}

		//
		/*private void createNewGameObject(LevelQuadrant obj) {

			GameObject go = new GameObject (obj.name);
			go.transform.SetParent (LevelEditor.Instance.goWorld.transform);

			MeshFilter meshFilter = go.AddComponent<MeshFilter> ();
			Mesh mesh = new Mesh ();
			meshFilter.mesh = mesh;

			MeshRenderer renderer = go.AddComponent<MeshRenderer> ();
			if (obj.material == "Default-Material") {
				renderer.material = new Material(Shader.Find("Standard"));
			} else {
				renderer.material = Resources.Load<Material> ("Materials/" + obj.material);
			}

			go.transform.localPosition = new Vector3 (obj.position.x, obj.position.y, obj.position.z);
			//go.transform.localRotation = new Quaternion(obj.rotation.x, obj.rotation.y, obj.rotation.z, obj.rotation.w);
			//go.transform.localScale    = new Vector3 (obj.scale.x, obj.scale.y, obj.scale.z);
		}*/

		//
		private LevelFile createLevelInfo(GameObject parent) {

			if (!parent) {
				return null;
			}

			LevelFile levelFile         = new LevelFile ();
			levelFile.fileFormatVersion = Globals.levelSaveFormatVersion;
			levelFile.levelName         = "My Level";
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

						MeshRenderer renderer = cube.GetComponent<MeshRenderer> ();
						if (renderer != null) {
							cubeObject.material = renderer.material.name.Replace (" (Instance)", "");
						}

						quadrant.levelObjects.Add (cubeObject);
					}
				}

				levelFile.levelQuadrants.Add (quadrant);
			}

			return levelFile;
		}
	}
}