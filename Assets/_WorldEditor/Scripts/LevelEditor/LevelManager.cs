//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SimpleJSON;

namespace DragginzWorldEditor
{
	public struct LevelStruct {
		public int id;
		public string filename; 
		public int x;
		public int y;
		public int z;
		public string jsonData;
		public LevelStruct(int id, string filename, int x, int y, int z, string jd = null) {
			this.id = id;
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
		private Dictionary<int, Dictionary<int, Dictionary<int, LevelStruct>>> _levelMapByPos;
		private Dictionary<int, LevelStruct> _levelMapById;
		private LevelStruct[] _levelByIndex;

		#region Getters

		#endregion

		#region PublicMethods

		public void init()
		{
			_levelMapByPos = new Dictionary<int, Dictionary<int, Dictionary<int, LevelStruct>>> ();
			_levelMapById = new Dictionary<int, LevelStruct> ();

			if (LevelEditor.Instance.levelListJson == null) {
				return;
			}

			JSONNode data = JSON.Parse(LevelEditor.Instance.levelListJson.text);
			if (data == null || data ["levels"] == null) {
				return;
			}

			JSONArray levels = (JSONArray) data ["levels"];
			int i, len = levels.Count;
			_levelByIndex = new LevelStruct[len];
			for (i = 0; i < len; ++i) {
				JSONNode level = levels [i];
				LevelStruct ls = new LevelStruct (int.Parse(level["id"]), level["filename"], int.Parse(level["x"]), int.Parse(level["y"]), int.Parse(level["z"]));
				_levelByIndex [i] = ls;
				saveLevelInfo (ls);
				MainMenu.Instance.addLevelToMenu (level ["filename"]);
			}
		}

		//
		public void loadLevel(int index)
		{
			if (index < 0 || index >= _levelByIndex.Length) {
				AppController.Instance.showPopup (PopupMode.Notification, "Error", Globals.errorLevelFileInvalidIndex);
			}
			// get level data from LevelManager
			// 
		}

		//
		public string getLevelJson(int id) 
		{
			if (_levelMapById.ContainsKey (id)) {
				return _levelMapById [id].jsonData;
			}

			return null;
		}

		public string getLevelJson(int x, int y, int z) 
		{
			if (_levelMapByPos.ContainsKey (x)) {
				if (_levelMapByPos [x].ContainsKey (y)) {
					if (_levelMapByPos [x] [y].ContainsKey (z)) {
						return _levelMapByPos [x] [y] [z].jsonData;
					}
				}
			}

			return null;
		}

		#endregion

		#region PrivateMethods

		private void saveLevelInfo(LevelStruct ls)
		{
			if (ls.id != -1) {
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
			}
		}

		#endregion
	}
}