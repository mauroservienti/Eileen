using System;
using System.Collections.Generic;

namespace Eileen.Data
{
    public class Publisher : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
    }
}