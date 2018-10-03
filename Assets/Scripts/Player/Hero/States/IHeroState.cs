public interface IHeroState {

    IHeroState Update();
    void Enter(Hero hero);
    void Exit();
}
