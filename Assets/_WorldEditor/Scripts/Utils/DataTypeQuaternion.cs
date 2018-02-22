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

			if (w != 0f) {
				s += "\"w\":" + w.ToString ();
			}

			if (x != 0f) {
				if (w != 0f) {
					s += ",";
				}
				s += "\"x\":" + x.ToString ();
			}

			if (y != 0f) {
				if (w != 0f || x != 0f) {
					s += ",";
				}
				s += "\"y\":" + y.ToString ();
			}

			if (z != 0f) {
				if (w != 0f || x != 0f || y != 0f) {
					s += ",";
				}
				s += "\"z\":" + z.ToString ();
			}

			s += "}";

			return s;
		}
	}
}