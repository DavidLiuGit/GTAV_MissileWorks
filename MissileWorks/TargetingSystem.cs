using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GTA;


namespace GFPS
{
	public class TargetingSystem
	{
		#region properties
		protected Ped _player;
		protected RelationshipGroup _playerRg;
		protected float minTargetableDist;
		protected float maxTargetableDist;
		#endregion



		#region constructor
		public TargetingSystem(Ped player)
		{
			_player = player;
			_playerRg = player.RelationshipGroup;
		}
		#endregion



		#region publicMethods
		/// <summary>
		/// Determine the <c>TargetType</c> of the specified Ped
		/// </summary>
		/// <param name="p">instance of <c>Ped</c></param>
		/// <returns><c>TargetType</c></returns>
		public TargetType getPedTargetType(Ped p)
		{
			// check if ped is player
			if (p == _player)
				return TargetType.Player;

			// check if dead
			if (p.IsDead)
				return TargetType.Dead;

			// get the player's relationship with the Ped
			Relationship rel = _player.GetRelationshipWithPed(p);
			switch (rel)
			{
				case Relationship.Companion:
				case Relationship.Like:
				case Relationship.Respect:
					return TargetType.Friendly;

				case Relationship.Hate:
				case Relationship.Dislike:
					return TargetType.Hostile;
			}

			// check if ped is shooting
			if (p.IsInCombat)
				return TargetType.Hostile;

			// if none of the above criteria fit, return Neutral
			return TargetType.Neutral;
		}
		#endregion
	}


	public enum TargetType
	{
		Player = 0,
		Friendly = 1,
		Hostile = 2,
		Neutral = 3,
		Untargetable = 4,
		Dead = 7
	}
}
