using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    /*
     * https://github.com/neuneu9/unity-volume-slider/blob/master/VolumeSlider.cs
     */

    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string parameter = "Volume";

        private void Awake()
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }

        private void OnEnable()
        {
            mixer.GetFloat(parameter, out var volume);
            slider.value = Db2Pa(volume);
            slider.onValueChanged.AddListener(value => mixer.SetFloat(parameter, Pa2Db(value)));
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveAllListeners();
        }

        /// <summary>
        ///     デシベル変換
        ///     0, 1, 10の音圧→-80, 0, 20のデシベル
        /// </summary>
        /// <param name="pa"></param>
        /// <returns></returns>
        private float Pa2Db(float pa)
        {
            pa = Mathf.Clamp(pa, 0.0001f, 10f);
            return 20f * Mathf.Log10(pa);
        }

        /// <summary>
        ///     音圧変換
        ///     -80, 0, 20のデシベル→0, 1, 10の音圧
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private float Db2Pa(float db)
        {
            db = Mathf.Clamp(db, -80f, 20f);
            return Mathf.Pow(10f, db / 20f);
        }
    }
}