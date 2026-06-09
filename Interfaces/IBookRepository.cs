using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Models;


namespace library_system.Interfaces
{
    public interface IBookRepository : IRepository<Book> // motmaeen show <Book> ro bezari man nazashete boodam ye nim saat alaf shode bodam
    {
        
    }
}