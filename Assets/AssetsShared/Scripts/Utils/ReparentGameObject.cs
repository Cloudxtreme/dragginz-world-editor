//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;

namespace AssetsShared
{
    public class ReparentGameObject : MonoBehaviour
    {
        public Transform parent;

        private void Start() {

            if (parent) {
                transform.SetParent(parent);
            }
        }
    }
}