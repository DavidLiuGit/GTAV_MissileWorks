using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;



namespace GFPS
{
	public class Missile
	{
		#region properties
		// config
		protected Model missileModel;
		protected bool canControl;
		protected bool attachCamera;

		// instance references & pointers
		protected Prop missile;				// Prop is a child of Entity

		// particle fx
		protected Vector3 particleFxOffset;
		protected List<ParticleEffect> particleFxList;
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
		#endregion




		#region protected
		/// <summary>
		/// Apply configurations
		/// </summary>
		protected virtual void configure()
		{
			missileModel = (Model)737852268;
			canControl = false;
			attachCamera = false;
		}


		/// <summary>
		/// Spawn the missile as a prop
		/// </summary>
		protected virtual void spawnMissile () {}


		/// <summary>
		/// Apply appropriate settings to the missile prop instance
		/// </summary>
		protected virtual void configureMissileProp() { }


		/// <summary>
		/// Attach particle effects to the missile prop
		/// </summary>
		protected virtual void attachParticleFx() { }

		#endregion
	}
}
