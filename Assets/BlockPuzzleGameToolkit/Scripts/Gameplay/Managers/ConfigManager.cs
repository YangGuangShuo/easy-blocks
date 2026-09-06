using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum CfgType
{
    test = 0,
    online = 1,
    review = 2
}

[System.Serializable]
public class ConfigData
{
    public string iosVersion = "";
    public string androidVersion = "";
    public CfgType iosType
    {
        get
        {
            string version = Application.version;
            return iosVersion == version + "_test" ? CfgType.test : iosVersion == version ? CfgType.review : CfgType.online;
        }
    }

    public CfgType androidType 
    {
        get
        {
            string version = Application.version;
            return androidVersion == version + "_test" ? CfgType.test : androidVersion == version ? CfgType.review : CfgType.online;
        }
    }
}

public class ConfigManager : MonoBehaviour
{
    internal static ConfigManager Inst;

    private ConfigData cfgData = new ConfigData();

    private UnityAction<bool> unityAction = null;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Inst = this;
    }

    public void ToGetConfig(UnityAction<bool> action)
    {
        unityAction = action;
        StartCoroutine(getCfg());
    }

    IEnumerator getCfg()
    {
        string url = "https://end-tower.oss-us-west-1.aliyuncs.com/serverCfg/blockCfg.json";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        // 3. 检查请求是否成功
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"请求失败: {request.error}");
            if (unityAction != null)
            {
                unityAction(false);
            }
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"原始响应: {jsonResponse}");
            // 使用 JsonUtility 将JSON字符串转换为对象
            ConfigData data = JsonUtility.FromJson<ConfigData>(jsonResponse);
            cfgData = data;
            if (data != null)
            {
                Debug.Log($"解析成功: iosVersion={data.iosVersion} :androidVersion={data.androidVersion}");
            }
            else
            {
                Debug.LogWarning("JSON解析失败，请检查字段是否匹配。");
            }
            if (unityAction != null)
            {
                unityAction(true);
            }
            yield return SceneManager.LoadSceneAsync("main");
        }
    }

    public CfgType cfgType()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        return cfgData.iosType;
        #elif UNITY_ANDROID && !UNITY_EDITOR
        return cfgData.androidType;
        #else
        return CfgType.test;
        #endif
    }
}
