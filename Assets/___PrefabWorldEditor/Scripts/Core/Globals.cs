//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections.Generic;

using UnityEngine;

using AssetsShared;

namespace PrefabWorldEditor
{
	public static class Globals
    {
		static public readonly string version = "Prefab World Editor v06.20.0a";

		static public readonly int levelSaveFormatVersion = 100;

		//

		public static readonly int TargetClientFramerate = 120;

		public const string propListName = "props_list_leveleditor";

        static public readonly string appContainerName   = "{AppController}";
		static public readonly string netContainerName   = "{NetManager}";
        static public readonly string worldContainerName = "[World]";

		//
		public enum PopupMode {
			Notification,
			Confirmation,
			Input,
			Overlay
		};

		public enum TOOL {
			SELECT,
			DIG,
			PAINT,
			BUILD,
			PROPS,
			RAILGUN,
			NUM_TOOLS
		};

		public const string defaultShaderName   = "Mobile/Diffuse"; // "Standard"
		public const string highlightShaderName = "Legacy Shaders/Reflective/Diffuse";

		static public readonly string[] materials = {"vwe_volcano", "vwe_moss", "vwe_shape", "vwe_stone", "vwe_marble", "vwe_cube-0", "vwe_cube-1", "vwe_cube-2", "vwe_cube-3", "vwe_cube-4", "vwe_cube-5", "vwe_cube-6", "vwe_cube-7", "vwe_cube-8", "vwe_cube-9"};
		static public readonly string[] materialsTools = {"vwe_tool_volcano", "vwe_tool_moss", "vwe_tool_shape", "vwe_tool_stone", "vwe_tool_marble", "vwe_tool_cube-0", "vwe_tool_cube-1", "vwe_tool_cube-2", "vwe_tool_cube-3", "vwe_tool_cube-4", "vwe_tool_cube-5", "vwe_tool_cube-6", "vwe_tool_cube-7", "vwe_tool_cube-8", "vwe_tool_cube-9"};

		static public readonly string warningObsoleteFileFormat  = "Can't load level:\n\nFile format is obsolete!";
		static public readonly string warningInvalidFileFormat   = "Can't load level '%1'\n\nFile format is invalid!";
		static public readonly string errorLevelFileInvalidIndex = "Invalid Level Index!";
		static public readonly string errorLevelFileInvalidFilename = "Invalid Level File Name:\n'%1'";

		static public readonly int LEVEL_WIDTH  = 36;
		static public readonly int LEVEL_HEIGHT = 36;
		static public readonly int LEVEL_DEPTH  = 36;

		static public readonly string containerGameObjectPrepend = "q_";
		static public readonly string cubesContainerName = "container";

		static public readonly string defaultLevelName = "myLevel";

		static public readonly float RAYCAST_DISTANCE_EDIT = 10.24f;

		static public readonly string urlLevelList = "";

		/*public struct Vector3Int
		{
			public int x;
			public int y;
			public int z;

			public Vector3Int(int x, int y, int z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
		};*/

		// Experimental
		static public readonly string[] materialsRailgun = {"vwe_railgun_cross", "vwe_railgun_steps", "vwe_railgun_corner_right", "vwe_railgun_corner_left"};
		public struct RailgunShape
		{
			public int width;
			public int height;
			public int depth;

			public List<Vector3> pos;
			public List<Vector3> size;

			public RailgunShape(int w, int h, int d, List<Vector3> p, List<Vector3> s)
			{
				this.width  = w;
				this.height = h;
				this.depth  = d;

				this.pos  = p;
				this.size = s;
			}
		};

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