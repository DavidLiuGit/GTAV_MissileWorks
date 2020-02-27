using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;



namespace GFPS
{
	public abstract class Missile
	{
		#region properties
		// config
		public bool active;
		protected Model missileModel;
		protected bool canControl;
		protected bool attachCamera = false;
		public bool autonomous;

		// instance references & pointers
		protected Prop missile;				// Prop is a child of Entity

		// particle fx
		protected ParticleEffectAsset particleFxAsset;
		protected Vector3 particleFxOffset = new Vector3(0f, -0.35f, 0f);
		protected float particleFxScale = 2.0f;
		protected string particleFxName;

		// explosion
		protected float explosionDamageScale = 1.0f;
		protected float explosionCamShake = 2.0f;
		protected ExplosionType explosionType = ExplosionType.PlaneRocket;
		#endregion



		#region constructorDestructor
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Missile()
		{
			configure();
			spawnMissile();
			configureMissileProp();
			attachParticleFx();
		}

		
		/// <summary>
		/// Cleans up any assets that were put into the world. Can only be called in main thread.
		/// </summary>
		/// <returns>Always returns false.</returns>
		public bool cleanUp () {
			try
			{
				missile.Delete();
			}
			catch { }
			finally
			{
				active = false;
			}
			return false;
		}
		#endregion




		#region virtualMethods
		/// <summary>
		/// Apply configurations
		/// </summary>
		protected virtual void configure()
		{
			active = true;
			missileModel = (Model)737852268;
			canControl = false;
			attachCamera = false;
			//_autonomous = false;
		}


		/// <summary>
		/// Control the missile. This method is meant to be invoked on each Script tick after the missile is activated.
		/// If the missile is no longer active for any reason, this object instance is marked as such, and control execution is stopped.
		/// </summary>
		/// <returns>Whether the control flow can proceed</returns>
		public virtual bool control()
		{
			// if the prop does not exist, stop execution flow
			if (!missile.Exists())
				return cleanUp();

			// if the missile has collided with something, call collisionHandler
			if (missile.HasCollided || missile.HasBeenDamagedByAnyWeapon())
				if (!collisionHandler())
					return false;				// stop execution if collisionHandler returns false

			return true;
		}
		#endregion




		#region abstractMethods
		/// <summary>
		/// Spawn the missile as a prop
		/// </summary>
		protected abstract void spawnMissile();


		/// <summary>
		/// Apply appropriate settings to the missile prop instance
		/// </summary>
		protected abstract void configureMissileProp();


		/// <summary>
		/// Attach particle effects to the missile prop
		/// </summary>
		protected abstract void attachParticleFx();


		/// <summary>
		/// Invoked when a collision is detected
		/// </summary>
		/// <returns>Whether the control flow can proceed</returns>
		protected abstract bool collisionHandler();
		#endregion




		#region accessorsMutators
		#endregion
	}
}
