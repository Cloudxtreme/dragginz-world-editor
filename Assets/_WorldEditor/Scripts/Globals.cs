//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;

namespace DragginzWorldEditor
{
	public static class Globals
    {
		static public readonly string version = "Level Editor v01.31.2a";
		static public readonly int levelSaveFormatVersion = 2;

        static public readonly string appContainerName      = "{AppController}";
        static public readonly string lightsContainerName   = "[Lights]";
        static public readonly string worldContainerName    = "[World]";
        static public readonly string mainMenuContainerName = "[MainMenu]";

		static public readonly int EDITOR_TOOL_LOOK  = 0;
		static public readonly int EDITOR_TOOL_DIG   = 1;
		static public readonly int EDITOR_TOOL_PAINT = 2;
		static public readonly int EDITOR_TOOL_BUILD = 3;
		static public readonly int NUM_EDITOR_TOOLS  = 4;

		public const string defaultShaderName   = "Mobile/Diffuse"; // "Standard"
		public const string highlightShaderName = "Legacy Shaders/Reflective/Diffuse";

        static public readonly string[] materials = {"Marble", "Moss", "Shape", "Stone"};

		static public readonly string warningObsoleteFileFormat = "Can't load level:\nFile format is obsolete!";
		static public readonly string warningInvalidFileFormat  = "Can't load level:\nFile format is invalid!";

		static public readonly string containerGameObjectPrepend = "quadrant_";
		static public readonly string rockGameObjectPrepend = "rock_";

		static public readonly float RAYCAST_DISTANCE_EDIT = 20.0f;


		/// <summary>
		/// ...
		/// </summary>
		public static Vector3[] getPointsOnSphere(int nPoints)
		{
			float fPoints = (float)nPoints;

			Vector3[] points = new Vector3[nPoints];

			float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
			float off = 2 / fPoints;

			for (int k = 0; k < nPoints; k++)
			{
				float y = k * off - 1 + (off / 2);
				float r = Mathf.Sqrt(1 - y * y);
				float phi = k * inc;

				points[k] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
			}

			return points;
		}

		/// <summary>
		/// ...
		/// </summary>
		public static DataTypeVector2[] vector2ToDataTypeVector2(Vector2[] v2Array) {

			int len = v2Array.Length;
			DataTypeVector2[] dtv2Array = new DataTypeVector2[len];
			for (int i = 0; i < len; ++i) {
				DataTypeVector2 dtv2 = new DataTypeVector2 ();
				dtv2.x = v2Array [i].x;
				dtv2.y = v2Array [i].y;
				dtv2Array [i] = dtv2;
			}

			return dtv2Array;
		}

		public static DataTypeVector3[] vector3ToDataTypeVector3(Vector3[] v3Array) {

			//Debug.Log ("vector3ToDataTypeVector3");
			int len = v3Array.Length;
			DataTypeVector3[] dtv3Array = new DataTypeVector3[len];
			for (int i = 0; i < len; ++i) {
				//Debug.Log ("   ->"+i);
				DataTypeVector3 dtv3 = new DataTypeVector3 ();
				dtv3.x = v3Array [i].x;
				dtv3.y = v3Array [i].y;
				dtv3.z = v3Array [i].z;
				dtv3Array [i] = dtv3;
				//Debug.Log ("   ->"+v3Array [i].ToString() + " to " + +dtv3Array[i].x+", "+dtv3Array[i].y+", "+dtv3Array[i].z);
			}

			return dtv3Array;
		}

		/// <summary>
		/// ...
		/// </summary>
		public static Vector2[] dataTypeVector2ToVector2(DataTypeVector2[] dtv2Array) {

			int len = dtv2Array.Length;
			Vector2[] v2Array = new Vector2[len];
			for (int i = 0; i < len; ++i) {
				Vector2 v2 = new Vector2 ();
				v2.x = dtv2Array [i].x;
				v2.y = dtv2Array [i].y;
				v2Array [i] = v2;
			}

			return v2Array;
		}

		public static Vector3[] dataTypeVector3ToVector3(DataTypeVector3[] dtv3Array) {

			//Debug.Log ("dataTypeVector3ToVector3");
			int len = dtv3Array.Length;
			Vector3[] v3Array = new Vector3[len];
			for (int i = 0; i < len; ++i) {
				//Debug.Log ("   ->"+i);
				Vector3 v3 = new Vector3 ();
				v3.x = dtv3Array [i].x;
				v3.y = dtv3Array [i].y;
				v3.z = dtv3Array [i].z;
				v3Array [i] = v3;
				//Debug.Log ("      ->" + dtv3Array[i].x+", "+dtv3Array[i].y+", "+dtv3Array[i].z + " to " + v3Array [i].ToString());
			}

			return v3Array;
		}

		public static void logDataTypeVector3Array(DataTypeVector3[] dtv3Array) {
			int len = dtv3Array.Length;
			for (int i = 0; i < len; ++i) {
				Debug.Log (i);
				Debug.Log ("   ->"+dtv3Array[i].x+", "+dtv3Array[i].y+", "+dtv3Array[i].z);
			}
		}
	}
}