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

		//[SerializeField]
		//public string name { get; set; }

		[SerializeField]
		public DataTypeVector3 position  { get; set; }

		//[SerializeField]
		//public string material { get; set; }

		[SerializeField]
		public int materialId { get; set; }

		//
		// Parse JSON data
		//
		public void parseJson(JSONNode data)
		{
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

			materialId = 0;
			if (data ["m"] != null) {
				materialId = Int32.Parse (data ["m"]);
			}

			/*material = "";
			if (data ["m"] != null) {
				material = data ["m"];
			}*/
		}

		//
		// Create JSON string
		//
		public string getJsonString()
		{
			string s = "{";

			//s += "\"n\":" + "\"" + name + "\"";

			string p = position.getJsonString();
			if (p != "{}") {
				s += "\"p\":" +p;
			}

			if (materialId != 0) {
				if (s.Length > 1) {
					s += ",";
				}
				s += "\"m\":" + materialId.ToString ();
			}

			//s += ",\"m\":" + "\"" + material + "\"";

			s += "}";

			return s;
		}
	}
}