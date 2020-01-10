using System;
using System.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace StickyHighlighterClassifier
{
	// Token: 0x0200000A RID: 10
	[ContentType("text")]
	[TagType(typeof(TextMarkerTag))]
	[Export(typeof(IViewTaggerProvider))]
	internal class HighlightWordTaggerProvider : IViewTaggerProvider
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001A RID: 26 RVA: 0x0000282C File Offset: 0x00000A2C
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002834 File Offset: 0x00000A34
		[Import]
		internal ITextSearchService TextSearchService { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001C RID: 28 RVA: 0x0000283D File Offset: 0x00000A3D
		// (set) Token: 0x0600001D RID: 29 RVA: 0x00002845 File Offset: 0x00000A45
		[Import]
		internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

		// Token: 0x0600001E RID: 30 RVA: 0x00002850 File Offset: 0x00000A50
		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			if (textView.TextBuffer != buffer)
			{
				return null;
			}
			ITextStructureNavigator textStructureNavigator = this.TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);
			return new StickyHighlightWordTagger(textView, buffer, this.TextSearchService, textStructureNavigator) as ITagger<T>;
		}
	}
}
