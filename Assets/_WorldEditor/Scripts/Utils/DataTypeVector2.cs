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

			if (x != 0f) {
				s += "\"x\":" + x.ToString ();
			}

			if (y != 0f) {
				if (x != 0f) {
					s += ",";
				}
				s += "\"y\":" + y.ToString ();
			}

			s += "}";

			return s;
		}
	}
}