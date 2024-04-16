using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    StateMachine sm;

    [HideInInspector] public GameManager gm;

    void Awake()
    {
        sm = new StateMachine();

        gm=GetComponent<GameManager>();

        // 1. Create all possible states
        // 'this' is passed because each state object has a reference to its owner.
        
        var navigatorTestState = new GameState_NavigatorTest(this);
        var mapEditorState = new GameState_MapEditor(this);

        // 2. Set all transitions
        // TASK: COMPLETE THE TRANSITIONS!

        /////////////////////////////////////////////////////////////////////////////////////////

        navigatorTestState.AddTransition(mapEditorState, (timeInState) =>
        {
            if(gm.sandboxMode) return true;
            else return false;
        });

        /////////////////////////////////////////////////////////////////////////////////////////

        mapEditorState.AddTransition(navigatorTestState, (timeInState) =>
        {
            if(!gm.sandboxMode) return true;
            else return false;
        });

        /////////////////////////////////////////////////////////////////////////////////////////
        
        sm.SetInitialState(navigatorTestState);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {
        sm.Tick(Time.deltaTime);
    }
}
