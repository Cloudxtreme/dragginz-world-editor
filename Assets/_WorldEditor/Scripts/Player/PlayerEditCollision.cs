//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AssetsShared;

namespace DragginzWorldEditor
{
	public class PlayerEditCollision : MonoSingleton<PlayerEditCollision>
	{
		public bool isColliding = false;
		public List<Vector3> lastSavePos;
		public int iCurSaveIndex = 0;

		void Awake()
		{
			lastSavePos.Add(transform.position);
			lastSavePos.Add(transform.position);
		}

		void OnCollisionEnter(Collision collision)
		{
			Debug.Log ("1 Collision detected " + collision.gameObject.name);
			isColliding = true;
		}

		void OnCollisionExit(Collision collisionInfo)
		{
			Debug.Log ("2 Collision detected " + collisionInfo.gameObject.name);
			isColliding = false;
		}

		void FixedUpdate()
		{
			if (!isColliding) {
				lastSavePos[iCurSaveIndex] = transform.position;
				iCurSaveIndex = (iCurSaveIndex == 0 ? 1 : 0);
			}
		}
	}
}