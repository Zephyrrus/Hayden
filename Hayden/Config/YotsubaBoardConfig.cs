namespace Hayden.Config
{
    public class YotsubaBoardConfig
    {
        /// <summary>
		/// Regexes used to archive only matching threads. Everything is archived if <see langword="null"/>
		/// </summary>
		public string?[] Whitelist { get; set; }

        public bool ArchivesEnabled { get; set; }
    }
}