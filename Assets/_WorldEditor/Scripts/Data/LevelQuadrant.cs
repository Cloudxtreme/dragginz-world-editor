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
	public class LevelQuadrant {

		[SerializeField]
		public string name { get; set; }

		[SerializeField]
		public DataTypeVector3 position  { get; set; }

		[SerializeField]
		public int isEdge { get; set; }

		[SerializeField]
		public List<LevelObject> levelObjects { get; set; }

		//
		public string getJsonString()
		{
			int i, len;

			string s = "{";

			s += "\"n\":" + "\"" + name + "\"";
			s += ",\"p\":" + position.getJsonString();
			s += ",\"e\":" + isEdge.ToString();

			s += ",\"objs\":[";
			len = levelObjects.Count;
			for (i = 0; i < len; ++i) {
				s += (i > 0 ? "," : "");
				s += levelObjects [i].getJsonString ();
			}
			s += "]";

			s += "}";

			return s;
		}
	}
}