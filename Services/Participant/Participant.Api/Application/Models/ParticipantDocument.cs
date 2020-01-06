using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Participant.API.Application.Models
{
	public class ParticipantDocument
	{
		[BsonId]
		public Guid Id { get; set; }

		public bool Enabled { get; set; }

		public Dictionary<string, object> FieldValues { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		public bool Locked { get; set; }

		public string SiteId { get; set; }

		public string Language { get; set; }

		public string TimeZoneId { get; set; }

		public bool IsVerified { get; set; }
	}
}
