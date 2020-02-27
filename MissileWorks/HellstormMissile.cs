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
		protected float initialHeight = 50.0f;
		protected float initialRadius = 15.0f;

		protected Model clusterMissileModel;
		protected float maxCruiseSpeed = 50.0f;
		protected float maxBoostSpeed = 100.0f;
		#endregion



		#region constructorDestructor
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
			
			// orient the missile towards the player
			Vector3 directionVector = (Game.Player.Character.Position - missile.Position).Normalized;			// get the normalized delta vector; points towards the player
			missile.Rotation = Helper.getEulerAngles(directionVector);

			// apply "thrust" to the missile
			missile.ApplyForceRelative(new Vector3(0f, 100f, 0f));


			missile.MarkAsNoLongerNeeded();
		}
		#endregion
	}
}
