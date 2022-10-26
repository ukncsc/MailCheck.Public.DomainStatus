using System;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MailCheck.DomainStatus.Contracts;

namespace MailCheck.DomainStatus.Common.Dao
{
    public class StatusConverter : IPropertyConverter
    {
        public DynamoDBEntry ToEntry(object value)
        {
            if (!(value is Status)) throw new ArgumentOutOfRangeException();

            DynamoDBEntry entry = new Primitive
            {
                Value = value.ToString()
            };

            return entry;
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            Primitive primitive = entry as Primitive;
            if (!(primitive?.Value is string) || string.IsNullOrEmpty((string)primitive.Value))
                throw new ArgumentOutOfRangeException();

            return Enum.Parse<Status>((string)primitive.Value);
        }
    }
}