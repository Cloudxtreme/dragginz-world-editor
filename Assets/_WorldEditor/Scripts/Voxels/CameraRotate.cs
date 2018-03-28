//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelChunks
{
	public class CameraRotate : MonoBehaviour
	{
		private float _rotSpeed = 2;

		private Camera _cam;

		void Awake()
		{
			_cam = Camera.main;
		}

		void Update()
		{
			if (Input.GetMouseButton (1)) {
				RotateObject ();
			}
		}

		void RotateObject()
		{
			//Get mouse position
			Vector3 mousePos = Input.mousePosition;

			//Adjust mouse z position
			mousePos.z = _cam.transform.position.y - transform.position.y;   

			//Get a world position for the mouse
			Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(mousePos);   

			//Get the angle to rotate and rotate
			float angle = -Mathf.Atan2(transform.position.z - mouseWorldPos.z, transform.position.x - mouseWorldPos.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angle, 0), _rotSpeed * Time.deltaTime);
		}
	}
}