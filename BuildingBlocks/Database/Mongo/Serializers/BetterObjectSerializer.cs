using Common.Abstractions.Dates;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Mongo.Serializers
{
	public class BetterObjectSerializer : ObjectSerializer
	{
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			if (value != null)
			{
				TypeInfo actualType = value.GetType().GetTypeInfo();
				if (actualType.IsGenericType)
				{
					if (actualType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
					{
						context.Writer.WriteStartDocument();
						foreach (DictionaryEntry v in (IDictionary)value)
						{
							context.Writer.WriteName(v.Key.ToString());
							this.Serialize(context, args, v.Value);
						}
						context.Writer.WriteEndDocument();
						return;
					}

					if (actualType.GetGenericTypeDefinition() == typeof(List<>))
					{
						context.Writer.WriteStartArray();
						foreach (object v in (IEnumerable)value)
						{
							this.Serialize(context, args, v);
						}
						context.Writer.WriteEndArray();
						return;
					}
				}
				else
				{
					switch (value)
					{
						case decimal dec:
							new DecimalSerializer(MongoDB.Bson.BsonType.Decimal128).Serialize(context, args, dec);
							return;
						case DsDate dsDate:
							this.Serialize(context, args, new { _t = "Date", _v = dsDate.Serialize() });
							return;
						case DsTime dsTime:
							this.Serialize(context, args, new { _t = "Time", _v = dsTime.Serialize() });
							return;
						case CultureInfo ci:
							this.Serialize(context, args, ci.Name);
							return;
					}
				}
			}
			base.Serialize(context, args, value);
		}

		public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			switch (context.Reader.GetCurrentBsonType())
			{
				case MongoDB.Bson.BsonType.Decimal128:
					return (decimal)context.Reader.ReadDecimal128();
				default:
					return base.Deserialize(context, args);
			}
		}
	}
}
