using Common.Abstractions;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mongo.Serializers
{
	public class OptionalValueSerializer<TValue> : StructSerializerBase<OptionalValue<TValue>>
	{
		private IBsonSerializer<TValue> valueSerializer { get; }

		public OptionalValueSerializer(IBsonSerializerRegistry serializerRegistry)
		{
			this.valueSerializer = serializerRegistry.GetSerializer<TValue>();
		}

		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, OptionalValue<TValue> value)
		{
			context.Writer.WriteStartDocument();

			context.Writer.WriteName(nameof(value.IsSpecified));
			context.Writer.WriteBoolean(value.IsSpecified);

			context.Writer.WriteName(nameof(value.Value));

			TValue optionalValue = value.IsSpecified ? value.Value : default;

			this.valueSerializer.Serialize(context, args, optionalValue);

			context.Writer.WriteEndDocument();
		}

		public override OptionalValue<TValue> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			context.Reader.ReadStartDocument();

			context.Reader.ReadName(nameof(OptionalValue<TValue>.IsSpecified));
			bool isSpecified = context.Reader.ReadBoolean();

			context.Reader.ReadName(nameof(OptionalValue<TValue>.Value));
			TValue value = this.valueSerializer.Deserialize(context, args);

			context.Reader.ReadEndDocument();

			if (!isSpecified)
			{
				return OptionalValue<TValue>.NotSpecified;
			}

			return OptionalValue<TValue>.WithValue(value);
		}
	}
}
