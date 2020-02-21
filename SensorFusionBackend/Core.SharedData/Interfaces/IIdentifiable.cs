using System;

namespace Core.Common.Interfaces
{
    public interface IIdentifiable
    {
        Guid Id { get; }
    }
}