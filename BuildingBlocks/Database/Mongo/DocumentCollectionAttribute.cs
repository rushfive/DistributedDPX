using System;
using System.Collections.Generic;
using System.Text;

namespace Mongo
{
	public class DocumentCollectionAttribute : Attribute
	{
		public string Name { get; }

		public DocumentCollectionAttribute(string name)
		{
			this.Name = name;
		}
	}
}
