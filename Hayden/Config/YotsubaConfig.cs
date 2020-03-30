﻿using System.Collections.Generic;

namespace Hayden.Config
{
	/// <summary>
	/// Configuration object for the 4chan API
	/// </summary>
	public class YotsubaConfig
	{
		/// <summary>
		/// An array of boards to be archived.
		/// </summary>
		public IDictionary<string, YotsubaBoardConfig> Boards { get; set; }

		/// <summary>
		/// The minimum amount of time (in seconds) that should be waited in-between API calls. Defaults to 1.0 seconds if <see langword="null"/>.
		/// </summary>
		public double? ApiDelay { get; set; }

		/// <summary>
		/// The minimum amount of time (in seconds) that should be waited in-between board scrapes. Defaults to 30.0 seconds if <see langword="null"/>.
		/// </summary>
		public double? BoardDelay { get; set; }
	}
}