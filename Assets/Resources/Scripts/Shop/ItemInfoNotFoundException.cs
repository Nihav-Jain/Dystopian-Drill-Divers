using UnityEngine;
using System;

namespace Shop
{
	public class ItemInfoNotFoundException : Exception
	{
		public ItemInfoNotFoundException()
		{
		}

		public ItemInfoNotFoundException(string message)
			: base(message)
		{
		}

		public ItemInfoNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
