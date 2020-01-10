using System;
using System.Composition;
using System.Drawing;
using System.Window.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StickyHighlighterClassifier
{
	// Token: 0x02000006 RID: 6
	[UserVisible(true)]
	[Export(typeof(EditorFormatDefinition))]
	[Name("MarkerFormatDefinition/HighlightWordFormatDefinition1")]
	internal class StickyHighlightWordFormatDefinition1 : BaseStickyHighlightWordFormatDefinition
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002120 File Offset: 0x00000320
		public StickyHighlightWordFormatDefinition1()
		{
			base.BackgroundColor = new Color?(Color.FromArgb(byte.MaxValue, byte.MaxValue, 49));
			base.ForegroundColor = new Color?(Color.FromArgb(198, 198, 71));
			base.DisplayName = "Highlighter #1";
		}
	}
}
