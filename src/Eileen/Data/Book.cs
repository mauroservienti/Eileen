﻿using System;

namespace Eileen.Data
{
    public class Book : IEntity
    {
        public int Id { get; set; }
        public int? AuthorId { get; set; }
        public Author Author { get; set; }
        public int? PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public string Title { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
    }
}