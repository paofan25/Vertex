using Cinemachine;
using UnityEngine;

public class EnterRoom : MonoBehaviour
{
    public CinemachineVirtualCamera vcam; // 虚拟相机
    public CinemachineConfiner confiner; // 相机移动限制器

    private void Awake()
    {
        vcam = GetComponentInChildren<CinemachineVirtualCamera>(); // 获取子物体中的虚拟相机
        confiner = GetComponentInChildren<CinemachineConfiner>(); // 获取子物体中的相机移动限制器
        
        EventBus.Subscribe<GetPlayerEvent>(GetPlayer); // 订阅获取玩家事件
        
        confiner.gameObject.SetActive(false); // 默认不激活相机
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            confiner.gameObject.SetActive(true); // 进入场景时激活相机
            EventBus.Publish(new GetCameraEvent(vcam)); // 发布获取相机事件
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            confiner.gameObject.SetActive(false); // 离开场景时禁用相机
        }
    }

    private void GetPlayer(GameEvent gameEvent)
    {
        GetPlayerEvent getPlayerEvent = (GetPlayerEvent) gameEvent;
        vcam.Follow = getPlayerEvent.player.transform; // 设置虚拟相机的跟随对象为玩家
    }
}
