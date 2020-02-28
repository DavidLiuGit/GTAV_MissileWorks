using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;
using GTA.Native;


namespace GFPS
{
	class Helper
	{
		static Random rng = new Random();

		/// <summary>
		/// Generate an offset Vector3. Offset by specified height, and at a random point in a circle, defined by <c>haloRadius</c>
		/// </summary>
		/// <param name="height">height, in meters</param>
		/// <param name="haloRadius">radius of circle to pick a point from</param>
		/// <returns></returns>
		public static Vector3 getOffsetVector3(float height, float haloRadius = 0.0f)
		{
			float x = 0.0f, y = 0.0f;

			// randomly generate the magnitude of x & y
			if (haloRadius > 0.0f)
			{
				double x2 = rng.NextDouble() * haloRadius * haloRadius;			// 0.0 < x^2 < haloRadius^2
				double y2 = Math.Pow(haloRadius, 2.0f) - x2;						// y^2 = haloRadius^2 - x^2
				x = (float)Math.Sqrt(x2);
				y = (float)Math.Sqrt(y2);
			}

			// randomly determine the signs of x & y, based on quadrant
			int quadrant = rng.Next(4);
			switch (quadrant)
			{
				case 0: return new Vector3(x, y, height);
				case 1: return new Vector3(x, -y, height);
				case 2: return new Vector3(-x, y, height);
				case 3: default: return new Vector3(-x, -y, height);		// add default so all code-paths have return value
			}
			
		}



		/// <summary>
		/// Given a target position and a radius, return a random coordinate on the edge of the circle.
		/// </summary>
		/// <param name="radius">Radius of the circle, in meters</param>
		/// <param name="playerPos"><c>Vector3</c> representing player's position</param>
		/// <returns></returns>
		public static Vector3 getVector3NearTarget(float radius, Vector3 targetPos)
		{
			return targetPos + getOffsetVector3(0.0f, radius);
		}



		public static void makeRelationshipGroupHate(RelationshipGroup rg, uint[] hateGroupHashes)
		{
			foreach (uint group in hateGroupHashes)
				rg.SetRelationshipBetweenGroups((RelationshipGroup)group, Relationship.Hate, false);
		}
		public readonly static uint[] defaultHateGroups = new uint[] {
			0xA49E591C,		// cops
			0xF50B51B7,		// rent-a-cops
			0x4325F88A, 0x11DE95FC, 0x8DC30DC3,		// gangs
			0x90C7DA60, 0x11A9A7E3, 0x45897C40, 0xC26D562A, 0x7972FFBD, 0x783E3868, 0x936E7EFB, 0x6A3B9F86, 0xB3598E9C,	// ambient gangs
			0x7EA26372,		// prisoners
			0x8296713E,		// dealers
		};



		/// <summary>
		/// Given a normalized direction Vector3, return a set of Euler angles, describing Rot[ZXY] (i.e. pitch, roll, yaw).
		/// All angles are in degrees. Roll will be set to 0.0
		/// </summary>
		/// <param name="unitDirectionVector">Normalized direction <c>Vector3</c></param>
		/// <returns><c>Vector3</c> whose x, y, z angles represent pitch, roll, and yaw angles respectively. Angles are in degrees.</returns>
		public static Vector3 getEulerAngles(Vector3 normDirectionVector, bool invertPitch = false)
		{
			// calculate angles
			float yaw = (float)(Math.Atan2(normDirectionVector.Y, normDirectionVector.X) * 180 / Math.PI) - 90f;
			float pitch = (float)(Math.Asin(normDirectionVector.Z) * (180 / Math.PI));

			// if any angles need to be inverted, do so
			if (invertPitch) pitch = invertAngleDegrees(pitch);

			return new Vector3(pitch, 0f, yaw);
		}


		/// <summary>
		/// Flip an angle 180 degrees. Output is constrained to -180.0 < output <= +180.0
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static float invertAngleDegrees(float angle)
		{
			// invert by adding 180 degrees
			angle += 180.0f;

			// normalize if needed
			return (angle % 360f + 360f) % 360f;
		}


		/// <summary>
		/// Request 
		/// </summary>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public static ParticleEffectAsset loadParticleFxAsset(string assetName)
		{
			ParticleEffectAsset asset = new ParticleEffectAsset(assetName);
			if (!asset.IsLoaded) asset.Request();
			return asset;
		}



		#region NativeAPI
		/// <summary>
		/// Apply a force to an entity at its center of gravity (mass). Resulting acceleration depends on object mass and force applied.
		/// </summary>
		/// <param name="ent">instance of <c>Entity</c></param>
		/// <param name="force"><c>Vector3</c> describing the force</param>
		/// <param name="relative">if true, the entity's coordinate axis is used. Otherwise, the global coord axis is used</param>
		/// <param name="ftype"><c>ForceType</c></param>
		public static void ApplyForceToCoG(Entity ent, Vector3 force, bool relative = false, ForceType ftype = ForceType.MaxForceRot2){
			 Function.Call(Hash.APPLY_FORCE_TO_ENTITY_CENTER_OF_MASS, ent, 1,
				 force.X, force.Y, force.Z, false, relative, true, false);
		}

		#endregion
	}
}
