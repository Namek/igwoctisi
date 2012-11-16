﻿namespace Client.Model
{
	using System.Runtime.Serialization;
	using System.Collections.Generic;

	[DataContract]
	class EndgameData
	{
		[DataMember]
		public int Rounds { get; set; }

		[DataMember]
		public int Time { get; set; }

		[DataMember]
		public string EndType { get; set; }

		[DataMember]
		public List<string> Players { get; set; }

		[DataMember]
		public List<GameStatistic> Statistics { get; set; }
	}
}