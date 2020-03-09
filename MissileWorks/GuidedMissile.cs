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

		// physics consts
		protected const float gravitationalAccel = -9.80665f;		// m/s^2
		protected const float airMolarMass = 0.0289644f;			// kg/Mol
		protected const float universalGasConst = 8.3144598f;		// N·m/(mol·K)
		protected const float standardTemperature = 273.15f;		// Kelvin

		// aerodynamics
		protected float maxDragForceMultiplier = -0.5f;		// value should be negative, since drag acts opposite the fwd vector
		protected float maxDragOffset = 0.001f;
		protected float maxDampingOffset = 0.001f;
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
				_spawnPosition = Helper.getVector3NearTarget(5f, Game.Player.Character.Position);

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

			// if the targeted entity no longer exists, don't do anything more
			if (!_targetEntity.Exists())
				return true;

			// determine the missile's position, orientation, velocity, and angular velocity
			Vector3 position = missile.Position;
			Vector3 rotation = missile.Rotation;
			Vector3 velocity = missile.Velocity;
			Vector3 rotVelocity = missile.RotationVelocity;
			//printDebug(position, rotation, velocity, rotVelocity);

			// apply aerodynamic forces
			applyAerodynamicForces(position, rotation, velocity, rotVelocity);
			//missile.ApplyForceRelative(-0.5f * forwardVector, new Vector3(0.0005f, 0f, 0f));



			return true;
		}
		#endregion



		#region flightControl
		/// <summary>
		/// Apply aerodynamic forces to slow down, rotate, or dampen the rotation of the missile
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="velocity"></param>
		/// <param name="rotVelocity"></param>
		protected void applyAerodynamicForces(Vector3 position, Vector3 rotation, Vector3 velocity, Vector3 rotVelocity)
		{
			// compute aerodynamic properties & ratios
			float airSpeed = velocity.Length();		// ||velocity||, assuming no wind; always positive or 0
			float densityMultipler = computeApproxAirDensityMultiplier(position.Z);
			float airSpeedDragMultiplier = computeAirSpeedDragMultiplier(airSpeed);
			float dragForceMultiplier = maxDragForceMultiplier * densityMultipler * airSpeedDragMultiplier;

			// compute the shortest-path-to-target vector, and its Euler angles
			Vector3 targetVector = _targetEntity.Position - position;
			Vector3 targetAngles = Helper.getEulerAngles(targetVector.Normalized);

			// apply rotational force; this force is caused by drag
			applySteeringForce(targetAngles, velocity, rotation, rotVelocity, dragForceMultiplier);

			// apply forces 
			//applyStabilizingForces(velocity, rotation, rotVelocity, dragForceMultiplier);
		}



		/// <summary>
		/// Apply a force, induced by drag on the missile's control surfaces, to the missile in order 
		/// to steer it where it needs to go. The magnitude of the force depends on drag.
		/// </summary>
		/// <param name="targetAngle"></param>
		/// <param name="velocity"></param>
		/// <param name="rotation"></param>
		/// <param name="rotVelocity"></param>
		/// <param name="dragMultiplier"></param>
		protected void applySteeringForce(Vector3 targetAngle, Vector3 velocity, Vector3 rotation, Vector3 rotVelocity, float dragMultiplier)
		{

			// compute the angle delta; that is, the difference between the missile's rotation, 
			// and the rotation it needs to face the target
			float targetYaw = Helper.angleDelta2D(targetAngle.Z, rotation.Z);

			// apply drag for accordingly
			Vector3 dragOffset = Vector3.Zero;

			if (targetYaw > 0f) dragOffset = new Vector3(-maxDragOffset, 0f, 0f);
			else if (targetYaw < 0f) dragOffset = new Vector3(maxDragOffset, 0f, 0f);

			missile.ApplyForceRelative(dragMultiplier * forwardVector, dragOffset);
			
			// debug printout
			string infoString = "TgtYaw: " + targetYaw + "~n~AngVel.Z: " + rotVelocity.Z;
			GTA.UI.Screen.ShowHelpTextThisFrame(infoString);
		}



		/// <summary>
		/// Apply a damping force to the missile's rotation (pitch & yaw). The force depends on drag, and how
		/// far off-axis the missile's rotation is from it's velocity vector
		/// </summary>
		/// <param name="velocity"></param>
		/// <param name="rotation"></param>
		/// <param name="rotVelocity"></param>
		/// <param name="dragMultiplier"></param>
		protected void applyStabilizingForces(Vector3 velocity, Vector3 rotation, Vector3 rotVelocity, float dragMultiplier)
		{
			// the magnitude of the rotational dampening force depends on 

			//missile
		}



		/// <summary>
		/// Compute air density (approximate) multiplier, assuming 0 temperature lapse rate. That is, 
		/// assuming temperature does not change with altitude. The approximation is based on barometric density eqn.
		/// See: https://en.wikipedia.org/wiki/Barometric_formula#Density_equations
		/// </summary>
		/// <param name="altitude">Altitude, in meters (GTA standard unit)</param>
		/// <returns>M</returns>
		protected float computeApproxAirDensityMultiplier(float altitude)
		{
			return (float)Math.Exp((gravitationalAccel * airMolarMass * altitude) / (universalGasConst * standardTemperature));
		}



		/// <summary>
		/// Compute drag multiplier due to air speed. Drag is proportional to airSpeed squared
		/// </summary>
		/// <param name="airSpeed">Air speed, computed as </param>
		/// <returns></returns>
		protected float computeAirSpeedDragMultiplier(float airSpeed)
		{
			return (float)(1 - (Math.Pow(maxCruiseSpeed - airSpeed, 2f) / (maxCruiseSpeed * maxCruiseSpeed)));
		}



		/// <summary>
		/// Show data on the missile in its current state
		/// </summary>
		/// <param name="position">missile position</param>
		/// <param name="rotation">missile rotation in Earth frame</param>
		/// <param name="velocity">missile velocity in Earth frame</param>
		/// <param name="rotVelocity">missile angular velocity, in Body frame</param>
		protected void printDebug(Vector3 position, Vector3 rotation, Vector3 velocity, Vector3 rotVelocity)
		{
			// computations for print out
			Vector3 deltaPosition = position - _spawnPosition;

			string debugStr = "Position: " + deltaPosition.Round().ToString() 
				+ "~n~AirSpeed: " + velocity.Length()
				+ "~n~Rotation: " + rotation.Round().ToString()
				+ "~n~RotSpeed: " + rotVelocity.Round(4).ToString();

			GTA.UI.Screen.ShowHelpTextThisFrame(debugStr);
		}
		#endregion
	}
}
