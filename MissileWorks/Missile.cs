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
		protected Model missileModel;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Missile()
		{
			configure();
			spawnMissile();
			configureMissileProp();
		}



		#region protected
		protected virtual void configure()
		{
			missileModel = (Model)737852268;
		}


		/// <summary>
		/// Spawn the missile as a prop
		/// </summary>
		protected virtual void spawnMissile () {}


		/// <summary>
		/// Apply settings to the prop of the missile
		/// </summary>
		protected virtual void configureMissileProp() { }

		#endregion
	}
}
