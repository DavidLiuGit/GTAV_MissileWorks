using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.UI;
using GTA.Math;



namespace GFPS
{
	class GuidedMissile : Missile
	{
		#region properties
		// targeting
		protected Entity _targetEntity;

		// initialization
		protected Vector3 _spawnPosition = Vector3.Zero;
		protected Vector3 _spawnRotation = Vector3.Zero;
		#endregion




		#region constructor
		/// <summary>
		/// Missile that locks on to a specific entity, and guides itself to the target
		/// </summary>
		public GuidedMissile()
			: base()
		{
			_targetEntity = Game.Player.Character;
		}

		public GuidedMissile(Entity targetEntity)
			: base()
		{
			_targetEntity = targetEntity;
		}

		public GuidedMissile(Entity targetEntity, Vector3 spawnPosition, Vector3 spawnRotation)
			: this(targetEntity)
		{
			_spawnPosition = spawnPosition;
		}
		#endregion




		#region overridenMethods
		/// <summary>
		/// Apply configurations before spawning the missile
		/// </summary>
		protected override void configure()
		{
			// call parent configure method
			base.configure();

			// overwrite with custom configs
			missileModel = (Model)(-1146260322);
			attachCamera = false;
			timeout = 20000;
		}



		/// <summary>
		/// Spawn the missile as a prop
		/// </summary>
		protected override void spawnMissile()
		{
			// if no spawn position was specified, spawn the missile in front of the player
			if (_spawnPosition == Vector3.Zero)
				_spawnPosition = Game.Player.Character.Position + new Vector3(0f, 1f, 1.5f);

			missile = World.CreateProp(missileModel, _spawnPosition, _spawnRotation, true, false);
		}



		/// <summary>
		/// Configure the missile prop itself
		/// </summary>
		protected override void configureMissileProp()
		{
			// apply physics settings to the missile
			missile.HasGravity = false;
			missile.MaxSpeed = maxCruiseSpeed;
			thrustMagnitude = 1f;

			// particle FX settings
			particleFxOffset = forwardVector * 0.25f;
		}



		/// <summary>
		/// Do not create any camera for the guided missile
		/// </summary>
		protected override void createCamera() {}



		/// <summary>
		/// Control the missile
		/// </summary>
		/// <returns></returns>
		public override bool control()
		{
			// invoke base control method; if it returns false, stop execution
			if (!base.control())
				return false;

			// apply forward thrust
			missile.ApplyForceRelative(thrustMagnitude * forwardVector);

			// determine the missile's position, orientation, velocity, and angular velocity
			Vector3 position = missile.Position;
			Vector3 rotation = missile.Rotation;
			Vector3 velocity = missile.Velocity;
			Vector3 rotVelocity = missile.RotationVelocity;
			printDebug(position, rotation, velocity, rotVelocity);

			// apply turning force
			missile.ApplyForceRelative(-0.5f * forwardVector, new Vector3(0.0005f, 0f, 0f));



			return true;
		}
		#endregion



		#region flightControl

		protected void printDebug(Vector3 position, Vector3 rotation, Vector3 velocity, Vector3 rotVelocity)
		{
			// computations for print out
			Vector3 deltaPosition = position - _spawnPosition;

			string debugStr = "Position: " + deltaPosition.Round().ToString() 
				+ "~n~GndSpeed: " + velocity.Length()
				+ "~n~Rotation: " + rotation.Round().ToString()
				+ "~n~RotSpeed: " + rotVelocity.Round(4).ToString();

			GTA.UI.Screen.ShowHelpTextThisFrame(debugStr);
		}
		#endregion
	}
}
