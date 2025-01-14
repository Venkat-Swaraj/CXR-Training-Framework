using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ImmersiveVRTools.Runtime.Common.Utilities {
    public class StreamingAssetsLoader
    {
        public static CoroutineWithData<TType> LoadJson<TType>(string filePathStreamingAssetFolderRelative, MonoBehaviour coroutineRunner)
        {
            return new CoroutineWithData<TType>(coroutineRunner, ParseJsonResultToType<TType>(filePathStreamingAssetFolderRelative, coroutineRunner));
        }

        private static IEnumerator ParseJsonResultToType<TType>(string filePathStreamingAssetFolderRelative, MonoBehaviour coroutineRunner)
        {
            var loadText = StreamingAssetsLoader.LoadText(filePathStreamingAssetFolderRelative, coroutineRunner);
            yield return loadText.Coroutine;

            yield return JsonUtility.FromJson<TType>(loadText.Result);
        }
        
        //files on android have to be loaded via web-request which needs awaiting
        public static CoroutineWithData<string> LoadText(string filePathStreamingAssetFolderRelative, MonoBehaviour coroutineRunner)
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, filePathStreamingAssetFolderRelative);
            if (Application.platform == RuntimePlatform.Android)
            {
                return new CoroutineWithData<string>(coroutineRunner, GetStreamingAssetTextAndroid(filePath));
            }
            else
            {
                return new CoroutineWithData<string>(coroutineRunner, GetStreamingAssetTextStandard(filePath.Replace(@"\", "/")));
            }
        }
        
        private static IEnumerator GetStreamingAssetTextStandard(string filePath)
        {
            yield return File.ReadAllText(filePath);
        }

        private static IEnumerator GetStreamingAssetTextAndroid(string filePath)
        {
            //Possibly could use BetterStreamingAssets https://github.com/gwiazdorrr/BetterStreamingAssets, to get rid of that WebRequest nonsense
            var webRequest = UnityWebRequest.Get(filePath);
            yield return webRequest.SendWebRequest();
            yield return webRequest.downloadHandler.text;
        }
    }
}
