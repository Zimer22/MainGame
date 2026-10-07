using UnityEngine;
using UnityEngine.EventSystems;

public class EventManager : Singleton<EventManager>
{
    [SerializeField]EventSystem _esystem;
     public EventSystem ESystem
    {
        get { return _esystem; }
        set { _esystem = value; }
    }
}
