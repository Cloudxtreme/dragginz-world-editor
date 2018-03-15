//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SimpleJSON;

namespace DragginzWorldEditor
{
	[Serializable]
	public class LevelQuadrant {

		//[SerializeField]
		//public string name { get; set; }

		[SerializeField]
		public DataTypeVector3 position  { get; set; }

		//[SerializeField]
		//public int isEdge { get; set; }

		[SerializeField]
		public List<LevelObject> levelObjects { get; set; }

		//
		// Parse JSON data
		//
		public void parseJson(JSONNode data)
		{
			int i, len;
			LevelObject levelObject;

			/*name = "";
			if (data ["n"] != null) {
				name = data ["n"];
			}*/

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

			/*isEdge = 0;
			if (data ["e"] != null) {
				isEdge = Int32.Parse (data ["e"]);
			}*/

			levelObjects = new List<LevelObject> ();
			if (data ["objs"] != null) {
				JSONArray objs = (JSONArray) data ["objs"];
				if (objs != null) {
					len = objs.Count;
					for (i = 0; i < len; ++i) {
						levelObject = new LevelObject ();
						levelObject.parseJson (objs [i]);
						levelObjects.Add (levelObject);
					}
				}
			}
		}

		//
		// Create JSON string
		//
		public string getJsonString()
		{
			int i, len;

			string s = "{";

			//s += "\"n\":" + "\"" + name + "\"";
			s += "\"p\":" + position.getJsonString();

			/*if (isEdge != 0) {
				s += ",\"e\":" + isEdge.ToString ();
			}*/

			len = levelObjects.Count;
			if (len > 0) {
				string o;
				bool gotOne = false;
				s += ",\"objs\":[";
				for (i = 0; i < len; ++i) {
					o = levelObjects [i].getJsonString ();
					//if (o != "{}") {
						s += (gotOne ? "," : "");
						s += o;
						gotOne = true;
					//}
				}
				s += "]";
			}

			s += "}";

			return s;
		}
	}
}