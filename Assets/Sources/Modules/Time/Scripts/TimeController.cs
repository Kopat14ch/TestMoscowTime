using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Sources.Modules.Time.Scripts
{
    [RequireComponent(typeof(Button))]
    internal class TimeController : MonoBehaviour
    {
        private const string MoscowTimeUrl = "https://worldtimeapi.org/api/timezone/Europe/Moscow.json";
        private const string TimeFormat = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";
        private Button _button;
        private Coroutine _getMoscowTimeWork;

        private void Awake() => _button = GetComponent<Button>();

        private void OnEnable() => _button.onClick.AddListener(OnButtonClick);

        private void OnDisable() => _button.onClick.RemoveListener(OnButtonClick);

#if !UNITY_EDITOR
[DllImport("__Internal")]
private static extern void ShowMessage(string message); 
#endif


        private void OnButtonClick()
        {
            _getMoscowTimeWork ??= StartCoroutine(GetMoscowTime());
        }

        private IEnumerator GetMoscowTime()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(MoscowTimeUrl))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResult = request.downloadHandler.text;
                    MoscowTimeData data = JsonUtility.FromJson<MoscowTimeData>(jsonResult);

                    if (data == null)
                    {
                        Debug.LogError("DataIsNull");
                    }
                    else
                    {
                        if (DateTime.TryParseExact(data.datetime, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dateTime))
                        {
                            TimeSpan time = dateTime.TimeOfDay;
                            string timeMessage = $"Time: {time.Hours}:{time.Minutes}";
                            Debug.Log(timeMessage);
#if !UNITY_EDITOR
                            ShowMessage(timeMessage);
#endif
                        }
                        else
                        {
                            Debug.LogError("ParseError");
                        }
                    }
                }
                else
                {
                    Debug.Log($"RequestError: {request.error}");
                }
            }

            _getMoscowTimeWork = null;
        }
    }
}