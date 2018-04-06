//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using UnityEngine;

namespace AssetsShared
{
    public static class TimerUtils
    {
        public static IEnumerator WaitAndPerform(float bufferTime, Action action)
        {
            yield return new WaitForSeconds(bufferTime);
            action();
        }

        public static IEnumerator CallRepeatedly(float interval, Action action)
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                action();
            }
        }
    }
}
