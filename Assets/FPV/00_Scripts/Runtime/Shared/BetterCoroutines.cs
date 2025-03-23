using System.Collections;
using UnityEngine;

namespace FPV.runtime.Shared
{
    ///<summary>
    /// A collection of Coroutines Helpers
    ///</summary>
    internal static class BetterCoroutines
    {
        /*
            usage: yield return CoroutinesHelper.OneSecond; 
            This is better than : yield return new waitforseconds(1);
            because: it doesn't generate garbage.
        */
        public static readonly WaitForSeconds PointZeroOneSeconds = new WaitForSeconds(0.01f);
        public static readonly WaitForSeconds PointZeroFiveSeconds = new WaitForSeconds(0.05f);
        public static readonly WaitForSeconds PointOneSeconds = new WaitForSeconds(0.1f);
        public static readonly WaitForSeconds PointTwoSeconds = new WaitForSeconds(0.2f);
        public static readonly WaitForSeconds PointThreeSeconds = new WaitForSeconds(0.3f);
        public static readonly WaitForSeconds PointFiveSeconds = new WaitForSeconds(0.5f);
        public static readonly WaitForSeconds PointSevenSeconds = new WaitForSeconds(0.7f);
        public static readonly WaitForSeconds PointSevenFiveSeconds = new WaitForSeconds(0.75f);
        public static readonly WaitForSeconds OneSecond = new WaitForSeconds(1);
        public static readonly WaitForSeconds OnePointFiveSeconds = new WaitForSeconds(1.5f);
        public static readonly WaitForSeconds TwoSeconds = new WaitForSeconds(2);
        public static readonly WaitForSeconds ThreeSeconds = new WaitForSeconds(3);
        public static readonly WaitForSeconds FourSeconds = new WaitForSeconds(4);
        public static readonly WaitForSeconds FiveSeconds = new WaitForSeconds(5);
        public static readonly WaitForSeconds EightSeconds = new WaitForSeconds(8);
        public static readonly WaitForSeconds TenSeconds = new WaitForSeconds(10);
        public static readonly WaitForSeconds TwelveSeconds = new WaitForSeconds(12);
        public static readonly WaitForSeconds FifteenSeconds = new WaitForSeconds(15);
        public static readonly WaitForSeconds TwentySeconds = new WaitForSeconds(20);
        public static readonly WaitForSeconds TwentyFiveSeconds = new WaitForSeconds(25);
        static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// EndOfFrame does not work in editor batchmode, so we need a defined
        /// </summary>
        /// <returns></returns>
        public static IEnumerator WaitAFrame()
        {
#if UNITY_EDITOR
            yield return Application.isBatchMode ? null : EndOfFrame;
#else
            yield return EndOfFrame;
#endif
        }

        /// <summary>
        /// Stop and nullify a routine in case the caller is destroyed
        /// </summary>
        /// <param name="routine">The Coroutine to call</param>
        /// <param name="behaviourWhichStartedIt">Caller Behaviour</param>
        public static void StopAndNullifyRoutine(ref Coroutine routine, MonoBehaviour behaviourWhichStartedIt)
        {
            if (routine == null) { return; }
            if (!behaviourWhichStartedIt) { return; }
            behaviourWhichStartedIt.StopCoroutine(routine);
            routine = null;
        }
        
        /// <summary>
        /// Wait for a delay and then execute an action
        /// </summary>
        /// <param name="delay">Delay to await</param>
        /// <param name="action">Action to execute</param>
        /// <returns></returns>
        public static IEnumerator WaitAndDo(IEnumerator delay, System.Action action)
        {
            yield return delay;
            action?.Invoke();
        }

        /// <summary>
        /// Wait for a delay and then execute an action
        /// </summary>
        /// <param name="delay">Delay to await</param>
        /// <param name="action">Action to execute</param>
        /// <returns></returns>
        public static IEnumerator WaitAndDo(YieldInstruction delay, System.Action action)
        {
            yield return delay;
            action?.Invoke();
        }
    }
}