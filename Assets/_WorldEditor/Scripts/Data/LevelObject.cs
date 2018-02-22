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
	public class LevelObject {

		[SerializeField]
		public string name { get; set; }

		[SerializeField]
		public DataTypeVector3 position  { get; set; }

		[SerializeField]
		public string material { get; set; }

		//
		// Parse JSON data
		//
		public void parseJson(JSONNode data)
		{
			name = "";
			if (data ["n"] != null) {
				name = data ["n"];
			}

			position = new DataTypeVector3 ();
			if (data ["p"] != null) {
				position.x = data ["p"] ["x"];
				position.y = data ["p"] ["y"];
				position.z = data ["p"] ["z"];
			}

			material = "";
			if (data ["m"] != null) {
				material = data ["m"];
			}
		}

		//
		// Create JSON string
		//
		public string getJsonString()
		{
			string s = "{";

			s += "\"n\":" + "\"" + name + "\"";
			s += ",\"p\":" + position.getJsonString();
			s += ",\"m\":" + "\"" + material + "\"";

			s += "}";

			return s;
		}
	}
}