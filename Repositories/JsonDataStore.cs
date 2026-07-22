using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace library_system.Repositories
{
    public class JsonDataStore
    {
        private readonly string BasePath = "Data";

        public List<T> Load<T>(string fileName) // toye in method gharare data ro az [data].json bekhonim va loadesh konim ta dar ayande betonim toye data ha taghir mesl remove, update, add va ... ezafe konim
        {
            string path = Path.Combine(BasePath, fileName); // inja darim path be file json ro dorost mikonim.

            if (!File.Exists(path))
                return new List<T>(); // inja agar file mojod nabood, misazatesh (albate alan faghat ye list khali barmigardone, file json ro hanoz nemisaze, faghat az error jolo giri mikone)

            string json = File.ReadAllText(path); // in code file toye Path ro baz mikone va mirizatesh toye var bename json
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>(); // in khat var json ro migire, baadesh tabdilesh mikone be list az object haye type T, yani agar T book bashe, json ro tarjome mikone be List<Book>. ?? new List<T>() ham chek mikone agar null bood, fagaht ye list khali ersal kone
        }

        public void Save<T>(string fileName, List<T> data) // gharare inja baad az ijade taghir ha , dobare file ro toye [data].json save konim
        {
            string path = Path.Combine(BasePath, fileName);

            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true }
            );
            File.WriteAllText(path, json);
        }
        // JsonSerializer.Serialize daghighan barakse   JsonSerializerDeserialize hast. yani list ro migire be json tabdilesh mikone. writeIndented = true ham bareye inke file ro dorost va fasele gozari dorost va toye chand khat benevise, agar ino nazarim kole data ro toye ye khat minivise. in chiz hayi ke marboot be Json bood is AI yad gereftam agar khasti bego behet bedam. vali khob niaz nist estefade koni chon mitoni az hami file istefade koni. hala vaghti az in file estefade konam manzoram ro motavaje mishi.
    }
}
