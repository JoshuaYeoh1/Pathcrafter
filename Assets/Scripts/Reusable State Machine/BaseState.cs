using System;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPair
{
    // https://learn.microsoft.com/en-us/dotnet/api/system.func-2?view=netstandard-2.0
    // This predicate accepts any method that takes a float parameter and returns a boolean
    // e.g. static bool Foo(float asdf) {...}
    //      bool HelloThere(float GeneralKenobi) {...}
    //      (float anon) => { bool anonMethod = false; return anonMethod; }
    public Func<float, bool> predicate;

    public BaseState nextState;
}

public abstract class BaseState
{
    public abstract string Name { get; }

    protected List<TransitionPair> Transitions = new List<TransitionPair>();

    private float timeEnteredState = -1f;
    private float timeInState = 0f;

    // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members
    // Even more lazier way to write get Property!
    // The compiler will expand the code below to proper code
    public float TimeEnteredState => timeEnteredState;

    public float TimeInState => timeInState;

    public void Enter()
    {
        //Debug.Log($"Entering {Name}");
        timeEnteredState = Time.time;
        OnEnter();
    }

    public void Update(float deltaTime)
    {
        // We could do "timeInState += deltaTime" instead of below
        // But since we already recorded the exact time state is entered, we can get away with this
        timeInState = Time.time - timeEnteredState;

        OnUpdate(deltaTime);
    }

    public void Exit()
    {
        //Debug.Log($"Exiting {Name}");
        OnExit();
    }

    public void AddTransition(BaseState to, Func<float, bool> predicate)
    {
        Transitions.Add(new TransitionPair { nextState = to, predicate = predicate });
    }

    // Following the style of Dictionary.TryGetValue
    // the method returns true/false for success/failure
    // when true/success, the out variable should be assigned to something that is not null or default.
    public bool TryGetNextTransition(out BaseState state)
    {
        foreach (var t in Transitions)
        {
            // This is the Func<float, bool> predicate in TransitionPair
            if (t.predicate(timeInState))
            {
                state = t.nextState;
                return true;
            }
        }

        // out variables have to be assigned before returning, for false it is null
        state = null;
        return false;
    }

    // These are overridable by subclasses ====================================
    // Why virtual and not abstract?
    // You can change it to abstract, go ahead!
    // I kept it as virtual so that subclasses are not forced to override
    // OnEnter, OnExit and OnUpdate.
    // Imagine if MonoBehaviour forces you to override Awake, Start, OnEnable, etc
    // makes it hard for people to start since it would be overwhelming.

    protected virtual void OnEnter()
    { }

    protected virtual void OnUpdate(float deltaTime)
    { }

    protected virtual void OnExit()
    { }

    // ========================================================================
}