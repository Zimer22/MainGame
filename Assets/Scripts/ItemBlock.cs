using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemBlock //Todo: implement detachable slots
{
    Dictionary<int, bool> _occupied = new()
    {
        {0,true},{1,true},{2,true},{3,true},{4,true},{5,true},{6,true},{7,true}
    };
    Dictionary<int, bool> _bUp = new()
    {
        {0,false},{1,false},{2,false},{3,false},{4,false},{5,false},{6,false},{7,false}
    };
    Dictionary<int, bool> _bDown = new()
    {
        {0,false},{1,false},{2,false},{3,false},{4,false},{5,false},{6,false},{7,false}
    };
    Dictionary<int, bool> _bLeft = new()
    {
        {0,false},{1,false},{2,false},{3,false},{4,false},{5,false},{6,false},{7,false}
    };
    Dictionary<int, bool> _bRight = new()
    {
        {0,false},{1,false},{2,false},{3,false},{4,false},{5,false},{6,false},{7,false}
    };
    Dictionary<int, int> _spriteIndex = new()
    {
        {0,0},{1,0},{2,0},{3,0},{4,0},{5,0},{6,0},{7,0}
    };
    event EventHandler ValueChanged;


    //the set values are assigned to event incase of a monobehaviour item changing its shape. the event should be linked to the item itself and jump to its containing inventory
    public Dictionary<int, bool> Occupied
    {
        get{return _occupied;}
        set
        {
            _occupied = value;
            EventHandler();
        }
    }
    public Dictionary<int, bool> BUp
    {
        get{return _bUp;}
        set
        {
            _bUp = value;
            EventHandler();
        }
    }
    public Dictionary<int, bool> BDown
    {
        get{return _bDown;}
        set
        {
            _bDown = value;
            EventHandler();
        }
    }
    public Dictionary<int, bool> BLeft
    {
        get{return _bLeft;}
        set
        {
            _bLeft = value;
            EventHandler();
        }
    }
    public Dictionary<int, bool> BRight
    {
        get{return _bRight;}
        set
        {
            _bRight = value;
            EventHandler();
        }
    }
    public Dictionary<int, int> SpriteIndex
    {
        get { return _spriteIndex; }
        set
        {
            _spriteIndex = value;
            EventHandler();
        }
    }
    void EventHandler()
    {
        ValueChanged.Invoke(this, EventArgs.Empty);
    }

}
