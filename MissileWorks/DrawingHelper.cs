using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;

namespace GFPS
{
	public class DrawingHelper
	{
		#region defaults
		public static SizeF defaultSizeF = new SizeF(48f, 48f);
		public static Color defaultColor = Color.White;
		#endregion



		#region drawing
		/// <summary>
		/// Draw a texture around specified entityList
		/// </summary>
		/// <param name="entityList">List of <c>entity</c> instances to mark</param>
		/// <param name="spr">Instance of <c>Sprite</c> to mark entityList with</param>
		/// <param name="distinguishPedRelationship">Change color based on the <c>Ped</c>'s relationship with the player</param>
		public static void markPedsOnScreen(List<Entity> entityList, Sprite spr, bool distinguishPedRelationship = true)
		{
			foreach (Entity entity in entityList)
			{
				if (!distinguishPedRelationship)
					markEntityOnScreen(entity, spr);

			}
		}



		/// <summary>
		/// Draw a preloaded <c>Sprite</c> on the screen where a specified entity is
		/// </summary>
		/// <param name="entity">Instance of <c>Entity</c> to mark</param>
		/// <param name="spr">Preloaded <c>Sprite</c> to mark the entity with</param>
		/// <param name="color">Force use of specified <c>Color</c></param>
		public static void markEntityOnScreen(Entity entity, Sprite spr, Color? color = null)
		{
			// get the position of the entity on screen & update the screen position of the sprite
			PointF screenPos = Screen.WorldToScreen(entity.Position);
			spr.Position = screenPos;			// update screen position of sprite
			spr.Color = color ?? spr.Color;		// update sprite color if needed; if null, keep original color
			spr.Draw();
		}



		/// <summary>
		/// Draw a preloaded <c>CustomSprite</c> on the screen where a specified entity is.
		/// </summary>
		/// <param name="entity">Instance of <c>Entity</c> to mark</param>
		/// <param name="spr">Preloaded <c>CustomSprite</c> to mark the entity with</param>
		/// <param name="color">Force use of specified <c>Color</c></param>
		public static void markEntityOnScreen(Entity entity, CustomSprite spr, Color? color = null)
		{
			// get the position of the entity on screen & update the screen position of the sprite
			PointF screenPos = Screen.WorldToScreen(entity.Position);
			spr.Position = screenPos;			// update screen position of sprite
			spr.Color = color ?? spr.Color;		// update sprite color if needed; if null, keep original color
			spr.Draw();
		}


		#endregion
	}
}
