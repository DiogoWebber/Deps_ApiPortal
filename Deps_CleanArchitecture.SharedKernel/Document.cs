using Deps_CleanArchitecture.SharedKernel.Interfaces;
using MongoDB.Bson;
using System;

namespace Deps_CleanArchitecture.SharedKernel
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
