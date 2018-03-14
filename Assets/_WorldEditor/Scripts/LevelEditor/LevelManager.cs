//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SimpleJSON;

namespace DragginzWorldEditor
{
	public struct LevelStruct {
		public int id;
		public string name; 
		public string filename; 
		public int x;
		public int y;
		public int z;
		public string jsonData;
		public LevelStruct(int id, string name, string filename, int x, int y, int z, string jd = null) {
			//Debug.Log ("creating level struct id "+id);
			this.id = id;
			this.name = name;
			this.filename = filename;
			this.x = x;
			this.y = y;
			this.z = z;
			this.jsonData = jd;
		}
	};

	//
	public class LevelManager : Singleton<LevelManager>
	{
		//private Dictionary<int, Dictionary<int, Dictionary<int, LevelStruct>>> _levelMapByPos;
		//private Dictionary<int, LevelStruct> _levelMapById;
		private LevelStruct[] _levelByIndex;

		private int _numLevels;

		#region Getters

		public int numLevels {
			get { return _numLevels; }
		}

		public LevelStruct[] levelByIndex {
			get { return _levelByIndex; }
		}

		#endregion

		#region PublicMethods

		public void init(string json)
		{
			//_levelMapByPos = new Dictionary<int, Dictionary<int, Dictionary<int, LevelStruct>>> ();
			//_levelMapById = new Dictionary<int, LevelStruct> ();

			_numLevels = 0;

			if (LevelEditor.Instance.levelListJson == null) {
				return;
			}

			JSONNode data = JSON.Parse (json);//LevelEditor.Instance.levelListJson.text);
			if (data == null || data ["levels"] == null) {
				return;
			}

			JSONArray levels = (JSONArray) data ["levels"];
			_numLevels = levels.Count;
			_levelByIndex = new LevelStruct[_numLevels];

			int i;
			for (i = 0; i < _numLevels; ++i) {
				JSONNode level = levels [i];
				LevelStruct ls = new LevelStruct (int.Parse(level["id"]), level["name"], level["filename"], int.Parse(level["x"]), int.Parse(level["y"]), int.Parse(level["z"]));
				_levelByIndex [i] = ls;
				saveLevelInfo (ls);
				MainMenu.Instance.addLevelToMenu (level ["filename"]);
			}
		}

		//
		public int getLevelIdByIndex(int index)
		{
			if (index < 0 || index >= _levelByIndex.Length) {
				return -1;
			}

			return _levelByIndex [index].id;
		}

		//
		public void loadLevelByIndex(int index)
		{
			if (index < 0 || index >= _levelByIndex.Length) {
				AppController.Instance.showPopup (PopupMode.Notification, "Error", Globals.errorLevelFileInvalidIndex);
			}

			LevelStruct ls = _levelByIndex [index];
			/*if (ls.jsonData == null || ls.jsonData == "") {
				TextAsset levelAsset = Resources.Load<TextAsset>("Data/Levels/"+ls.filename);
				if (levelAsset != null) {
					string json = levelAsset.text;
					ls.jsonData = json;
					_levelByIndex [index] = ls;
				}
			}*/

			if (ls.jsonData == null || ls.jsonData == "") {
				AppController.Instance.showPopup (PopupMode.Notification, "Error", Globals.warningInvalidFileFormat.Replace("%1",ls.filename));
			} else {
				LevelData.Instance.loadLevelFromJson (ls.jsonData);
			}
		}

		//
		public void setLevelJson(int id, string json) 
		{
			int i;
			for (i = 0; i < _numLevels; ++i) {
				if (_levelByIndex [i].id == id) {
					LevelStruct level = _levelByIndex [i];
					level.jsonData = json;
					_levelByIndex [i] = level;
					break;
				}
			}

			/*if (_levelMapById.ContainsKey (id)) {
				LevelStruct level = _levelMapById [id];
				level.jsonData = json;
				_levelMapById [id] = level;
			}*/
		}

		//
		public string getLevelJson(int id) 
		{
			string json = null;

			int i;
			for (i = 0; i < _numLevels; ++i) {
				if (_levelByIndex [i].id == id) {
					json = _levelByIndex [i].jsonData;
					break;
				}
			}

			/*if (_levelMapById.ContainsKey (id)) {
				return _levelMapById [id].jsonData;
			}*/

			return json;
		}

		//
		public string getLevelJson(int x, int y, int z) 
		{
			/*if (_levelMapByPos.ContainsKey (x)) {
				if (_levelMapByPos [x].ContainsKey (y)) {
					if (_levelMapByPos [x] [y].ContainsKey (z)) {
						return _levelMapByPos [x] [y] [z].jsonData;
					}
				}
			}*/

			return null;
		}

		//
		public LevelChunk createOfflineLevelChunk()
		{
			LevelChunk chunk;

			GameObject gameObject = AssetFactory.Instance.createLevelContainerClone ();
			gameObject.name = "LevelChunk_Offline";

			chunk = gameObject.AddComponent<LevelChunk> ();
			chunk.init ();

			return chunk;
		}

		//
		public Dictionary<int, LevelChunk> createLevelChunks()
		{
			Dictionary<int, LevelChunk> chunks = new Dictionary<int, LevelChunk> ();

			int i;
			for (i = 0; i < _numLevels; ++i) {

				LevelStruct level = _levelByIndex [i];

				GameObject gameObject = AssetFactory.Instance.createLevelContainerClone ();
				gameObject.name = "LevelChunk_" + level.id.ToString ();
				gameObject.transform.position = new Vector3 (level.x * Globals.LEVEL_WIDTH, -level.y * Globals.LEVEL_HEIGHT, level.z * Globals.LEVEL_DEPTH);

				LevelChunk chunk = gameObject.AddComponent<LevelChunk> ();
				chunk.init ();
				chunk.setLevelData (level);

				chunks.Add (level.id, chunk);
			}

			return chunks;
		}

		#endregion

		#region PrivateMethods

		private void saveLevelInfo(LevelStruct ls)
		{
			/*if (ls.id != -1) {
				if (!_levelMapById.ContainsKey (ls.id)) {
					_levelMapById.Add (ls.id, ls);
				}
			}

			if (!_levelMapByPos.ContainsKey (ls.x)) {
				_levelMapByPos.Add(ls.x, new Dictionary<int, Dictionary<int, LevelStruct>> ());
			}

			if (!_levelMapByPos[ls.x].ContainsKey (ls.y)) {
				_levelMapByPos[ls.x].Add(ls.y, new Dictionary<int, LevelStruct> ());
			}

			if (!_levelMapByPos[ls.x][ls.y].ContainsKey (ls.z)) {
				_levelMapByPos[ls.x][ls.y].Add(ls.z, ls);
			}*/
		}

		#endregion
	}
}