//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using UnityEngine;

namespace DragginzWorldEditor
{
	[Serializable]
	public class DataTypeQuaternion {

		public float w { get; set; }
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }

		//
		public string getJsonString() {

			string s = "{";

			s += "\"w\":" + w.ToString();
			s += ",\"x\":" + x.ToString();
			s += ",\"y\":" + y.ToString();
			s += ",\"z\":" + z.ToString();

			s += "}";

			return s;
		}
	}
}