﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;
using GTA.Native;



namespace GFPS
{
	public class HellstormMissile : Missile
	{
		#region properties
		protected const float initialHeight = 100.0f;
		protected const float initialRadius = 10.0f;

		protected Model clusterMissileModel;
		protected float maxCruiseSpeed = 20.0f;
		protected float maxBoostSpeed = 200.0f;

		protected Vector3 cruisingThrustVector = new Vector3(0f, -20f, 0f);
		#endregion



		#region constructorDestructor
		public HellstormMissile()
			: base()
		{}
		#endregion




		#region overridenMethods
		protected override void configure()
		{
			// call parent configure() method
			base.configure();

			// overwrite any configs as necessary
			missileModel = (Model)(-1479625776);
			//clusterMissileModel = (Model)737852268;
			clusterMissileModel = (Model)(-1146260322);
			attachCamera = true;
			explosionType = ExplosionType.Plane;
			invertThrust = true;

			// load particle FX
			particleFxAsset = new ParticleEffectAsset("scr_agencyheistb");
			particleFxAsset.Request();
			particleFxOffset = new Vector3(0f, 2.925f, 0f);
			particleFxName = "scr_agency3b_proj_rpg_trail";
		}


		/// <summary>
		/// Spawn a missile as a prop
		/// </summary>
		protected override void spawnMissile()
		{
			// get an offset Vector3 to spawn the missile at
			Vector3 playerPos = Game.Player.Character.Position;
			Vector3 randomAround = playerPos.Around(initialRadius);
			randomAround.Z += initialHeight;

			// spawn the prop
			missile = World.CreateProp(missileModel, randomAround, true, false);
		}


		/// <summary>
		/// Apply appropriate settings to the missile prop instance
		/// </summary>
		protected override void configureMissileProp()
		{
			// apply settings to the missile
			missile.HasGravity = false;
			missile.MaxSpeed = maxCruiseSpeed;

			// orient the missile towards the player. get normalized delta vector that points towards the player
			Vector3 directionVector = (Game.Player.Character.Position - missile.Position).Normalized;
			missile.Rotation = Helper.getEulerAngles(directionVector, invertThrust);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool control()
		{
			// invoke base control method; if it returns false, stop execution
			if (!base.control())
				return false;

			// apply "thrust" to the missile; recall that the missile has a set maximum speed
			missile.ApplyForceRelative(cruisingThrustVector);

			return true;
		}


		/// <summary>
		/// Request and attach particle effects to the missile
		/// </summary>
		protected override void attachParticleFx()
		{
			ParticleEffect fx = World.CreateParticleEffect(particleFxAsset, particleFxName,
				missile, particleFxOffset, Vector3.Zero, particleFxScale);
		}


		/// <summary>
		/// The Hellfire main missile collision handler. Missile will explode on collision.
		/// </summary>
		protected override bool collisionHandler()
		{
			// get the missile's position and create an explosion at that position
			Vector3 missilePos = missile.Position;
			World.AddExplosion(missilePos, explosionType, explosionDamageScale, explosionCamShake);

			return cleanUp();
		}
		#endregion
	}
}
