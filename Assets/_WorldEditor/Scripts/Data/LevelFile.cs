//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzWorldEditor
{
	[Serializable]
	public class LevelFile {

		[SerializeField]
		public int fileFormatVersion { get; set; }

		[SerializeField]
		public string levelName { get; set; }

		[SerializeField]
		public List<LevelQuadrant> levelQuadrants { get; set; }

		[SerializeField]
		public List<LevelProp> levelProps { get; set; }

		[SerializeField]
		public DataTypeVector3 playerPosition  { get; set; }

		[SerializeField]
		public DataTypeVector3 playerEuler  { get; set; }

		//
		public string getJsonString()
		{
			int i, len;

			string s = "{";

			s += "\"v\":" + fileFormatVersion.ToString();
			s += ",\"n\":" + "\"" + levelName + "\"";

			s += ",\"quads\":[";
			len = levelQuadrants.Count;
			for (i = 0; i < len; ++i) {
				s += (i > 0 ? "," : "");
				s += levelQuadrants [i].getJsonString ();
			}
			s += "]";

			s += ",\"props\":[";
			len = levelProps.Count;
			for (i = 0; i < len; ++i) {
				s += (i > 0 ? "," : "");
				s += levelProps [i].getJsonString ();
			}
			s += "]";

			s += ",\"p\":" + playerPosition.getJsonString();
			s += ",\"r\":" + playerEuler.getJsonString();

			s += "}";

			return s;
		}
	}
}