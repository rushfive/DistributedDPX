using Common.Abstractions;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mongo.Serializers
{
	public class GenericSerializationProvider : BsonSerializationProviderBase
	{
		private static Dictionary<Type, Type> serializers { get; } = new Dictionary<Type, Type>
		{
			{ typeof(OptionalValue<>), typeof(OptionalValueSerializer<>)}
		};

		public override IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry serializerRegistry)
		{
			if (GenericSerializationProvider.serializers.TryGetValue(type, out Type serializer))
			{
				return this.CreateSerializer(serializer, serializerRegistry);
			}

			TypeInfo typeInfo = type.GetTypeInfo();

			if (typeInfo.IsGenericType && !typeInfo.ContainsGenericParameters)
			{
				if (GenericSerializationProvider.serializers.TryGetValue(type.GetGenericTypeDefinition(), out serializer))
				{
					return this.CreateGenericSerializer(serializer, typeInfo.GetGenericArguments(), serializerRegistry);
				}
			}

			return null;
		}
	}
}
