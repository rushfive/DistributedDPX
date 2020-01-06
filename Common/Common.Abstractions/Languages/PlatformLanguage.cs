using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Abstractions.Languages
{
	public class PlatformLanguage
	{
		public PlatformLanguage(string code, string label)
		{
			this.Code = code;
			this.Label = label;
		}

		public string Code { get; set; }

		public string Label { get; set; }
	}
}
