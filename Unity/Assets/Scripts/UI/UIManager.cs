using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject agentModeUI;
    public GameObject sandboxModeUI;

    void Start()
    {
        GameEventSystem.Current.AgentModeEvent += OnAgentMode;
        GameEventSystem.Current.SandboxModeEvent += OnSandboxMode;
    }

    void OnDestroy()
    {
        GameEventSystem.Current.AgentModeEvent -= OnAgentMode;
        GameEventSystem.Current.SandboxModeEvent -= OnSandboxMode;
    }

    void OnAgentMode()
    {
        agentModeUI.SetActive(true);
        sandboxModeUI.SetActive(false);
    }

    void OnSandboxMode()
    {
        agentModeUI.SetActive(false);
        sandboxModeUI.SetActive(true);
    }
}
