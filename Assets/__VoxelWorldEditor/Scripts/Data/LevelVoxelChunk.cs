//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AssetsShared;

using SimpleJSON;

namespace DragginzVoxelWorldEditor
{
	[Serializable]
	public class LevelVoxelChunk {

		[SerializeField]
		public DataTypeVector3 position  { get; set; }

		[SerializeField]
		public DataTypeVector3 size  { get; set; }

		[SerializeField]
		public int materialId { get; set; }

		//
		// Parse JSON data
		//
		public void parseJson(JSONNode data)
		{
			position = new DataTypeVector3 ();
			position.x = 0;
			position.y = 0;
			position.z = 0;
			if (data ["p"] != null) {
				if (data ["p"] ["x"] != null) {
					position.x = (float)data ["p"] ["x"];
				}
				if (data ["p"] ["y"] != null) {
					position.y = (float)data ["p"] ["y"];
				}
				if (data ["p"] ["z"] != null) {
					position.z = (float)data ["p"] ["z"];
				}
			}

			size = new DataTypeVector3 ();
			size.x = 0;
			size.y = 0;
			size.z = 0;
			if (data ["s"] != null) {
				if (data ["s"] ["x"] != null) {
					size.x = (float)data ["s"] ["x"];
				}
				if (data ["s"] ["y"] != null) {
					size.y = (float)data ["s"] ["y"];
				}
				if (data ["s"] ["z"] != null) {
					size.z = (float)data ["s"] ["z"];
				}
			}

			materialId = 0;
			if (data ["m"] != null) {
				materialId = Int32.Parse (data ["m"]);
			}
		}

		//
		// Create JSON string
		//
		public string getJsonString()
		{
			string s = "{";

			string p = position.getJsonString();
			if (p != "{}") {
				s += "\"p\":" +p;
			}

			string sz = size.getJsonString();
			if (sz != "{}") {
				if (s.Length > 1) {
					s += ",";
				}
				s += "\"s\":" +sz;
			}

			if (materialId != 0) {
				if (s.Length > 1) {
					s += ",";
				}
				s += "\"m\":" + materialId.ToString ();
			}

			s += "}";

			return s;
		}
	}
}