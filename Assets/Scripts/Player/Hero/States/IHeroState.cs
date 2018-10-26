public interface IHeroState {

    IHeroState Update();
    void Enter(PlayerController pc);
    void Exit();
}
