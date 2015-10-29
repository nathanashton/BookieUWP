﻿using Bookie.Common.Model;
using System;
using System.Collections.Generic;

namespace Bookie.Domain.Interfaces
{
    public interface IBookService
    {
        List<Book> Find(Func<Book, bool> where);

        List<Book> GetByTitle(string title);

        Book GetById(int id);

        List<Book> GetAll();

        Book Add(Book book);

        void Update(Book book);
    }
}