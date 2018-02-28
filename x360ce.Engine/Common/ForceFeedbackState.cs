﻿using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using x360ce.Engine.Data;

namespace x360ce.Engine
{

	/// <summary>
	/// Class used to store current force feedback state of device.
	/// </summary>
	public class ForceFeedbackState
	{
		public ForceFeedbackState(UserDevice ud)
		{
			LeftRestart = true;
			RightRestart = true;
			LeftPeriodicForce = new PeriodicForce();
			RightPeriodicForce = new PeriodicForce();
			LeftConstantForce = new ConstantForce();
			RightConstantForce = new ConstantForce();
			// Find and assign actuators.
			var actuators = ud.DeviceObjects.Where(x => x.Flags.HasFlag(DeviceObjectTypeFlags.ForceFeedbackActuator)).ToList();
			// If actuator available then...
			if (actuators.Count > 0)
			{
				// Try to find left actuator.
				var actuator = actuators.FirstOrDefault(x => x.Type == ObjectGuid.XAxis);
				// If default actuator not found then take default.
				if (actuator == null)
					actuator = actuators[0];
				actuators.Remove(actuator);
				LeftActuator = actuator;
				LeftParameters = GetParameters(LeftActuator.Offset);
				LeftEnabled = true;
			}
			// If actuator available then...
			if (actuators.Count > 0)
			{
				// Try to find right actuator.
				var actuator = actuators.FirstOrDefault(x => x.Type == ObjectGuid.YAxis);
				// If default actuator not found then take default.
				if (actuator == null)
					actuator = actuators[0];
				actuators.Remove(actuator);
				RightActuator = actuator;
				LeftParameters = GetParameters(RightActuator.Offset);
				RightEnabled = true;
			}
		}

		const uint INFINITE = 0xFFFFFFFF;
		const uint DIEB_NOTRIGGER = 0xFFFFFFFF;

		EffectParameters GetParameters(int offset)
		{
			var p = new EffectParameters();
			// Right-handed Cartesian direction:
			// x: -1 = left,     1 = right,   0 - no direction
			// y: -1 = backward, 1 = forward, 0 - no direction
			// z: -1 = down,     1 = up,      0 - no direction
			// Left motor.
			p.Axes = new int[1] { offset };
			p.Flags = EffectFlags.Cartesian | EffectFlags.ObjectOffsets;
			p.Envelope = new Envelope();
			p.StartDelay = 0;
			p.Duration = unchecked((int)INFINITE);
			p.SamplePeriod = 0;
			p.TriggerButton = unchecked((int)DIEB_NOTRIGGER);
			p.TriggerRepeatInterval = 0;
			return p;
		}

		public void UpdateLeftParameters(int strength, int direction)
		{
			if (!LeftEnabled)
				return;
			LeftParameters.Gain = strength;
			LeftParameters.Directions = new int[1] { direction };
		}
		public void UpdateRightParameters(int strength, int direction)
		{
			if (!RightEnabled)
				return;
			RightParameters.Gain = strength;
			RightParameters.Directions = new int[1] { direction };
		}

		// Left

		public bool LeftEnabled = false;
		public DeviceObjectItem LeftActuator;
		public EffectParameters LeftParameters;
		public Effect LeftEffect;
		public bool LeftRestart;
		public PeriodicForce LeftPeriodicForce;
		public ConstantForce LeftConstantForce;

		// Right

		public bool RightEnabled = false;
		public DeviceObjectItem RightActuator;
		public EffectParameters RightParameters;
		public Effect RightEffect;
		public bool RightRestart;
		public PeriodicForce RightPeriodicForce;
		public ConstantForce RightConstantForce;

	}
}
