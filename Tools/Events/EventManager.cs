using System;
using System.Collections.Generic;

public delegate void Callback();
public delegate void Callback<T0>(T0 arg);
public delegate void Callback<T0, T1>(T0 arg, T1 arg1);
public delegate void Callback<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);

public class EventManager : Singleton<EventManager>
{
    #region Members

    private Dictionary<Enum, Delegate> _noArgsEventsTable = new Dictionary<Enum, Delegate>();
    private Dictionary<Enum, Delegate> _oneArgEventsTable = new Dictionary<Enum, Delegate>();
    private Dictionary<Enum, Delegate> _twoArgsEventTable = new Dictionary<Enum, Delegate>();
    private Dictionary<Enum, Delegate> _threeArgsEventTable = new Dictionary<Enum, Delegate>();

    #endregion Members

    #region API Methods

    private void OnDestroy()
    {
        _noArgsEventsTable.Clear();
        _noArgsEventsTable = null;

        _oneArgEventsTable.Clear();
        _oneArgEventsTable = null;

        _twoArgsEventTable.Clear();
        _twoArgsEventTable = null;

        _threeArgsEventTable.Clear();
        _threeArgsEventTable = null;
    }

    #endregion API Methods

    #region Class Methods

    public static void AddListener(Enum eventType, Callback callback) => Instance.AddListenerToEventTable(eventType, callback);
    public static void AddListener<T0>(Enum eventType, Callback<T0> callback) => Instance.AddListenerToEventTable<T0>(eventType, callback);
    public static void AddListener<T0, T1>(Enum eventType, Callback<T0, T1> callback) => Instance.AddListenerToEventTable<T0, T1>(eventType, callback);
    public static void AddListener<T0, T1, T2>(Enum eventType, Callback<T0, T1, T2> callback) => Instance.AddListenerToEventTable<T0, T1, T2>(eventType, callback);
    public static void Invoke(Enum eventType) => Instance.InvokeEvent(eventType);
    public static void Invoke<T0>(Enum eventType, T0 arg) => Instance.InvokeEvent<T0>(eventType, arg);
    public static void Invoke<T0, T1>(Enum eventType, T0 arg0, T1 arg1) => Instance.InvokeEvent<T0, T1>(eventType, arg0, arg1);
    public static void Invoke<T0, T1, T2>(Enum eventType, T0 arg0, T1 arg1, T2 arg2) => Instance.InvokeEvent<T0, T1, T2>(eventType, arg0, arg1, arg2);
    public static void RemoveListener(Enum eventType, Callback callback) => Instance.RemoveListenerFromEventTable(eventType, callback);
    public static void RemoveListener<T0>(Enum eventType, Callback<T0> callback) => Instance.RemoveListenerFromEventTable<T0>(eventType, callback);
    public static void RemoveListener<T0, T1>(Enum eventType, Callback<T0, T1> callback) => Instance.RemoveListenerFromEventTable<T0, T1>(eventType, callback);
    public static void RemoveListener<T0, T1, T2>(Enum eventType, Callback<T0, T1, T2> callback) => Instance.RemoveListenerFromEventTable<T0, T1, T2>(eventType, callback);

    public void AddListenerToEventTable(Enum eventType, Callback callback)
    {
        if (GetEventCallback(eventType, _noArgsEventsTable, callback))
            _noArgsEventsTable[eventType] = (Callback)_noArgsEventsTable[eventType] + callback;
    }

    public void AddListenerToEventTable<T0>(Enum eventType, Callback<T0> callback)
    {
        if (GetEventCallback(eventType, _oneArgEventsTable, callback))
            _oneArgEventsTable[eventType] = (Callback<T0>)_oneArgEventsTable[eventType] + callback;
    }

    public void AddListenerToEventTable<T0, T1>(Enum eventType, Callback<T0, T1> callback)
    {
        if (GetEventCallback(eventType, _twoArgsEventTable, callback))
            _twoArgsEventTable[eventType] = (Callback<T0, T1>)_twoArgsEventTable[eventType] + callback;
    }

    public void AddListenerToEventTable<T0, T1, T2>(Enum eventType, Callback<T0, T1, T2> callback)
    {
        if (GetEventCallback(eventType, _threeArgsEventTable, callback))
            _threeArgsEventTable[eventType] = (Callback<T0, T1, T2>)_threeArgsEventTable[eventType] + callback;
    }

    public void InvokeEvent(Enum eventType)
    {
        Delegate eventCallback;
        if (_noArgsEventsTable.TryGetValue(eventType, out eventCallback))
        {
            Callback callback = eventCallback as Callback;
            callback.Invoke();
        }
    }

    public void InvokeEvent<T0>(Enum eventType, T0 arg)
    {
        Delegate eventCallback;
        if (_oneArgEventsTable.TryGetValue(eventType, out eventCallback))
        {
            Callback<T0> callback = eventCallback as Callback<T0>;
            callback.Invoke(arg);
        }
    }

    public void InvokeEvent<T0, T1>(Enum eventType, T0 arg0, T1 arg1)
    {
        Delegate eventCallback;
        if (_twoArgsEventTable.TryGetValue(eventType, out eventCallback))
        {
            Callback<T0, T1> callback = eventCallback as Callback<T0, T1>;
            callback.Invoke(arg0, arg1);
        }
    }

    public void InvokeEvent<T0, T1, T2>(Enum eventType, T0 arg0, T1 arg1, T2 arg2)
    {
        Delegate eventCallback;
        if (_threeArgsEventTable.TryGetValue(eventType, out eventCallback))
        {
            Callback<T0, T1, T2> callback = eventCallback as Callback<T0, T1, T2>;
            callback.Invoke(arg0, arg1, arg2);
        }
    }

    public void RemoveListenerFromEventTable(Enum eventType, Callback callback)
    {
        if (CheckRemoveListener(eventType, _noArgsEventsTable, callback))
        {
            _noArgsEventsTable[eventType] = (Callback)_noArgsEventsTable[eventType] - callback;
            _noArgsEventsTable.Remove(eventType);
        }
    }

    public void RemoveListenerFromEventTable<T0>(Enum eventType, Callback<T0> callback)
    {
        if (CheckRemoveListener(eventType, _oneArgEventsTable, callback))
        {
            _oneArgEventsTable[eventType] = (Callback<T0>)_oneArgEventsTable[eventType] - callback;
            _oneArgEventsTable.Remove(eventType);
        }
    }

    public void RemoveListenerFromEventTable<T0, T1>(Enum eventType, Callback<T0, T1> callback)
    {
        if (CheckRemoveListener(eventType, _twoArgsEventTable, callback))
        {
            _twoArgsEventTable[eventType] = (Callback<T0, T1>)_twoArgsEventTable[eventType] - callback;
            _twoArgsEventTable.Remove(eventType);
        }
    }

    public void RemoveListenerFromEventTable<T0, T1, T2>(Enum eventType, Callback<T0, T1, T2> callback)
    {
        if (CheckRemoveListener(eventType, _threeArgsEventTable, callback))
        {
            _threeArgsEventTable[eventType] = (Callback<T0, T1, T2>)_threeArgsEventTable[eventType] - callback;
            _threeArgsEventTable.Remove(eventType);
        }
    }

    private bool GetEventCallback<T>(T eventType, Dictionary<Enum, Delegate> eventsTable, Delegate inputCallback) where T : Enum
    {
        if (!eventsTable.ContainsKey(eventType))
            eventsTable.Add(eventType, null);

        Delegate callback = eventsTable[eventType];
        return callback == null || callback.GetType() == inputCallback.GetType();
    }

    private bool CheckRemoveListener<T>(T eventType, Dictionary<Enum, Delegate> eventsTable, Delegate inputCallback) where T : Enum
    {
        if (!eventsTable.ContainsKey(eventType))
            return false;

        Delegate callback = eventsTable[eventType];
        return !(callback == null || callback.GetType() != inputCallback.GetType());
    }

    #endregion Class Methods
}