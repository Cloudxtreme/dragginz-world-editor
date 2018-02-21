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