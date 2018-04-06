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
	public class LevelProp {

		[SerializeField]
		public int id { get; set; }

		//[SerializeField]
		//public string name { get; set; }

		[SerializeField]
		public DataTypeVector3 position  { get; set; }

		[SerializeField]
		public DataTypeQuaternion rotation  { get; set; }

		//
		// Parse JSON data
		//
		public void parseJson(JSONNode data)
		{
			id = 0;
			if (data ["id"] != null) {
				id = Int32.Parse (data ["id"]);
			}

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

			rotation = new DataTypeQuaternion ();
			rotation.w = 0;
			rotation.x = 0;
			rotation.y = 0;
			rotation.z = 0;
			if (data ["r"] != null) {
				if (data ["r"] ["w"] != null) {
					rotation.w = (float)data ["r"] ["w"];
				}
				if (data ["r"] ["x"] != null) {
					rotation.x = (float)data ["r"] ["x"];
				}
				if (data ["r"] ["y"] != null) {
					rotation.y = (float)data ["r"] ["y"];
				}
				if (data ["r"] ["z"] != null) {
					rotation.z = (float)data ["r"] ["z"];
				}
			}
		}

		//
		// Create JSON string
		//
		public string getJsonString()
		{
			string s = "{";

			s += "\"id\":" + id.ToString();
			//s += ",\"n\":" + "\"" + name + "\"";
			s += ",\"p\":" + position.getJsonString();
			s += ",\"r\":" + rotation.getJsonString();

			s += "}";

			return s;
		}
	}
}