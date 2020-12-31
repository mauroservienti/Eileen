using System;

namespace Eileen.Data
{
    public interface IEntity
    {
        DateTimeOffset CreatedOn { get; set; }
        DateTimeOffset LastModifiedOn { get; set; }
    }
}