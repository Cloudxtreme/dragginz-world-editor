//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DragginzWorldEditor
{
	public class PlayerEditCollision : MonoSingleton<PlayerEditCollision>
	{
		public bool isColliding;
		public Vector3 lastSavePos;

		void Awake()
		{
			isColliding = false;
			lastSavePos = transform.position;
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

		void Update()
		{
			if (!isColliding) {
				lastSavePos = transform.position;
			}
		}
	}
}