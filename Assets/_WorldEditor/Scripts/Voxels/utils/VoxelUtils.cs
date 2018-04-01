//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections.Generic;

using UnityEngine;

namespace VoxelChunks
{
	public static class VoxelUtils
    {
		static public readonly uint LEFT_TOP_FRONT  = 0;
		static public readonly uint LEFT_TOP_BACK   = 1;
		static public readonly uint RIGHT_TOP_FRONT = 2;
		static public readonly uint RIGHT_TOP_BACK  = 3;
		static public readonly uint LEFT_BOT_FRONT  = 4;
		static public readonly uint LEFT_BOT_BACK   = 5;
		static public readonly uint RIGHT_BOT_FRONT = 6;
		static public readonly uint RIGHT_BOT_BACK  = 7;

		static public readonly int   MAX_CHUNK_UNITS = 72;
		static public readonly float CHUNK_SIZE = 0.5f;

		// ---------------------------------------------------------------------------------------------
		public struct VoxelVector3Int
		{
			public int x;
			public int y;
			public int z;

			public VoxelVector3Int(int x = 0, int y = 0, int z = 0)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public string ToString()
			{
				return this.x + ", " + this.y + ", " + this.z;
			}
		};

		// ---------------------------------------------------------------------------------------------
		public struct VoxelCorners
		{
			public VoxelVector3Int bot_left_front;
			public VoxelVector3Int bot_left_back;
			public VoxelVector3Int bot_right_back;
			public VoxelVector3Int bot_right_front;
			public VoxelVector3Int top_left_front;
			public VoxelVector3Int top_left_back;
			public VoxelVector3Int top_right_back;
			public VoxelVector3Int top_right_front;

			public string ToString()
			{
				var s = "";
				s += "\nbot_left_front:  " + bot_left_front.x  + ", " + bot_left_front.y  + ", " + bot_left_front.z;
				s += "\nbot_left_back:   " + bot_left_back.x   + ", " + bot_left_back.y   + ", " + bot_left_back.z;
				s += "\nbot_right_back:  " + bot_right_back.x  + ", " + bot_right_back.y  + ", " + bot_right_back.z;
				s += "\nbot_right_front: " + bot_right_front.x + ", " + bot_right_front.y + ", " + bot_right_front.z;
				s += "\ntop_left_front:  " + top_left_front.x  + ", " + top_left_front.y  + ", " + top_left_front.z;
				s += "\ntop_left_back:   " + top_left_back.x   + ", " + top_left_back.y   + ", " + top_left_back.z;
				s += "\ntop_right_back:  " + top_right_back.x  + ", " + top_right_back.y  + ", " + top_right_back.z;
				s += "\ntop_right_front: " + top_right_front.x + ", " + top_right_front.y + ", " + top_right_front.z;
				return s;
			}
		};

		// ---------------------------------------------------------------------------------------------
		public struct VoxelChunk
		{
			public GameObject      go;
			public Vector3         goPos;
			public VoxelVector3Int pos;
			public VoxelVector3Int size;
			public Bounds          bounds;
			public VoxelCorners    corners;

			public bool Identical(VoxelChunk vsOther)
			{
				bool identical = false;
				if (pos.x == vsOther.pos.x && pos.y == vsOther.pos.y && pos.z == vsOther.pos.z) {
					if (size.x == vsOther.size.x && size.y == vsOther.size.y && size.z == vsOther.size.z) {
						identical = true;
					}
				}
				return identical;
			}

			public bool Encased(VoxelChunk vsOther)
			{
				bool encased = false;
				if (vsOther.pos.x <= pos.x && (vsOther.pos.x + vsOther.size.x) >= (pos.x + size.x)) {
					if (vsOther.pos.y <= pos.y && (vsOther.pos.y + vsOther.size.y) >= (pos.y + size.y)) {
						if (vsOther.pos.z <= pos.z && (vsOther.pos.z + vsOther.size.z) >= (pos.z + size.z)) {
							encased = true;
						}
					}
				}
				return encased;
			}

			//
			public bool IntersectsLeftX(VoxelVector3Int point)
			{
				bool intersecting = false;
				if (point.x >= corners.bot_left_front.x && point.x < corners.bot_right_front.x) {
					intersecting = true;
				}
				return intersecting;
			}

			public bool IntersectsRightX(VoxelVector3Int point)
			{
				bool intersecting = false;
				if (point.x > corners.bot_left_front.x && point.x <= corners.bot_right_front.x) {
					intersecting = true;
				}
				return intersecting;
			}

			//
			public bool IntersectsBottomY(VoxelVector3Int point)
			{
				bool intersecting = false;
				if (point.y >= corners.bot_left_front.y && point.y < corners.top_left_front.y) {
					intersecting = true;
				}
				return intersecting;
			}

			public bool IntersectsTopY(VoxelVector3Int point)
			{
				bool intersecting = false;
				if (point.y > corners.bot_left_front.y && point.y <= corners.top_left_front.y) {
					intersecting = true;
				}
				return intersecting;
			}

			//
			public bool IntersectsFrontZ(VoxelVector3Int point)
			{
				bool intersecting = false;
				if (point.z >= corners.bot_left_front.z && point.z < corners.bot_left_back.z) {
					intersecting = true;
				}
				return intersecting;
			}

			public bool IntersectsBackZ(VoxelVector3Int point)
			{
				bool intersecting = false;
				if (point.z > corners.bot_left_front.z && point.z <= corners.bot_left_back.z) {
					intersecting = true;
				}
				return intersecting;
			}

			//
			public bool Intersects(VoxelVector3Int point)
			{
				bool intersecting = false;
				if (point.x > corners.bot_left_front.x && point.x < corners.bot_right_front.x) {
					if (point.y > corners.bot_left_front.y && point.y < corners.top_right_front.y) {
						if (point.z > corners.bot_left_front.z && point.z < corners.bot_left_back.z) {
							intersecting = true;
						}
					}
				}
				return intersecting;
			}
		};

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		public static VoxelVector3Int convertVector3ToVoxelVector3Int(Vector3 v)
		{
			return new VoxelVector3Int ((int)v.x, (int)v.y, (int)v.z);
		}

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		public static VoxelCorners createVoxelCorners(VoxelVector3Int pos, int width, int height, int depth)
		{
			VoxelCorners vc    = new VoxelCorners ();
			vc.bot_left_front  = new VoxelVector3Int (pos.x,             pos.y,              pos.z);
			vc.bot_left_back   = new VoxelVector3Int (pos.x,             pos.y,              pos.z + depth - 1);
			vc.bot_right_back  = new VoxelVector3Int (pos.x + width - 1, pos.y,              pos.z + depth - 1);
			vc.bot_right_front = new VoxelVector3Int (pos.x + width - 1, pos.y,              pos.z);
			vc.top_left_front  = new VoxelVector3Int (pos.x,             pos.y + height - 1, pos.z);
			vc.top_left_back   = new VoxelVector3Int (pos.x,             pos.y + height - 1, pos.z + depth - 1);
			vc.top_right_back  = new VoxelVector3Int (pos.x + width - 1, pos.y + height - 1, pos.z + depth - 1);
			vc.top_right_front = new VoxelVector3Int (pos.x + width - 1, pos.y + height - 1, pos.z);

			return vc;
		}
	}
}