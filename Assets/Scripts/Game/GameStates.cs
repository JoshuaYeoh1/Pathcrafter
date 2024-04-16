using UnityEngine;

/////////////////////////////////////////////////////////////////////////////////////////

public class GameState_NavigatorTest : BaseState
{
    public override string Name => "NavigatorTest";

    GameManager gm;

    public GameState_NavigatorTest(GameStateMachine sm)
    {
        gm = sm.gm;
    }

    protected override void OnEnter()
    {
        Debug.Log($"Game State: {Name}");

        GameEventSystem.Current.OnAgentMode();
    }

    protected override void OnUpdate(float deltaTime)
    {
    }

    protected override void OnExit()
    {
    }
}

/////////////////////////////////////////////////////////////////////////////////////////

public class GameState_MapEditor : BaseState
{
    public override string Name => "MapEditor";

    GameManager gm;

    public GameState_MapEditor(GameStateMachine sm)
    {
        gm = sm.gm;
    }

    protected override void OnEnter()
    {
        Debug.Log($"Game State: {Name}");

        GameEventSystem.Current.OnSandboxMode();
    }

    protected override void OnUpdate(float deltaTime)
    {
    }

    protected override void OnExit()
    {
    }
}

/////////////////////////////////////////////////////////////////////////////////////////

