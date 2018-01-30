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
		public bool isColliding = false;

		void OnCollisionEnter(Collision collision)
		{
			//Debug.Log ("Collision detected " + collision.gameObject.name);
			isColliding = true;
		}

		void OnCollisionExit(Collision collisionInfo)
		{
			isColliding = false;
		}
	}
}