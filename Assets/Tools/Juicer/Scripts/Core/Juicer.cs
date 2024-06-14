using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pelumi.Juicer
{
    public static class Juicer
    {
        #region Core
        private class JuicerMonoBehaviour : MonoBehaviour { }

        private static JuicerMonoBehaviour JuicerController;

        private static List<JuicerSequencer> juicerSequencers = new List<JuicerSequencer>();

        private static void Init()
        {
            if (JuicerController == null)
            {
                GameObject gameObject = new GameObject("JuicerController");
                JuicerController = gameObject.AddComponent<JuicerMonoBehaviour>();
            }
        }

        public static JuicerSequencer GetJuicerSequencer()
        {
            if (juicerSequencers.Count > 0)
            {
                JuicerSequencer juicerSequencer = juicerSequencers[0];
                juicerSequencer.Reset();
                juicerSequencers.RemoveAt(0);
                return juicerSequencer;
            }
            else
            {
                return new JuicerSequencer();
            }
        }

        public static void ReleaseJuicerSequencer(JuicerSequencer juicerSequencer)
        {
            juicerSequencers.Add(juicerSequencer);
        }

        public static CoroutineHandle StartCoroutine(IEnumerator enumerator)
        {
            Init();
            return JuicerController.RunCoroutine(enumerator);
        }

        public static void StopCoroutine(CoroutineHandle coroutineHandle)
        {
            Init();
            JuicerController.StopCoroutine(coroutineHandle.Coroutine);
        }

        public static CoroutineHandle RunCoroutine(this MonoBehaviour owner, IEnumerator coroutine)
        {
            return new CoroutineHandle(owner, coroutine);
        }

        private static IEnumerator  RunActionAfterInstruction(YieldInstruction instruction, JuicerCallBack action)
        {
            yield return instruction;
            action.Invoke();
        }

        private static IEnumerator RunActionAfterInstruction(CustomYieldInstruction instruction, JuicerCallBack action)
        {
            yield return instruction;
            action.Invoke();
        }
        #endregion

        #region Generic
        public static JuicerRuntimeCore<T> To<T>(JuicerGetter<T> getter, JuicerSetter<T> setter, T end, float duration) where T : struct
        {

            Init();
            JuicerRuntimeCore<T> juicerRuntime = new JuicerRuntimeCore<T>
            (JuicerController, getter,  setter,  end, duration);
            return juicerRuntime;
        }

        public static Coroutine WaitForSeconds(float duration, JuicerCallBack OnComplete)
        {
            Init();
            return JuicerController.StartCoroutine(RunActionAfterInstruction(new WaitForSeconds(duration), OnComplete));
        }

        public static Coroutine WaitForEndOfFrame(JuicerCallBack OnComplete)
        {
            Init();
            return JuicerController.StartCoroutine(RunActionAfterInstruction(new WaitForEndOfFrame(), OnComplete));
        }

        public static Coroutine WaitForFixedUpdate(JuicerCallBack OnComplete)
        {
            Init();
            return JuicerController.StartCoroutine(RunActionAfterInstruction(new WaitForFixedUpdate(), OnComplete));
        }

        public static Coroutine WaitForSecondsRealtime(float duration, JuicerCallBack OnComplete)
        {
            Init();
            return JuicerController.StartCoroutine(RunActionAfterInstruction(new WaitForSecondsRealtime(duration), OnComplete));
        }

        public static Coroutine WaitForCustomYieldInstruction(CustomYieldInstruction instruction, JuicerCallBack OnComplete)
        {
            Init();
            return JuicerController.StartCoroutine(RunActionAfterInstruction(instruction, OnComplete));
        }
        #endregion

        #region Transform - Scale
        public static JuicerRuntimeCore<Vector3> JuicyScale(this Transform transform, Vector3 to, float duration)
        {
            Init();

            JuicerRuntimeCore<Vector3> juicerRuntime = new JuicerRuntimeCore<Vector3>
            (transform,
                () => transform.localScale,
                (value) =>
                {
                    transform.localScale = value;
                },
                to, duration
            );
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyScale(this Transform transform, float to, float duration)
        {
            Init();

            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform,
                () => transform.localScale.x,
                (value) =>
                {
                    transform.localScale = new Vector3(value, value, value);
                },
                to, duration
            );
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyScaleX(this Transform transform, float to, float duration)
        {
            Init();

            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform,
                () => transform.localScale.x,
                (value) => transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z),
                to, duration
            );
            
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyScaleY(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.localScale.y, (value) => transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z),to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyScaleZ(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.localScale.z, (value) => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value), to, duration);
            return juicerRuntime;
        }

        public static CoroutineHandle JuicyShakeScale(this Transform transform, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            Init();
            return StartCoroutine(JuicerCore.Shake(transform, transform.localScale, (value) => transform.localScale = value, duration, strength, vibrato, randomness, fadeOut, randomnessMode));
        }

        #endregion

        #region Transform - World Position

        public static JuicerRuntimeCore<Vector3> JuicyAnchoredMove(this RectTransform rectTransform, Vector3 to, float duration)
        {
            Init();
            JuicerRuntimeCore<Vector3> juicerRuntime = new JuicerRuntimeCore<Vector3>
            (rectTransform, () => rectTransform.anchoredPosition, (value) => rectTransform.anchoredPosition = value, to, duration);
            return juicerRuntime;
        }


        public static JuicerRuntimeCore<Vector3> JuicyMove(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerRuntimeCore<Vector3> juicerRuntime = new JuicerRuntimeCore<Vector3>
            (transform, () => transform.position, (value) => transform.position = value,to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyMoveX(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.position.x, (value) => transform.position = new Vector3(value, transform.position.y, transform.position.z), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyMoveY(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.position.y, (value) => transform.position = new Vector3(transform.position.x, value, transform.position.z), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyMoveZ(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.position.z, (value) => transform.position = new Vector3(transform.position.x, transform.position.y, value), to, duration);
            return juicerRuntime;
        }

#endregion

        #region Transform - Local Position

        public static JuicerRuntimeCore<Vector3> JuicyLocalMove(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerRuntimeCore<Vector3> juicerRuntime = new JuicerRuntimeCore<Vector3>
            (transform, () => transform.localPosition, (value) => transform.localPosition = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyLocalMoveX(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.localPosition.x, (value) => transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyLocalMoveY(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.localPosition.y, (value) => transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyLocalMoveZ(this Transform transform, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (transform, () => transform.localPosition.z, (value) => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value), to, duration);
            return juicerRuntime;
        }

        public static CoroutineHandle JuicyShakePosition(this Transform transform, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            Init();
            return StartCoroutine(JuicerCore.Shake(transform, transform.localPosition, (value) => transform.localPosition = value, duration, strength, vibrato, randomness, fadeOut, randomnessMode));
        }

        public static CoroutineHandle JuicyShakePosition(this RectTransform transform, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            Init();
            return StartCoroutine(JuicerCore.Shake(transform, transform.anchoredPosition, (value) => transform.anchoredPosition = value, duration, strength, vibrato, randomness, fadeOut, randomnessMode));
        }

        #endregion

        #region Transform - World Rotation
        public static JuicerRuntimeCore<Vector3> JuicyRotate(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerRuntimeCore<Vector3> juicerRuntime = new JuicerRuntimeCore<Vector3>
            (transform, () => transform.eulerAngles, (value) => transform.eulerAngles = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<Quaternion> JuicyRotateQuaternion(this Transform transform, Quaternion to, float duration)
        {
            Init();
            JuicerRuntimeCore<Quaternion> juicerRuntime = new JuicerRuntimeCore<Quaternion>
            (transform, () => transform.rotation, (value) => transform.rotation = value, to, duration);
            return juicerRuntime;
        }
        #endregion

        #region Transform - Local Rotation
        public static JuicerRuntimeCore<Vector3> JuicyLocalRotate(this Transform transform, Vector3 to, float duration)
        {
            Init();
            JuicerRuntimeCore<Vector3> juicerRuntime = new JuicerRuntimeCore<Vector3>
            (transform, () => transform.localEulerAngles, (value) => transform.localEulerAngles = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<Quaternion> JuicyLocalRotateQuaternion(this Transform transform, Quaternion to, float duration)
        {
            Init();
            JuicerRuntimeCore<Quaternion> juicerRuntime = new JuicerRuntimeCore<Quaternion>
            (transform, () => transform.localRotation, (value) => transform.localRotation = value, to, duration);
            return juicerRuntime;
        }

        public static CoroutineHandle JuicyShakeRotation(this Transform transform, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            Init();
            return StartCoroutine(JuicerCore.Shake(transform, transform.localEulerAngles, (value) => transform.rotation = Quaternion.Euler(value), duration, strength, vibrato, randomness, fadeOut, randomnessMode));
        }
        #endregion

        #region Material
        public static JuicerRuntimeCore<Color> JuicyColour(this Material material, Color to, float duration)
        {
            Init();
            JuicerRuntimeCore<Color> juicerRuntime = new JuicerRuntimeCore<Color>
            (material, () => material.color, (value) => material.color = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyAlpha(this Material material, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (material, () => material.color.a, (value) => material.color = new Color(material.color.r, material.color.g, material.color.b, value), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyFloatProperty(this Material material, string property, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
                (material ,() => material.GetFloat(property), (value) => material.SetFloat(property, value), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyVectorProperty(this Material material, string property, Vector4 to, float duration)
        {
            Init();
            JuicerRuntimeCore<Vector4> juicerRuntime = new JuicerRuntimeCore<Vector4>
                (material, () => material.GetVector(property), (value) => material.SetVector(property, value), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntime JuicyColourProperty(this Material material, string property, Color to, float duration)
        {
            Init();
            JuicerRuntimeCore<Color> juicerRuntime = new JuicerRuntimeCore<Color>
                (material, () => material.GetColor(property), (value) => material.SetColor(property, value), to, duration);

            return juicerRuntime;
        }
        #endregion

        #region UI
        public static JuicerRuntimeCore<float> JuicyAlpha(this CanvasGroup canvasGroup, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (canvasGroup, () => canvasGroup.alpha, (value) => canvasGroup.alpha = value, to, duration);
            juicerRuntime.SetOnStart(() =>
            {
                canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
                canvasGroup.interactable = canvasGroup.alpha > 0;
            });

            juicerRuntime.SetOnCompleted(() => 
            {
                canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
                canvasGroup.interactable = canvasGroup.alpha > 0;
            });

            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyFillAmount(this Image image, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (image, () => image.fillAmount, (value) => image.fillAmount = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<Color> JuicyColour(this Image image, Color to, float duration)
        {
            Init();
            JuicerRuntimeCore<Color> juicerRuntime = new JuicerRuntimeCore<Color>
            (image, () => image.color, (value) => image.color = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<Color> JuicyColour(this TMP_Text textMeshPro, Color to, float duration)
        {
            Init();
            JuicerRuntimeCore<Color> juicerRuntime = new JuicerRuntimeCore<Color>
            (textMeshPro, () => textMeshPro.color, (value) => textMeshPro.color = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyAlpha(this TMP_Text textMeshPro, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (textMeshPro, () => textMeshPro.color.a, (value) => textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, value), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyAlpha(this Image image, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (image, () => image.color.a, (value) => image.color = new Color(image.color.r, image.color.g, image.color.b, value), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<string> JuicyText(this TMP_Text textMeshPro, string to, float duration)
        {
            Init();
            JuicerRuntimeCore<string> juicerRuntime = new JuicerRuntimeCore<string>
            (textMeshPro, () => textMeshPro.text, (value) => textMeshPro.text = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyTextNumber(this TMP_Text textMeshPro, float to, float duration, string format = "F0")
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (textMeshPro, () => float.Parse(textMeshPro.text), (value) => textMeshPro.text = value.ToString(format), to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<Vector3> JuicyScroll(this ScrollRect scrollRect, Vector3 to, float duration)
        {
            Init();
            JuicerRuntimeCore<Vector3> juicerRuntime = new JuicerRuntimeCore<Vector3>
            (scrollRect, () => scrollRect.normalizedPosition, (value) => scrollRect.normalizedPosition = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyValue(this Slider slider, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
            (slider, () => slider.value, (value) => slider.value = value, to, duration);
            return juicerRuntime;
        }
        #endregion

        #region Renderer
        public static JuicerRuntimeCore<float> JuicyFloatProperty(this Renderer renderer, string propertyName, float to, float duration)
        {
            Init();
            MaterialPropertyBlock materialProperty = new MaterialPropertyBlock();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
                (renderer, () =>
                {
                    renderer.GetPropertyBlock(materialProperty);
                    return materialProperty.GetFloat(propertyName);
                }, (value) =>
                {
                    materialProperty.SetFloat(propertyName, value);
                    renderer.SetPropertyBlock(materialProperty);
                }, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<Vector4> JuicyVectorProperty(this Renderer renderer, string propertyName, Vector4 to, float duration)
        {
            Init();
            MaterialPropertyBlock materialProperty = new MaterialPropertyBlock();
            JuicerRuntimeCore<Vector4> juicerRuntime = new JuicerRuntimeCore<Vector4>
                (renderer, () =>
                {
                    renderer.GetPropertyBlock(materialProperty);
                    return materialProperty.GetVector(propertyName);
                }, (value) =>
                {
                    materialProperty.SetVector(propertyName, value);
                    renderer.SetPropertyBlock(materialProperty);
                }, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<Color> JuicyColorProperty(this Renderer renderer, string propertyName, Color to, float duration)
        {
            Init();
            MaterialPropertyBlock materialProperty = new MaterialPropertyBlock();
            JuicerRuntimeCore<Color> juicerRuntime = new JuicerRuntimeCore<Color>
                (renderer, () =>
                {
                    renderer.GetPropertyBlock(materialProperty);
                    return materialProperty.GetColor(propertyName);
                }, (value) =>
                {
                    materialProperty.SetColor(propertyName, value);
                    renderer.SetPropertyBlock(materialProperty);
                }, to, duration);
            return juicerRuntime;
        }

        #endregion

        #region LineRenderer

        public static JuicerRuntimeCore<float> JuicyWidth(this LineRenderer lineRenderer, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
                (lineRenderer, () => lineRenderer.widthMultiplier, (value) => lineRenderer.widthMultiplier = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyStartWidth(this LineRenderer lineRenderer, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
                (lineRenderer, () => lineRenderer.startWidth, (value) => lineRenderer.startWidth = value, to, duration);
            return juicerRuntime;
        }

        public static JuicerRuntimeCore<float> JuicyEndWidth(this LineRenderer lineRenderer, float to, float duration)
        {
            Init();
            JuicerRuntimeCore<float> juicerRuntime = new JuicerRuntimeCore<float>
                (lineRenderer, () => lineRenderer.endWidth, (value) => lineRenderer.endWidth = value, to, duration);
            return juicerRuntime;
        }
        #endregion

        #region TextMesh
        public static void ChangeCharacterColor(this TextMeshProUGUI textMeshPro, int characterIndex, Color color)
        {
            textMeshPro.ForceMeshUpdate();
            Color[] colors = textMeshPro.mesh.colors;
            colors[4 * characterIndex] = color;
            colors[4 * characterIndex + 1] = color;
            colors[4 * characterIndex + 2] = color;
            colors[4 * characterIndex + 3] = color;
            textMeshPro.mesh.colors = colors;
            textMeshPro.UpdateGeometry(textMeshPro.mesh, 0);
        }
        #endregion
    }
}