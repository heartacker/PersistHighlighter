using System;
using System.Collections.Generic;

namespace StickyHighlighterClassifier
{
	// Token: 0x0200000B RID: 11
	public class HighlightedWordCollection
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002890 File Offset: 0x00000A90
		private HighlightedWordCollection()
		{
			this.HighlightedWords = new List<string>();
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000028A3 File Offset: 0x00000AA3
		public static HighlightedWordCollection Instance
		{
			get
			{
				if (HighlightedWordCollection.instance == null)
				{
					HighlightedWordCollection.instance = new HighlightedWordCollection();
				}
				return HighlightedWordCollection.instance;
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000028BC File Offset: 0x00000ABC
		public void ToggleHighlightWord(string word)
		{
			foreach (string text in this.HighlightedWords)
			{
				if (text == word)
				{
					this.HighlightedWords.Remove(text);
					this.OnChanged(EventArgs.Empty);
					return;
				}
			}
			this.HighlightedWords.Add(word);
			this.OnChanged(EventArgs.Empty);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002944 File Offset: 0x00000B44
		public void ClearHighlightedWords()
		{
			this.HighlightedWords.Clear();
			this.OnChanged(EventArgs.Empty);
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000024 RID: 36 RVA: 0x0000295C File Offset: 0x00000B5C
		// (remove) Token: 0x06000025 RID: 37 RVA: 0x00002994 File Offset: 0x00000B94
		public event HighlightedWordCollection.ChangedEventHandler Changed;

		// Token: 0x06000026 RID: 38 RVA: 0x000029C9 File Offset: 0x00000BC9
		protected virtual void OnChanged(EventArgs e)
		{
			if (this.Changed != null)
			{
				this.Changed(this, e);
			}
		}

		// Token: 0x0400000A RID: 10
		private static HighlightedWordCollection instance;

		// Token: 0x0400000B RID: 11
		public List<string> HighlightedWords;

		// Token: 0x0200000C RID: 12
		// (Invoke) Token: 0x06000028 RID: 40
		public delegate void ChangedEventHandler(object sender, EventArgs e);
	}
}
