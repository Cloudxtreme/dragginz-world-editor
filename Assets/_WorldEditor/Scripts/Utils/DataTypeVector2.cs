//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using UnityEngine;

namespace DragginzWorldEditor
{
	[Serializable]
	public class DataTypeVector2{

		public float x { get; set; }
		public float y { get; set; }

		//
		public string getJsonString() {

			string s = "{";

			s += "\"x\":" + x.ToString();
			s += ",\"y\":" + y.ToString();

			s += "}";

			return s;
		}
	}
}