using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.SmoothTransformOperations
{
    public class TransformSmoothMover
    {
        public static TrackableCoroutine MoveConstantSpeed(Transform transformToMove, Vector3 end, float speedPerSecond) 
            => MoveConstantSpeed(transformToMove, end, speedPerSecond, out float _);
        public static TrackableCoroutine MoveConstantSpeed(Transform transformToMove, Vector3 end, float speedPerSecond, out float secondsForTransition)
        {
            secondsForTransition = CalculateDurationNeededToCompleteAtConstantSpeed(end, transformToMove.position, speedPerSecond);
            return MoveOverSeconds(transformToMove, end, secondsForTransition);
        }

        public static float CalculateDurationNeededToCompleteAtConstantSpeed(Vector3 end, Vector3 start, float speedPerSecond)
        { 
            var distance = Vector3.Distance(start, end);
            return distance / speedPerSecond;
        }

        public static TrackableCoroutine MoveOverSeconds(Transform transformToMove, Vector3 end, float seconds)
        {
            var trackableCoroutine = new TrackableCoroutine();
            return trackableCoroutine.Init(MoveOverSecondsInternal(transformToMove, end, seconds, trackableCoroutine));

        }

        public static IEnumerator MoveOverSecondsInternal(Transform transformToMove, Vector3 end, float seconds, TrackableCoroutine trackableCoroutine)
        {
            float elapsedTime = 0;
            var startingPos = transformToMove.position;
            var vectorToCover = end - startingPos;
            var alreadyCompleted = 0f;
            while (elapsedTime < seconds && !trackableCoroutine.IsForceStopRequested)
            {
                var timeCompletion = elapsedTime / seconds;
                var moveByPercentage = timeCompletion - alreadyCompleted;
                var moveBy = Vector3.Lerp(Vector3.zero, vectorToCover, moveByPercentage);
                transformToMove.position += moveBy;
                alreadyCompleted += moveByPercentage;
                elapsedTime += Time.deltaTime;

                trackableCoroutine.OnBeforeYieldReturn();
                yield return new WaitForEndOfFrame();
            }

            if (!trackableCoroutine.IsForceStopRequested)
            {
                var moveByFinal = Vector3.Lerp(Vector3.zero, vectorToCover, 1 - alreadyCompleted);
                transformToMove.position += moveByFinal;
            }

            trackableCoroutine.OnFinished();
        }
    }
}
