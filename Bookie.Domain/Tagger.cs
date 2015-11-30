﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Bookie.Domain.Services;

namespace Bookie.Domain
{
    public class Tagger
    {
        private readonly BookService _bookDomain;

        private readonly string[] _removedwords =
        {
            "edition", "for", "and", "with", "in", "the", "of", "to", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            "by", "1st", "2nd", "3rd", "4th", "5th", "6th", " "
        };

        public Tagger(IBookRepository bookRepository)
        {
            _bookDomain = new BookService(bookRepository);
        }

        private readonly List<string> words = new List<string>();

        public async Task<List<TagResult>> Go()
        {
            var allBooks = await _bookDomain.GetAllAsync();
            foreach (var book in allBooks)
            {
                var exploded = book.Title.Split(' ');
                foreach (var s in exploded)
                {
                    if (!_removedwords.Contains(s.ToLower()))
                    {
                        words.Add(s);
                    }
                }
            }
            var grouped = words.GroupBy(s => s).Select(group => new TagResult{Word = group.Key, Count = group.Count()}).ToList();
            grouped.Sort((a, b) => b.Count - a.Count);
            return grouped;
        }
    }
}