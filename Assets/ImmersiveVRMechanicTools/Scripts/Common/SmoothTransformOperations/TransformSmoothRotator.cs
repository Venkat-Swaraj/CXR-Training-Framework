using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.SmoothTransformOperations
{
    public static class TransformSmoothRotator
    {
        public static TrackableCoroutine RotateConstantSpeed(Transform transformToRotate, Quaternion end, float anglesPerSecond)
        {
            var secondsForTransition = CalculateDurationNeededToCompleteAtConstantSpeed(end, transformToRotate.rotation, anglesPerSecond);
            return RotateOverSeconds(transformToRotate, end, secondsForTransition);
        }

        public static float CalculateDurationNeededToCompleteAtConstantSpeed(Quaternion end, Quaternion start, float anglesPerSecond)
        {
            var rotationAnglesDifference = Quaternion.Angle(start, end);
            var secondsForTransition = rotationAnglesDifference / anglesPerSecond;
            return secondsForTransition;
        }

        public static TrackableCoroutine RotateOverSeconds(Transform transformToRotate, Quaternion end, float seconds)
        {
            var trackableCoroutine = new TrackableCoroutine();
            return trackableCoroutine.Init(RotateOverSecondsInternal(transformToRotate, end, seconds, trackableCoroutine));
        }

        private static IEnumerator RotateOverSecondsInternal(Transform transformToRotate, Quaternion end, float seconds, TrackableCoroutine trackableCoroutine)
        {
            float elapsedTime = 0;
            var startingRotation = transformToRotate.rotation;
            while (elapsedTime < seconds && !trackableCoroutine.IsForceStopRequested)
            {
                var timeCompletion = elapsedTime / seconds;
                var newRotation = Quaternion.Lerp(startingRotation, end, timeCompletion);
                transformToRotate.rotation = newRotation;
                elapsedTime += Time.deltaTime;

                trackableCoroutine.OnBeforeYieldReturn();
                yield return new WaitForEndOfFrame();
            }

            if (!trackableCoroutine.IsForceStopRequested)
            {
                var finalRotation = Quaternion.Lerp(startingRotation, end, 1);
                transformToRotate.rotation = finalRotation;
            }

            trackableCoroutine.OnFinished();
        }
    }
}