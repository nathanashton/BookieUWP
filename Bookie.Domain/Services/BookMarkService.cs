using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookie.Domain.Services
{
    public class BookMarkService : IBookMarkService
    {
        private readonly IBookMarkRepository _repository;

        public BookMarkService(IBookMarkRepository repository)
        {
            _repository = repository;
        }

        public List<BookMark> Find(Func<BookMark, bool> @where)
        {
            return _repository.Find(where).ToList();
        }

        public BookMark GetById(int id)
        {
            return _repository.Find(x=> x.Id == id).FirstOrDefault();
        }

        public List<BookMark> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public List<BookMark> GetAllForBook(Book book)
        {
            return _repository.Find(x => x.Book.Id == book.Id).ToList();
        }

        public BookMark Add(BookMark book)
        {
            return _repository.Add(book);
        }

        public void Update(BookMark book)
        {
            _repository.Update(book);
        }

        public void Remove(BookMark bookmark)
        {
            _repository.Remove(bookmark);
        }
    }
}