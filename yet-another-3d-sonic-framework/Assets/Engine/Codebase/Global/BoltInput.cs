using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public static class HInput{
	public static float GetAxis(string axisName) => BoltInput.Instance.GetAxis(axisName);
	public static Vector2 GetAxis2D(string axisName) => BoltInput.Instance.GetAxis2D(axisName);

	public static bool GetButton(string axisName) => BoltInput.Instance.GetButton(axisName);
	public static bool GetButtonDown(string axisName) => BoltInput.Instance.GetButtonDown(axisName);
	public static bool GetButtonUp(string axisName) => BoltInput.Instance.GetButtonUp(axisName);

	//--------Functions prefixed with F = force functions, bypasses BoltInput.BlockInput--------//
	public static float FGetAxis(string axisName) => BoltInput.Instance.FGetAxis(axisName);
	public static Vector2 FGetAxis2D(string axisName) => BoltInput.Instance.FGetAxis2D(axisName);

	public static bool FGetButton(string axisName) => BoltInput.Instance.FGetButton(axisName);
	public static bool FGetButtonDown(string axisName) => BoltInput.Instance.FGetButtonDown(axisName);
	public static bool FGetButtonUp(string axisName) => BoltInput.Instance.FGetButtonUp(axisName);
	
	//-----------------------------------------------------------------------------------------//
	public static void BlockInput() => BoltInput.Instance.BlockInput = true;
	public static void UnblockInput() => BoltInput.Instance.BlockInput = false;
	
	public static InputType ActiveInputType;

	public static void BlacklistInput(params string[] names){
		foreach(string s in names){
			if (BoltInput.Instance.BlacklistedInputs.Contains(s)) continue;
			else BoltInput.Instance.BlacklistedInputs.Add(s);
		}
	}
	public static void RemoveBlacklistInput(params string[] names){
		foreach(string s in names){
			if (BoltInput.Instance.BlacklistedInputs.Contains(s)) BoltInput.Instance.BlacklistedInputs.Remove(s);
		}
	}
}

/// <summary>
/// Input handler for the Blur Framework
/// 2023 - strix
/// </summary>
/// 
public enum InputType{
	Keyboard, Mouse, Gamepad
};

public class BoltInput : MonoBehaviour
{
	public PlayerInput MainInput;
	
	public List<UButton> Buttons;
	public List<UAxis1D> Axis1D;
	public List<UAxis2D> Axis2D;
	
	public List<string> BlacklistedInputs = new List<string>();
	public List<string> BlacklistedMaps = new List<string>();
	
	public static BoltInput Instance;
	
	public bool BlockInput;
		
	void Awake()
	{
        BoltInput.Instance = this;
		MainInput.onActionTriggered += OnActionTriggered;
		Buttons = new List<UButton>(); Axis1D = new List<UAxis1D>(); Axis2D = new List<UAxis2D>();
	    foreach(InputActionMap a in MainInput.actions.actionMaps){
	    	foreach(InputAction s in a.actions){
	    		switch(s.type){
	    			case InputActionType.Button:
		    			UButton button = new UButton();
	    				button.button = s;
	    				button.Name = s.name;
	    				button.MapName = a.name;
	    				Buttons.Add(button);
	    				break;
	    			
	    			case InputActionType.Value:
	    				switch(s.expectedControlType){
	    					case "Axis":
	    						UAxis1D axis1 = new UAxis1D();
	    						axis1.Name = s.name;
	    						Axis1D.Add(axis1);
	    						break;
	    					case "Vector2":
	    						UAxis2D axis2 = new UAxis2D();
	    						axis2.Name = s.name;
	    						Axis2D.Add(axis2);
	    						break;
	    				}
	    				break;
	    		}
	    	}
	    }
    }
	internal UButton UBGet(string name) => Buttons.Find(s=>s.Name == name);
	internal UAxis1D UA1Get(string name) => Axis1D.Find(s=>s.Name == name);
	internal UAxis2D UA2Get(string name) => Axis2D.Find(s=>s.Name == name);
	void OnActionTriggered(InputAction.CallbackContext callbackContext)
	{
		print(callbackContext.action.activeControl.device.displayName);
		switch(callbackContext.action.activeControl.device.displayName){
			default:
				HInput.ActiveInputType = InputType.Gamepad;
				break;
			case "Mouse":
                HInput.ActiveInputType = InputType.Mouse;
				break;
			case "Keyboard":
                HInput.ActiveInputType = InputType.Keyboard;
				break;
		}
		switch (callbackContext.action.type)
		{
			case InputActionType.Button:
				UBGet(callbackContext.action.name).hold = callbackContext.ReadValueAsButton();
				break;
			case InputActionType.Value:
				switch(callbackContext.action.expectedControlType){
					case "Axis":
						UA1Get(callbackContext.action.name).Value = callbackContext.ReadValue<float>();
						break;
					case "Vector2":
						UA2Get(callbackContext.action.name).Value = callbackContext.ReadValue<Vector2>();
						break;
				}
				break;
		}
	}
	public bool GetButtonDown(string name) { if (!BlacklistedInputs.Contains(name) && !BlockInput) return UBGet(name).pressed; else return false;}
	public bool GetButtonUp(string name) { if (!BlacklistedInputs.Contains(name) && !BlockInput) return UBGet(name).released; else return false;}
	public bool GetButton(string name) { if (!BlacklistedInputs.Contains(name) && !BlockInput) return UBGet(name).hold; else return false;}
	
	public float GetAxis(string name) {if (!BlacklistedInputs.Contains(name) && !BlockInput) return UA1Get(name).Value; else return 0;}
	public Vector2 GetAxis2D(string name) {if (!BlacklistedInputs.Contains(name) && !BlockInput) return UA2Get(name).Value; else return Vector2.zero;}
	
	//Force functions always return the value even if its name or parent map are blacklisted
	public bool FGetButtonDown(string name) => UBGet(name).pressed;
	public bool FGetButtonUp(string name) => UBGet(name).released;
	public bool FGetButton(string name) => UBGet(name).hold;
	
	public float FGetAxis(string name) => UA1Get(name).Value;
	public Vector2 FGetAxis2D(string name) => UA2Get(name).Value;
}

public class UAxis1D{
	public string Name;
	public float Value;
}
[System.Serializable]
public class UAxis2D{
	public string Name;
	public Vector2 Value;
}
[System.Serializable]
public class UButton
{
	public InputAction button;
	public string Name; public string MapName; //Define a public name that we can index later with helper functions
	public Action press, release;

	public float PressedTime, FPressedTime, ReleasedTime, FReleasedTime;
	[HideInInspector] public bool hold
    {
	    get => _hold;
	    set{
	    	if (_hold == value) return;
	    	_hold = value;
	    	if (_hold){
	    		PressedTime = Time.unscaledTime;
	    		FPressedTime = Time.fixedUnscaledTime;
	    	}
	    	else{
	    		ReleasedTime = Time.unscaledTime;
	    		FReleasedTime = Time.fixedUnscaledTime;
	    	}
	    }
    }
	public bool upressed;
	
	public bool pressed => !Time.inFixedTimeStep ? PressedTime == Time.unscaledTime : FPressedTime == Time.fixedUnscaledTime;
	public bool released =>!Time.inFixedTimeStep ? ReleasedTime == Time.unscaledTime : FReleasedTime == Time.fixedUnscaledTime;

    bool _hold;
}
