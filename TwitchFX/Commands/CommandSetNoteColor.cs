using TwitchFX.Colors;
using UnityEngine;

namespace TwitchFX.Commands {
	
	public class CommandSetNoteColor : Command {
		
		public CommandSetNoteColor() {
			
			usage = "<left color> <right color> OR\n" +
			"<left color> <right color> <duration in seconds>";
			
			SetArgsCount(2, 3);
			
		}
		
		protected override void Execute(string[] args) {
			
			Color leftColor = ParseColor(args[0]);
			Color rightColor = ParseColor(args[1]);
			bool rainbow = false;
			if (Helper.IsRainbow(leftColor) || Helper.IsRainbow(rightColor))
			{
				rainbow = true;
			}
			Color diff = leftColor - rightColor;
			float mag = Mathf.Abs(diff.r) + Mathf.Abs(diff.g) + Mathf.Abs(diff.b);
			
			if ( mag < 0.2f && !rainbow)
			{
				Plugin.chat.Send("Someone tried to be funny and set the note colors the same");
				return;
			}

			float? duration = TryParseFloat(args, 2);
			
			ColorController.instance.SetNoteColors(leftColor, rightColor, duration);
			
		}
		
	}
	
}