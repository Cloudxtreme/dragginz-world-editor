//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using System.Collections;

namespace AssetsShared
{
	public class SphereObjects : MonoBehaviour {

		public int numberOfPoints = 64;
		//public float scale = 3.0f;

		// Use this for initialization
		void Start () {

			GameObject innerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			innerSphere.transform.SetParent(transform);
			innerSphere.transform.position = transform.localPosition;
			innerSphere.transform.localScale = transform.localScale;//innerSphere.transform.localScale * (scale * 2);
			innerSphere.transform.name = "Inner Sphere";

			Vector3[] myPoints = getPointsOnSphere(numberOfPoints);

			foreach (Vector3 point in myPoints)
			{
				GameObject outerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				outerSphere.transform.SetParent(transform);
				outerSphere.transform.position = transform.localPosition + (point * transform.localScale.x * .5f);
				outerSphere.transform.localScale = transform.localScale * .25f;
			}
		}

		private Vector3[] getPointsOnSphere(int nPoints)
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
	}
}