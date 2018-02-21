//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using UnityEngine;

namespace DragginzWorldEditor
{
	[Serializable]
	public class DataTypeVector3 {

		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }

		//
		public string getJsonString() {

			string s = "{";

			s += "\"x\":" + x.ToString();
			s += ",\"y\":" + y.ToString();
			s += ",\"z\":" + z.ToString();

			s += "}";

			return s;
		}
	}
}