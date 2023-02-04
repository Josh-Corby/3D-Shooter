using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : Utilities
{
    protected static PlayerManager PM { get { return PlayerManager.INSTANCE; } }
    protected static InputManager IM { get { return InputManager.INSTANCE; } }
    protected static UIManager UI { get { return UIManager.INSTANCE; } }
    protected static GameManager GM { get { return GameManager.INSTANCE; } }
    protected static WaveManager WM { get { return WaveManager.INSTANCE; } }
}
public class GameBehaviour<T> : GameBehaviour where T : GameBehaviour
{
    public bool dontDestroy;

    private static T instance_;
    public static T INSTANCE
    {
        get
        {
            if (instance_ == null)
            {
                instance_ = GameObject.FindObjectOfType<T>();
                if (instance_ == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    singleton.AddComponent<T>();
                }
            }
            return instance_;
        }
    }
    protected virtual void Awake()
    {
        if (instance_ == null)
        {
            instance_ = this as T;
            if (dontDestroy) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
