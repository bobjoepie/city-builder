using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum KeyAction
{
    LeftClick,
    RightClick,

    Up,
    Left,
    Down,
    Right,

    Use,
    SpaceKey,
    EnterKey,

    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,
    Slot6,
    Slot7,
    Slot8,
    Slot9,
    Slot0,

    Tab,
    Escape,
    Shift,
    Ctrl,
    Alt,

    DialogueContinue,
    DialogueSkip,

    ConfirmClick,
    CancelClick,
    RepeatModifier,

    Rotate,

    RotateLeft,
    RotateRight,

    ZoomIn,
    ZoomOut,
    ResetZoom,

    SpeedUpCamera,

    ConfirmHotkey,
    CancelHotkey,

    CancelSelection,
    DeleteSelected,
    UpgradeSelected,
}

public struct DefaultActionMaps
{
    public static readonly List<KeyAction> MouseKeyActions = new List<KeyAction>()
    {
        KeyAction.LeftClick,
        KeyAction.RightClick,
    };

    public static readonly List<KeyAction> MovementKeyActions = new List<KeyAction>()
    {
        KeyAction.Up,
        KeyAction.Left,
        KeyAction.Down,
        KeyAction.Right,
    };

    public static readonly List<KeyAction> NumberKeyActions = new List<KeyAction>()
    {
        KeyAction.Slot1,
        KeyAction.Slot2,
        KeyAction.Slot3,
        KeyAction.Slot4,
        KeyAction.Slot5,
        KeyAction.Slot6,
        KeyAction.Slot7,
        KeyAction.Slot8,
        KeyAction.Slot9,
        KeyAction.Slot0,
    };

    public static readonly List<KeyAction> MenuKeyActions = new List<KeyAction>()
    {
        KeyAction.Tab,
        KeyAction.Escape,
    };

    public static readonly List<KeyAction> DialogueKeyActions = new List<KeyAction>()
    {
        KeyAction.DialogueContinue,
        KeyAction.DialogueSkip,
    };

    public static readonly List<KeyAction> ConfirmationActions = new List<KeyAction>()
    {
        KeyAction.ConfirmClick,
        KeyAction.CancelClick,
        KeyAction.RepeatModifier,
        KeyAction.ConfirmHotkey,
        KeyAction.CancelHotkey,
    };

    public static readonly List<KeyAction> CameraActions = new List<KeyAction>()
    {
        KeyAction.Up,
        KeyAction.Left,
        KeyAction.Down,
        KeyAction.Right,

        KeyAction.RotateLeft,
        KeyAction.RotateRight,

        KeyAction.ZoomIn,
        KeyAction.ZoomOut,
        KeyAction.ResetZoom,

        KeyAction.SpeedUpCamera,
    };

    public static readonly List<KeyAction> ModalHotkeys = new List<KeyAction>()
    {
        KeyAction.ConfirmHotkey,
        KeyAction.CancelHotkey
    };

    public static readonly List<KeyAction> SelectionActions = new List<KeyAction>()
    {
        KeyAction.CancelSelection,
        KeyAction.DeleteSelected,
        KeyAction.UpgradeSelected,
    };
}

public interface IInputController { }

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private readonly Dictionary<IInputController, HashSet<KeyAction>> actionMaps = new Dictionary<IInputController, HashSet<KeyAction>>();
    private readonly Dictionary<IInputController, HashSet<KeyAction>> heldActionMaps = new Dictionary<IInputController, HashSet<KeyAction>>();

    private readonly Dictionary<KeyAction, List<KeyCode>> KeyActionMap = new Dictionary<KeyAction, List<KeyCode>>()
    {
        {KeyAction.LeftClick            ,       new List<KeyCode> { KeyCode.Mouse0 }},
        {KeyAction.RightClick           ,       new List<KeyCode> { KeyCode.Mouse1 }},

        {KeyAction.Up                   ,       new List<KeyCode> { KeyCode.W }},
        {KeyAction.Left                 ,       new List<KeyCode> { KeyCode.A }},
        {KeyAction.Down                 ,       new List<KeyCode> { KeyCode.S }},
        {KeyAction.Right                ,       new List<KeyCode> { KeyCode.D }},

        {KeyAction.Use                  ,       new List<KeyCode> { KeyCode.E }},
        {KeyAction.SpaceKey             ,       new List<KeyCode> { KeyCode.Space }},
        {KeyAction.EnterKey             ,       new List<KeyCode> { KeyCode.Return }},

        {KeyAction.Slot1                ,       new List<KeyCode> { KeyCode.Alpha1 }},
        {KeyAction.Slot2                ,       new List<KeyCode> { KeyCode.Alpha2 }},
        {KeyAction.Slot3                ,       new List<KeyCode> { KeyCode.Alpha3 }},
        {KeyAction.Slot4                ,       new List<KeyCode> { KeyCode.Alpha4 }},
        {KeyAction.Slot5                ,       new List<KeyCode> { KeyCode.Alpha5 }},
        {KeyAction.Slot6                ,       new List<KeyCode> { KeyCode.Alpha6 }},
        {KeyAction.Slot7                ,       new List<KeyCode> { KeyCode.Alpha7 }},
        {KeyAction.Slot8                ,       new List<KeyCode> { KeyCode.Alpha8 }},
        {KeyAction.Slot9                ,       new List<KeyCode> { KeyCode.Alpha9 }},
        {KeyAction.Slot0                ,       new List<KeyCode> { KeyCode.Alpha0 }},

        {KeyAction.Tab                  ,       new List<KeyCode> { KeyCode.Tab }},
        {KeyAction.Escape               ,       new List<KeyCode> { KeyCode.Escape }},
        {KeyAction.Shift                ,       new List<KeyCode> { KeyCode.LeftShift }},
        {KeyAction.Ctrl                 ,       new List<KeyCode> { KeyCode.LeftControl }},
        {KeyAction.Alt                  ,       new List<KeyCode> { KeyCode.LeftAlt }},

        {KeyAction.DialogueContinue     ,       new List<KeyCode> { KeyCode.Return }},
        {KeyAction.DialogueSkip         ,       new List<KeyCode> { KeyCode.Escape }},

        {KeyAction.ConfirmClick         ,       new List<KeyCode> { KeyCode.Mouse0 }},
        {KeyAction.CancelClick          ,       new List<KeyCode> { KeyCode.Mouse1 }},
        {KeyAction.RepeatModifier       ,       new List<KeyCode> { KeyCode.LeftShift }},

        {KeyAction.Rotate               ,       new List<KeyCode> { KeyCode.R }},

        {KeyAction.RotateLeft           ,       new List<KeyCode> { KeyCode.LeftBracket }},
        {KeyAction.RotateRight          ,       new List<KeyCode> { KeyCode.RightBracket }},

        {KeyAction.ZoomIn               ,       new List<KeyCode> { KeyCode.PageUp }},
        {KeyAction.ZoomOut              ,       new List<KeyCode> { KeyCode.PageDown }},
        {KeyAction.ResetZoom            ,       new List<KeyCode> { KeyCode.Home }},

        {KeyAction.SpeedUpCamera        ,       new List<KeyCode> { KeyCode.LeftShift }},

        {KeyAction.ConfirmHotkey        ,       new List<KeyCode> { KeyCode.Return }},
        {KeyAction.CancelHotkey         ,       new List<KeyCode> { KeyCode.Escape }},

        {KeyAction.CancelSelection      ,       new List<KeyCode> { KeyCode.Escape }},
        {KeyAction.DeleteSelected       ,       new List<KeyCode> { KeyCode.Delete }},
        {KeyAction.UpgradeSelected      ,       new List<KeyCode> { KeyCode.U }},
    };

    private InputManager()
    {
        Instance = this;
    }

    public bool PollKeyDown(IInputController entity, KeyAction action)
    {
        if (actionMaps.ContainsKey(entity) && actionMaps[entity].Contains(action))
        {
            foreach (var key in KeyActionMap[action])
            {
                if (Input.GetKeyDown(key))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool PollKeyUp(IInputController entity, KeyAction action)
    {
        if (actionMaps.ContainsKey(entity) && actionMaps[entity].Contains(action))
        {
            foreach (var key in KeyActionMap[action])
            {
                if (Input.GetKeyUp(key))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool PollKey(IInputController entity, KeyAction action)
    {
        if (actionMaps.ContainsKey(entity) && actionMaps[entity].Contains(action))
        {
            foreach (var key in KeyActionMap[action])
            {
                if (Input.GetKey(key))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool PollKeyDownIgnoreUI(IInputController entity, KeyAction action)
    {
        if (actionMaps.ContainsKey(entity) && actionMaps[entity].Contains(action))
        {
            foreach (var key in KeyActionMap[action])
            {
                if (Input.GetKeyDown(key) && !IsHoveringOverUI())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool PollKeyUpIgnoreUI(IInputController entity, KeyAction action)
    {
        if (actionMaps.ContainsKey(entity) && actionMaps[entity].Contains(action))
        {
            foreach (var key in KeyActionMap[action])
            {
                if (Input.GetKeyUp(key) && !IsHoveringOverUI())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool PollKeyIgnoreUI(IInputController entity, KeyAction action)
    {
        if (actionMaps.ContainsKey(entity) && actionMaps[entity].Contains(action))
        {
            foreach (var key in KeyActionMap[action])
            {
                if (Input.GetKey(key) && !IsHoveringOverUI())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsHoveringOverUI()
    {
        return UIDocManager.Instance.IsHoveringOverUI();
    }

    public void Remap(KeyAction keyAction, List<KeyCode> keyCodes)
    {
        if (KeyActionMap.ContainsKey(keyAction))
        {
            KeyActionMap[keyAction] = keyCodes;
        }
    }

    public void ToggleActionMaps(IInputController entity)
    {
        if (heldActionMaps.ContainsKey(entity))
        {
            ReleaseActionMap(entity);
        }
        else if (actionMaps.ContainsKey(entity))
        {
            HoldActionMap(entity);
        }
    }

    public void HoldActionMap(IInputController entity)
    {
        if (heldActionMaps.ContainsKey(entity))
        {
            heldActionMaps[entity].UnionWith(actionMaps[entity]);
        }
        else
        {
            heldActionMaps.Add(entity, actionMaps[entity]);
        }
        actionMaps.Remove(entity);
    }

    public void ReleaseActionMap(IInputController entity)
    {
        if (actionMaps.ContainsKey(entity))
        {
            actionMaps[entity].UnionWith(heldActionMaps[entity]);
        }
        else
        {
            actionMaps.Add(entity, heldActionMaps[entity]);
        }
        heldActionMaps.Remove(entity);
    }

    public void Register<T>(T entity, List<KeyAction> actionMap)
    {
        switch (entity)
        {
            case IInputController e:
                if (actionMaps.ContainsKey(e))
                {
                    actionMaps[e].UnionWith(actionMap);
                }
                else
                {
                    actionMaps.Add(e, actionMap.ToHashSet());
                }
                break;
        }
    }

    public void Register<T>(T entity, KeyAction action)
    {
        switch (entity)
        {
            case IInputController e:
                if (actionMaps.ContainsKey(e))
                {
                    actionMaps[e].Add(action);
                }
                else
                {
                    actionMaps.Add(e, new HashSet<KeyAction>() { action });
                }
                break;
        }
    }

    public void Unregister<T>(T entity, List<KeyAction> actionMap = null)
    {
        switch (entity)
        {
            case IInputController e:
                if (actionMap == null)
                {
                    actionMaps.Remove(e);
                }
                else if (actionMaps.ContainsKey(e))
                {
                    actionMaps[e].ExceptWith(actionMap);
                }
                break;
        }
    }

    public void Unregister<T>(T entity, KeyAction action)
    {
        switch (entity)
        {
            case IInputController e:
                if (actionMaps.ContainsKey(e))
                {
                    actionMaps[e].Remove(action);
                }
                break;
        }
    }
}
