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
	public class LevelProp {

		[SerializeField]
		public int id { get; set; }

		[SerializeField]
		public string name { get; set; }

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

			name = "";
			if (data ["n"] != null) {
				name = data ["n"];
			}

			position = new DataTypeVector3 ();
			if (data ["p"] != null) {
				position.x = data ["p"]["x"];
				position.y = data ["p"]["y"];
				position.z = data ["p"]["z"];
			}

			rotation = new DataTypeQuaternion ();
			if (data ["r"] != null) {
				rotation.w = data ["r"]["w"];
				rotation.x = data ["r"]["x"];
				rotation.y = data ["r"]["y"];
				rotation.z = data ["r"]["z"];
			}
		}

		//
		// Create JSON string
		//
		public string getJsonString()
		{
			string s = "{";

			s += "\"id\":" + id.ToString();
			s += ",\"n\":" + "\"" + name + "\"";
			s += ",\"p\":" + position.getJsonString();
			s += ",\"r\":" + rotation.getJsonString();

			s += "}";

			return s;
		}
	}
}