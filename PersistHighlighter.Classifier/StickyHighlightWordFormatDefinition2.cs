using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace PersistHighlighter.Classifier
{
	// Token: 0x02000007 RID: 7
	[Name("MarkerFormatDefinition/HighlightWordFormatDefinition2")]
	[UserVisible(true)]
	[Export(typeof(EditorFormatDefinition))]
	internal class StickyHighlightWordFormatDefinition2 : BaseStickyHighlightWordFormatDefinition
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002178 File Offset: 0x00000378
		public StickyHighlightWordFormatDefinition2()
		{
			base.BackgroundColor = new Color?(Color.FromRgb(byte.MaxValue, 219, 0));
			base.ForegroundColor = new Color?(Color.FromRgb(198, 169, 0));
			base.DisplayName = "Highlighter #2";
		}
	}
}
