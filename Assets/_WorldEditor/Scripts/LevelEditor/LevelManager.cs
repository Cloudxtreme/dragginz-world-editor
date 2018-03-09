//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzWorldEditor
{
	public struct levelStruct {
		public int id;
		public string name; 
		public int x;
		public int y;
		public int z;
		public string jsonFile;
		public string jsonData;
		public levelStruct(int id, string name, int x, int y, int z, string jf, string jd) {
			this.id = id;
			this.name = name;
			this.x = x;
			this.y = y;
			this.z = z;
			this.jsonFile = jf;
			this.jsonData = jd;
		}
	};

	//
	public class LevelManager : Singleton<PropsManager>
	{
		private Dictionary<int, Dictionary<int, Dictionary<int, levelStruct>>> _levelMapByPos;
		private Dictionary<int, levelStruct> _levelMapById;

		#region Getters

		#endregion

		#region PublicMethods

		public void init()
		{
			_levelMapByPos = new Dictionary<int, Dictionary<int, Dictionary<int, levelStruct>>> ();
			_levelMapById = new Dictionary<int, levelStruct> ();

			for (int lev = 0; lev < Globals.levelIndex.Length; ++lev) {
				string[] levInfo = Globals.levelIndex [lev].Split (new char[]{';'});
				if (levInfo.Length != 4) {
					Debug.LogWarning ("wrong level index data");
				} else {
					levelStruct ls = new levelStruct (-1, levInfo[0], int.Parse(levInfo[1]), int.Parse(levInfo[2]), int.Parse(levInfo[3]), "", "");
					saveLevelInfo (ls);
				}
			}
		}

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

		private void saveLevelInfo(levelStruct ls)
		{
			if (ls.id != -1) {
				if (!_levelMapById.ContainsKey (ls.id)) {
					_levelMapById.Add (ls.id, ls);
				}
			}

			if (!_levelMapByPos.ContainsKey (ls.x)) {
				_levelMapByPos.Add(ls.x, new Dictionary<int, Dictionary<int, levelStruct>> ());
			}

			if (!_levelMapByPos[ls.x].ContainsKey (ls.y)) {
				_levelMapByPos[ls.x].Add(ls.y, new Dictionary<int, levelStruct> ());
			}

			if (!_levelMapByPos[ls.x][ls.y].ContainsKey (ls.z)) {
				_levelMapByPos[ls.x][ls.y].Add(ls.z, ls);
			}
		}

		#endregion
	}
}