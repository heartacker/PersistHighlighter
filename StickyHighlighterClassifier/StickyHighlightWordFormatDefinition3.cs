using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StickyHighlighterClassifier
{
	// Token: 0x02000008 RID: 8
	[Export(typeof(EditorFormatDefinition))]
	[Name("MarkerFormatDefinition/HighlightWordFormatDefinition3")]
	[UserVisible(true)]
	internal class StickyHighlightWordFormatDefinition3 : BaseStickyHighlightWordFormatDefinition
	{
		// Token: 0x06000007 RID: 7 RVA: 0x000021CC File Offset: 0x000003CC
		public StickyHighlightWordFormatDefinition3()
		{
			base.BackgroundColor = new Color?(Color.FromRgb(byte.MaxValue, 182, 0));
			base.ForegroundColor = new Color?(Color.FromRgb(198, 142, 0));
			base.DisplayName = "Highlighter #3";
		}
	}
}
