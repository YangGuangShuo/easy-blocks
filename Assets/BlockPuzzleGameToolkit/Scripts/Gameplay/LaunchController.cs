using UnityEngine;

public class LaunchController : MonoBehaviour
{
    [SerializeField]
    private GameObject noNetworkTips;
    void Start()
    {
        GetCfg();
    }

    private void GetCfg()
    {
        ConfigManager.Inst.ToGetConfig((bool isSuccess) =>
        {
            if (!isSuccess)
            {
                noNetworkTips.SetActive(true);
            }
        });
    }

     public void OnNoNetworkBtn()
    {
        noNetworkTips.SetActive(false);
        GetCfg();
    }
}
