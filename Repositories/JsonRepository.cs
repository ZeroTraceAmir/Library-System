using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library_system.Interfaces;

namespace library_system.Repositories
{
    public class JsonRepository<T> : IRepository<T>
        where T : IEntity // faghat method haye IRepo ro dare va faghat roye model hayi kar mikone ke Ientity bashan. yani id dashte bashan.
    {
        private readonly JsonDataStore dataStore; //gharare az method haye JsonDataStore estefade konim.
        private readonly string filePath;

        public JsonRepository(JsonDataStore dataStore, string filePath)
        {
            this.dataStore = dataStore;
            this.filePath = filePath;
        }

        public List<T> GetAll() //tamame data haro az file json ma mikhone, va list ro barmigardone.
        {
            List<T> items = dataStore.Load<T>(filePath);
            return items;
        }

        public T? GetById(int id)
        {
            List<T>? items = GetAll();
            T item = items.FirstOrDefault(x => x.Id == id);
            return item;
        } // mesle getall hame data haro migire, baad Itemi ke Id barabar ba id (vorodei karbar) dashte bashe barash migardone. ? beraye ine ke agar peyda nakard, null bargardone va error nade. FirstOrDefault ye method built in toye LINQ hast, nimikhastam azash estefade konam, vali agar nemikardim algorithm search ro khodemon bayad minevishtim ke kheyli sakht mishod, baraye hamin azash estefade kardam, TA ha ham goftan ke moshkeli nadare (toye goroh goftan)

        public void Add(T entity)
        {
            List<T> items = GetAll();
            items.Add(entity);
            dataStore.Save(filePath, items);
        }

        public void Update(T entity)
        {
            List<T> items = GetAll();
            int index = items.FindIndex(x => x.Id == entity.Id);

            if (index != -1)
            {
                items[index] = entity;
                dataStore.Save(filePath, items);
            } // if ro baraye in gozashtam ke AGAR PEYDA kard ba noskhe jadid jaygozin kone, agar nemizashtim va linq id ro peyda nemikard, -1 barmigardon, ke items[index] mishode items[-1] chon hamchin chizi vojod nadare error ArgumentOutOfRangeException midad.
        }

        public void Delete(int id)
        {
            List<T> items = GetAll();
            T item = items.FirstOrDefault(x => x.Id == id);

            if (item != null)
            {
                items.Remove(item);
                dataStore.Save(filePath, items);
            }
        }

        public T? this[int id] => GetById(id);
    }
}


// kolan barnamam in bood ke bejaye inke faghat ye bakhshi az data.json ro tagir bedim, kolesh ro barmidarim,taghir ijad mikonim baad kolesh ro az aval toye file data.json zakhire mikonim. yani ye file jadid. in harkat konde va ram ziadi mikhore, vali khob sade tar bood. xd. proje ham koochike aslan taasiri nadare ke mozavajehesh beshim.
