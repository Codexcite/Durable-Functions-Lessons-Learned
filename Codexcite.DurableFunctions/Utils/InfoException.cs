using System;
using System.Collections.Generic;
using System.Text;

namespace Codexcite.DurableFunctions.Utils
{
	public class InfoException : Exception
	{
		public InfoException(int data, string message) : base(message)
		{
			Info = data;
		}

		public int Info { get; set; }

	}
}
