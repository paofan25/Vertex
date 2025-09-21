/// <summary>
/// 可交互接口
/// </summary>
public interface IInteractable
{
    void Interact(PlayerFacade player);
    bool CanInteract(PlayerFacade player);
    string GetInteractionPrompt();
}