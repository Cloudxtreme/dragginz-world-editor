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
					createObjects (levelFile, parent);
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
		private void createObjects(LevelFile levelFile, GameObject parent) {
			
			if (levelFile.fileFormatVersion != Globals.levelSaveFormatVersion) {
				AppController.Instance.showPopup (PopupMode.Notification, "Warning", Globals.warningObsoleteFileFormat);
				return;
			}

			foreach (Transform child in parent.transform) {
				GameObject.Destroy (child.gameObject);
			}
 
			//Debug.Log ("format version: " + levelFile.fileFormatVersion);
			//Debug.Log ("levelFile: " + levelFile.levelName);
			for (int i = 0; i < levelFile.levelObjects.Count; ++i) {
				createNewGameObject (levelFile.levelObjects [i]);
			}
		}

		/// <summary>
		/// ...
		/// </summary>
		public void saveLevelData(GameObject parent, string filename) {

			LevelFile levelFile = createLevelInfo (parent);
			if (levelFile == null) {
				return;
			}

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(filename, FileMode.OpenOrCreate);

			bf.Serialize(file, levelFile);

			file.Close();
			file.Dispose();
		}

		/// <summary>
		/// ...
		/// </summary>
		private void createNewGameObject(LevelObject obj) {

			GameObject go = new GameObject (obj.name);
			go.transform.SetParent (LevelEditor.Instance.goWorld.transform);

			MeshFilter meshFilter = go.AddComponent<MeshFilter> ();
			Mesh mesh = new Mesh ();
			meshFilter.mesh = mesh;

			mesh.vertices   = Globals.dataTypeVector3ToVector3 (obj.vertices);
			mesh.triangles  = obj.triangles;
			mesh.normals    = Globals.dataTypeVector3ToVector3 (obj.normals);
			mesh.uv         = Globals.dataTypeVector2ToVector2 (obj.uv);

			MeshRenderer renderer = go.AddComponent<MeshRenderer> ();
			if (obj.material == "Default-Material") {
				renderer.material = new Material(Shader.Find("Standard"));
			} else {
				renderer.material = Resources.Load<Material> ("Materials/" + obj.material);
			}

			go.transform.localPosition = new Vector3 (obj.position.x, obj.position.y, obj.position.z);
			go.transform.localRotation = new Quaternion(obj.rotation.x, obj.rotation.y, obj.rotation.z, obj.rotation.w);
			go.transform.localScale    = new Vector3 (obj.scale.x, obj.scale.y, obj.scale.z);
		}

		/// <summary>
		/// ...
		/// </summary>
		private LevelFile createLevelInfo(GameObject parent) {

			if (!parent) {
				return null;
			}

			LevelFile levelFile         = new LevelFile ();
			levelFile.fileFormatVersion = Globals.levelSaveFormatVersion;
			levelFile.levelName         = "Kick Ass Level";
			levelFile.levelObjects      = new List<LevelObject> ();

			foreach (Transform child in parent.transform) {

				if (!child.gameObject.activeSelf) {
					continue;
				}

				LevelObject obj = new LevelObject ();
				obj.name = child.name;

				MeshFilter meshFilter = child.GetComponent<MeshFilter> ();
				if (meshFilter != null && meshFilter.mesh != null) {
					obj.vertices  = Globals.vector3ToDataTypeVector3(meshFilter.mesh.vertices);
					obj.triangles = meshFilter.mesh.triangles;
					obj.normals   = Globals.vector3ToDataTypeVector3(meshFilter.mesh.normals);
					obj.uv        = Globals.vector2ToDataTypeVector2(meshFilter.mesh.uv);
				}

				obj.position   = new DataTypeVector3 ();
				obj.position.x = child.localPosition.x;
				obj.position.y = child.localPosition.y;
				obj.position.z = child.localPosition.z;

				obj.rotation   = new DataTypeQuaternion ();
				obj.rotation.w = child.localRotation.w;
				obj.rotation.x = child.localRotation.x;
				obj.rotation.y = child.localRotation.y;
				obj.rotation.z = child.localRotation.z;

				obj.scale   = new DataTypeVector3();
				obj.scale.x = child.localScale.x;
				obj.scale.y = child.localScale.y;
				obj.scale.z = child.localScale.z;

				MeshRenderer renderer = child.GetComponent<MeshRenderer> ();
				if (renderer != null) {
					obj.material = renderer.material.name.Replace(" (Instance)","");
				}

				levelFile.levelObjects.Add (obj);
			}

			return levelFile;
		}
	}
}