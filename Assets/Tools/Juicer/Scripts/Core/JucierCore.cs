using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

namespace Pelumi.Juicer
{
    public enum AnimationCurveType
    {
        Linear,
        EaseInOut,
        Constant
    }

    public enum TimeMode
    {
        Unscaled,
        Scaled
    }

    public enum LoopType
    {
        Restart,
        Yoyo,
        Incremental
    }

    public enum TextAnimationMode
    {
        ClearOldText,
        AppendNewText,
        ReplaceText
    }

    public enum ShakeRandomnessMode
    {
        Full,
        Harmonic
    }

    public static class JuicerCore
    {
        public static IEnumerator Do<T>(T startingValue, JuicerSetter<T> valueToModify, T destination, JuicerRuntimeParam juicerRuntimeParam, JuicerRuntimeController juicerRuntimeController)
        {
            float currentDelay = juicerRuntimeController.StartDelay;

            while (currentDelay > 0.0f)
            {
                if (!juicerRuntimeController.IsPaused) currentDelay -= juicerRuntimeParam.TimeMode == TimeMode.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            juicerRuntimeController.OnStart();

            float rate = 1.0f / juicerRuntimeController.Duration;
            bool forward = true; // Flag to indicate the animation direction
            float i = forward ? 0.0f : 1.0f; // The current time value
            int loopCount = 1;

            T previousDestination = destination;

            while (true)
            {
                if (!juicerRuntimeController.IsPaused)
                {
                    float timeScale = juicerRuntimeParam.TimeMode == TimeMode.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                    i += forward ? timeScale * rate : -timeScale * rate; // Increment or decrement based on direction

                    if (juicerRuntimeController.Target.Equals(null)) // Check if the target is destroyed
                    {
                        yield break;
                    }

                    valueToModify.Invoke(GetValue(startingValue, destination, juicerRuntimeParam, i));

                    juicerRuntimeController.Process(i);

                    if ((forward && i >= 1.0f) || (!forward && i <= 0.0f))
                    {
                        if (juicerRuntimeParam.LoopCount == 1 || (juicerRuntimeParam.LoopCount >= 1 && loopCount >= juicerRuntimeParam.LoopCount))
                        {
                            juicerRuntimeController.OnCompleted();
                            yield break;
                        }
                        else
                        {
                            loopCount++;
                            juicerRuntimeController.OnCompleteStep();

                            currentDelay = juicerRuntimeController.StepDelay;
                            while (currentDelay > 0)
                            {
                                if (!juicerRuntimeController.IsPaused) currentDelay -= juicerRuntimeParam.TimeMode == TimeMode.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                                yield return null;
                            }

                            switch (juicerRuntimeParam.GetLoopType)
                            {
                                case LoopType.Restart:

                                    i = 0.0f;
                                    forward = true;

                                    break;

                                case LoopType.Yoyo:

                                    forward = !forward;
                                    break;

                                case LoopType.Incremental:

                                    previousDestination = destination;
                                    destination = AddValue( destination , SubtractValue(destination, startingValue));
                                    startingValue = previousDestination;
                                    i = 0.0f;

                                    break;
                                default: break;
                            }
                        }
                    }
                }
                yield return null;
            }
        }

        public static T GetValue<T>(T startingValue, T destination, JuicerRuntimeParam juicerRuntimeParam, float i)
        {
            switch (startingValue)
            {
                case float floatValue when destination is float destFloatValue:
                    return (T)(object)Mathf.Lerp(floatValue, destFloatValue, GetEaseValue(juicerRuntimeParam, i));

                case Vector3 vectorValue when destination is Vector3 destVectorValue:
                    return (T)(object)Vector3.Lerp(vectorValue, destVectorValue, GetEaseValue(juicerRuntimeParam, i));

                case Quaternion quaternionValue when destination is Quaternion destQuaternionValue:
                    return (T)(object)Quaternion.Lerp(quaternionValue, destQuaternionValue, GetEaseValue(juicerRuntimeParam, i));

                case Color colorValue when destination is Color destColorValue:
                    return (T)(object)Color.Lerp(colorValue, destColorValue, GetEaseValue(juicerRuntimeParam, i));

                case Vector4 vectorValue when destination is Vector4 destVectorValue:
                    return (T)(object)Vector4.Lerp(vectorValue, destVectorValue, GetEaseValue(juicerRuntimeParam, i));

                case string stringValue when destination is string destStringValue:
                        return (T)(object)StringLerp(stringValue, destStringValue, juicerRuntimeParam, i);

                default:
                    Debug.LogError("Type not supported");
                    return default(T);
            }
        }

        public static T AddValue<T>(T first, T second)
        {
            switch (first)
            {
                case float floatValue when second is float secondFloatValue:
                    return (T)(object)(floatValue + secondFloatValue);

                case Vector3 vectorValue when second is Vector3 secondVectorValue:
                    //object secondVectorValue = (object)second;
                    //if (secondVectorValue == null)
                    //{
                    //    secondVectorValue = Vector3.zero;
                    //}

                    return (T)(object)(vectorValue + secondVectorValue);

                case Quaternion quaternionValue when second is Quaternion secondQuaternionValue:
                    return (T)(object)AddQuaternion(quaternionValue, secondQuaternionValue);

                case Color colorValue when second is Color secondColorValue:
                    return (T)(object)(colorValue + secondColorValue);

                case Vector4 vectorValue when second is Vector4 secondVector4Value:
                    return (T)(object)(vectorValue + secondVector4Value);

                case string stringValue when second is string secondStringValue:
                    return (T)(object)(stringValue);

                default:
                    Debug.LogError("Type not supported :" + typeof(T));
                    return default(T);
            }
        }

        public static T SubtractValue<T>(T first, T second)
        {
            switch (first)
            {
                case float floatValue when second is float secondFloatValue:
                    return (T)(object)(floatValue - secondFloatValue);

                case Vector3 vectorValue when second is Vector3 secondVectorValue:
                    return (T)(object)(vectorValue - secondVectorValue);

                case Quaternion quaternionValue when second is Quaternion secondQuaternionValue:
                    return (T)(object)SubtractQuaternion(quaternionValue , secondQuaternionValue);

                case Color colorValue when second is Color secondColorValue:
                    return (T)(object)(colorValue - secondColorValue);

                case Vector4 vectorValue when second is Vector4 secondVector4Value:
                    return (T)(object)(vectorValue - secondVector4Value);

                case string stringValue:
                    return (T)(object)(stringValue);

                default:
                    Debug.LogError("Type not supported");
                    return default(T);
            }
        }

        private static Quaternion SubtractQuaternion(Quaternion a, Quaternion b)
        {
            Quaternion difference = Quaternion.Inverse(a) * b; // Perform quaternion subtraction
            return difference;
        }

        private static Quaternion AddQuaternion(Quaternion a, Quaternion b)
        {
            Quaternion sum = a * b;
            return sum;
        }

        public static string StringLerp(string from, string to, JuicerRuntimeParam juicerRuntimeParam, float i)
        {
            int removeCount = Mathf.FloorToInt(Mathf.Lerp(0, from.Length, GetEaseValue(juicerRuntimeParam, i))); // Number of characters to remove
            int addCount = Mathf.FloorToInt(Mathf.Lerp(0, to.Length, GetEaseValue(juicerRuntimeParam, i))); // Number of characters to add

            string trimmedFrom = from.Substring(0, from.Length - removeCount); // Remove characters from the end of 'from'
            string trimmedTo = to.Substring(0, addCount); // Take characters from the start of 'to'

            switch (juicerRuntimeParam.TextAnimationMode)
            {
                case TextAnimationMode.ClearOldText:
                    return trimmedFrom + trimmedTo;

                case TextAnimationMode.AppendNewText:
                    return from + trimmedTo;
    
                case TextAnimationMode.ReplaceText:
                    int remainingCount = Mathf.Max(0, Mathf.Min(addCount, to.Length)); // Number of characters to add from 'to'
                    string addedText = to.Substring(0, remainingCount);
                    return addedText;
            }
            return "";
        }
      
        public static bool HasFinished(float duration, bool forward)
        {
            if ((forward && duration >= 1.0f) || (!forward && duration <= 0.0f))
            {
                return true;
            }
            return false;
        }

        public static float GetEaseValue(JuicerRuntimeParam juicerRuntimeParam, float time)
        {
            if (juicerRuntimeParam.CustomEase != null)
            {
                return juicerRuntimeParam.CustomEase.Evaluate(time);
            }
            else
            {
                return juicerRuntimeParam.EaseFunction(0, 1, time);
            }
        }

        public static float TextToFloat(this string text)
        {
            if (float.TryParse(text, out float result))
            {
                return result;
            }
            else
            {
                Debug.LogError($"Cannot convert {text} to float");
                return 0;
            }
        }

        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
        {
            instance.StopMovement();
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = instance.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0
            );
            return result;
        }

        public static IEnumerator Shake(Vector3 original, JuicerSetter<Vector3> valueToModify, float duration, Vector3 strength, int vibrato, float randomness, bool fadeOut, ShakeRandomnessMode randomnessMode)
        {
            Vector3 originalPosition = original;
            Vector3 startPosition = original;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float timeMultiplier = Mathf.Pow(elapsedTime / duration, 2f); // Gradually reduce the vibration strength over time
                Vector3 randomOffset = Vector3.zero;

                if (randomnessMode == ShakeRandomnessMode.Full)
                {
                    randomOffset = new Vector3(
                        UnityEngine.Random.Range(-strength.x, strength.x),
                        UnityEngine.Random.Range(-strength.y, strength.y),
                        UnityEngine.Random.Range(-strength.z, strength.z)
                    );
                }
                else if (randomnessMode == ShakeRandomnessMode.Harmonic)
                {
                    float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                    float randomMagnitude = UnityEngine.Random.Range(0f, Mathf.Clamp01(randomness / 180f)) * strength.magnitude;

                    randomOffset = new Vector3(
                        Mathf.Cos(randomAngle) * randomMagnitude,
                        Mathf.Sin(randomAngle) * randomMagnitude,
                        Mathf.Cos(randomAngle) * randomMagnitude
                    );
                }

                Vector3 vibratoOffset = Vector3.zero;
                if (vibrato > 0)
                {
                    float vibratoTime = elapsedTime * vibrato;
                    vibratoOffset = new Vector3(
                        Mathf.Sin(vibratoTime) * timeMultiplier * strength.x,
                        Mathf.Sin(vibratoTime) * timeMultiplier * strength.y,
                        Mathf.Sin(vibratoTime) * timeMultiplier * strength.z
                    );
                }

                valueToModify?.Invoke(originalPosition + randomOffset + vibratoOffset);

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            if (fadeOut)
            {
                while (elapsedTime > 0f)
                {
                    float fadeAmount = Mathf.Clamp01(elapsedTime / duration);

                    valueToModify?.Invoke(Vector3.Lerp(originalPosition, startPosition, fadeAmount));

                    elapsedTime -= Time.deltaTime;
                    yield return null;
                }
            }

            valueToModify?.Invoke(originalPosition);
        }
    }

    public class CoroutineHandle : IEnumerator
    {
        public event Action<CoroutineHandle> OnCompleted;
        public bool IsDone { get; private set; }

        public Coroutine Coroutine { get; private set; }
        public object Current {get;}
        public bool MoveNext() => !IsDone;
        public void Reset() { }

        public CoroutineHandle(MonoBehaviour owner, IEnumerator coroutine)
        {
            Coroutine = owner.StartCoroutine(Wrap(coroutine));
        }

        private IEnumerator Wrap(IEnumerator coroutine)
        {
            yield return coroutine;
            IsDone = true;
            OnCompleted?.Invoke(this);
        }
    }

    public class WaitUntilJuicerComplected : CustomYieldInstruction
    {
        private JuicerRuntime _juicerRuntime;
        public override bool keepWaiting => !_juicerRuntime.IsFinished;

        public WaitUntilJuicerComplected(JuicerRuntime juicerRuntime)
        {
            _juicerRuntime = juicerRuntime;
        }
    }

    public class WaitUntilJuicerComplectedOrStep : CustomYieldInstruction
    {
        private JuicerRuntime _juicerRuntime;
        public override bool keepWaiting => !_juicerRuntime.IsFinished && !_juicerRuntime.IsStepCompleted;

        public WaitUntilJuicerComplectedOrStep(JuicerRuntime juicerRuntime)
        {
            _juicerRuntime = juicerRuntime;
        }
    }
}