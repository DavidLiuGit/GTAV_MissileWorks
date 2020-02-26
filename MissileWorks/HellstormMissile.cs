using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;



namespace GFPS
{
	public class HellstormMissile : Missile
	{
		#region properties
		protected float initialHeight = 10.0f;
		protected float initialRadius = 5.0f;

		protected Model clusterMissileModel;
		protected Prop missile;
		#endregion



		#region constructor
		public HellstormMissile()
			: base()
		{
			missileModel = (Model)737852268;
			clusterMissileModel = (Model)(-1146260322);

			//spawnMissile();
		}
		#endregion




		#region public
		protected override void configure()
		{
			missileModel = (Model)737852268;
			clusterMissileModel = (Model)(-1146260322);
		}


		/// <summary>
		/// Spawn a missile as a prop
		/// </summary>
		protected override void spawnMissile()
		{
			// get an offset Vector3 to spawn the missile at
			Vector3 offset = Helper.getOffsetVector3(initialHeight, initialRadius);

			// spawn the prop
			missile = World.CreateProp(missileModel, Game.Player.Character.Position + offset, new Vector3(0f, 0f, 45f), true, false);
		}


		/// <summary>
		/// 
		/// </summary>
		protected override void configureMissileProp()
		{
			missile.HasGravity = false;

			//missile.ApplyForceRelative(new Vector3(0f, 50f, 0f), new Vector3(50f, 0f, 0f));
			missile.Velocity = new Vector3(0f, 10f, 0f);
		}
		#endregion
	}
}
