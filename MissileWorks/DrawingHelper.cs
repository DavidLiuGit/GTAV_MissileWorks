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



		/// <summary>
		/// Determine the <c>Color</c> to mark an entity with, depending on its <c>TargetType</c>
		/// </summary>
		/// <param name="tt"><c>TargetType</c></param>
		/// <returns><c>Color</c></returns>
		public static Color getColorFromTargetType(TargetType tt)
		{
			switch (tt)
			{
				case TargetType.Player:
				case TargetType.Friendly:
					return Color.Green;

				case TargetType.Hostile:
					return Color.OrangeRed;

				case TargetType.Untargetable:
					return Color.Black;

				case TargetType.Neutral:
					return Color.FromArgb(120, Color.White);

				default:
					return Color.Transparent;
			}
		}
		#endregion
	}
}
