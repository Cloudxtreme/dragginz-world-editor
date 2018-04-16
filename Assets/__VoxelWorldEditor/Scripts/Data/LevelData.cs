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

using AssetsShared;

namespace DragginzVoxelWorldEditor
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
			LevelEditor.Instance.curVoxelsLevelChunk.reset ();

			string json = File.ReadAllText(fileName);

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
			VoxelsLevelChunk levelChunk = levelEditor.curVoxelsLevelChunk;

			levelEditor.resetAll ();

			Vector3 savedPos = new Vector3 (levelFile.playerPosition.x, levelFile.playerPosition.y, levelFile.playerPosition.z);
			Vector3 savedRot = new Vector3 (levelFile.playerEuler.x, levelFile.playerEuler.y, levelFile.playerEuler.z);
			levelChunk.setStartPos (savedPos, savedRot);
			FlyCam.Instance.setNewInitialPosition (savedPos, savedRot);
			FlyCam.Instance.reset ();

			//GameObject goQuadrant;
			//Transform trfmContainer;
			//GameObject container;
			Vector3 pos = Vector3.zero;

			int quadLen = levelEditor.cubesPerQuadrant;
			float fRockSize = levelEditor.fRockSize;

			int i, len = levelFile.levelVoxelChunks.Count;
			Debug.Log ("levelFile.levelVoxelChunks.Count: " + levelFile.levelVoxelChunks.Count);
			for (i = 0; i < len; ++i)
			{
				pos.x = (int)levelFile.levelVoxelChunks[i].position.x;
				pos.y = (int)levelFile.levelVoxelChunks[i].position.y;
				pos.z = (int)levelFile.levelVoxelChunks[i].position.z;

				VoxelUtils.VoxelChunk vc = levelEditor.curVoxelsLevelChunk.createVoxelChunk (
					VoxelUtils.convertVector3ToVoxelVector3Int(pos),
					(int)levelFile.levelVoxelChunks[i].size.x,
					(int)levelFile.levelVoxelChunks[i].size.y,
					(int)levelFile.levelVoxelChunks[i].size.z
				);

				vc.materialIndex = levelFile.levelVoxelChunks [i].materialId;

				levelEditor.curVoxelsLevelChunk.aVoxelChunks.Add(vc);

				levelEditor.curVoxelsLevelChunk.setVoxelChunkMesh(vc);
			}

			MainMenu.Instance.setCubeCountText ("Voxel Chunks: "+levelEditor.curVoxelsLevelChunk.aVoxelChunks.Count.ToString());

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

			File.WriteAllText (filename, json);
		}

		//
		private LevelFile createLevelData(string levelName)
		{
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

			levelFile.levelVoxelChunks = new List<LevelVoxelChunk> ();

			VoxelsLevelChunk vlc = LevelEditor.Instance.curVoxelsLevelChunk;
			VoxelUtils.VoxelChunk vc;

			int i, len = vlc.aVoxelChunks.Count;
			for (i = 0; i < len; ++i) {

				vc = vlc.aVoxelChunks [i];
				LevelVoxelChunk voxelChunk = new LevelVoxelChunk ();

				voxelChunk.position   = new DataTypeVector3 ();
				voxelChunk.position.x = vc.pos.x;
				voxelChunk.position.y = vc.pos.y;
				voxelChunk.position.z = vc.pos.z;

				voxelChunk.size   = new DataTypeVector3 ();
				voxelChunk.size.x = vc.size.x;
				voxelChunk.size.y = vc.size.y;
				voxelChunk.size.z = vc.size.z;

				voxelChunk.materialId = vc.materialIndex;

				levelFile.levelVoxelChunks.Add (voxelChunk);
			}

			// PROPS

			List<LevelProp> levelProps = new List<LevelProp> ();

			Dictionary<GameObject, worldProp> worldProps = LevelEditor.Instance.curVoxelsLevelChunk.worldProps;
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

			return levelFile;
		}
	}
}