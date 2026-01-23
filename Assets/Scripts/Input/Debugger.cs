using UnityEngine;

public static class Debugger
{
    public static void Log(object message, bool isServer)
    {
        Debug.Log($"<color={(isServer ? "red" : "green")}>{message}</color>");
    }
}
