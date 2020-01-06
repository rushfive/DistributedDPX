using System;
using System.Collections.Generic;
using System.Text;
using Common.Abstractions.Dates;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Serializers
{
	public class DsDateSerializer : StructSerializerBase<DsDate>
	{
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DsDate value)
		{
			context.Writer.WriteInt64(value.Serialize());
		}

		public override DsDate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			if (context.Reader.CurrentBsonType == MongoDB.Bson.BsonType.String)
			{
				return DsDate.Parse(context.Reader.ReadString(), canDeserializeNumber: true);
			}
			else
			{
				return DsDate.Deserialize(context.Reader.ReadInt64());
			}
		}
	}

	public class DsTimeSerializer : StructSerializerBase<DsTime>
	{
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DsTime value)
		{
			context.Writer.WriteInt64(value.Serialize());
		}

		public override DsTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			if (context.Reader.CurrentBsonType == MongoDB.Bson.BsonType.String)
			{
				return DsTime.Parse(context.Reader.ReadString(), canDeserializeNumber: true);
			}
			else
			{
				return DsTime.Deserialize(context.Reader.ReadInt64());
			}
		}
	}

	public class DsDateTimeSerializer : StructSerializerBase<DsDateTime>
	{
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DsDateTime value)
		{
			context.Writer.WriteInt64(value.Serialize());
		}

		public override DsDateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			if (context.Reader.CurrentBsonType == MongoDB.Bson.BsonType.String)
			{
				return DsDateTime.Parse(context.Reader.ReadString(), canDeserializeNumber: true);
			}
			else
			{
				return DsDateTime.Deserialize(context.Reader.ReadInt64());
			}
		}
	}
}
