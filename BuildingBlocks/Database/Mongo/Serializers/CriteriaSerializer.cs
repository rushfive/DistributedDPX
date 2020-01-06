//using DatStat.Internal.Abstractions.Models;
//using DatStat.Internal.Abstractions.Reporting;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization;
//using MongoDB.Bson.Serialization.Serializers;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Mongo.Serializers
//{
//	public class CriteriaSerializer : CriteriaBaseSerializer<Criteria, Condition>
//	{
//		protected override Criteria Create(List<Condition> conditions, EvaluationType evaluationType, string customExpression)
//		{
//			return Criteria.CreateOrDefault(conditions, evaluationType, customExpression);
//		}
//	}

//	public abstract class CriteriaBaseSerializer<TCriteria, TCondition> : SerializerBase<TCriteria>
//		where TCriteria : CriteriaBase<TCondition>
//		where TCondition : ICondition
//	{
//		protected abstract TCriteria Create(List<TCondition> conditions, EvaluationType evaluationType, string customExpression);

//		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TCriteria value)
//		{
//			if (value == null || value.Conditions.IsNullOrEmpty())
//			{
//				context.Writer.WriteNull();
//				return;
//			}
//			context.Writer.WriteStartDocument();

//			context.Writer.WriteName("Conditions");
//			context.Writer.WriteStartArray();
//			foreach (TCondition condition in value.Conditions)
//			{
//				BsonSerializer.Serialize(context.Writer, condition, args: args);
//			}
//			context.Writer.WriteEndArray();

//			context.Writer.WriteName("EvaluationType");
//			context.Writer.WriteString(value.EvaluationType.ToString());

//			context.Writer.WriteName("CustomExpression");

//			if (string.IsNullOrWhiteSpace(value.CustomExpression))
//			{
//				context.Writer.WriteNull();
//			}
//			else
//			{
//				context.Writer.WriteString(value.CustomExpression);
//			}

//			context.Writer.WriteEndDocument();
//		}

//		public override TCriteria Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
//		{
//			if (context.Reader.CurrentBsonType == BsonType.Null)
//			{
//				context.Reader.ReadNull();
//				return null;
//			}
//			var doc = BsonSerializer.Deserialize<BsonDocument>(context.Reader);

//			List<TCondition> conditions = doc["Conditions"].AsBsonArray
//				.Select(c => BsonSerializer.Deserialize<TCondition>(c.AsBsonDocument))
//				.ToList();

//			if (!conditions.Any())
//			{
//				return null;
//			}
//			string typeString = doc["EvaluationType"].IsBsonNull ? null : doc["EvaluationType"].AsString;
//			var evaluationType = (EvaluationType)Enum.Parse(typeof(EvaluationType), typeString);
//			string customExpression = doc["CustomExpression"].IsBsonNull ? null : doc["CustomExpression"].AsString;

//			return this.Create(conditions, evaluationType, customExpression);
//		}
//	}
//}
