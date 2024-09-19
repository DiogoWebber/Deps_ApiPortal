using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Deps_CleanArchitecture.SharedKernel.Interfaces
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }

        DateTime CreatedAt { get; }
    }
}
