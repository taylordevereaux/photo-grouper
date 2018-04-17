using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper
{
    public class PhotoCollection : IPhotoCollection
    {
        private List<PhotoFile> _list = new List<PhotoFile>();

        public PhotoCollection(List<PhotoFile> files)
        {
            this._list.AddRange(files);
        }

        public async Task<string> ToJson()
        {
            return await Task.Run(() => ToJsonSync());
        }

        public string ToJsonSync()
        {
            return JsonConvert.SerializeObject(this._list);
        }

        public IEnumerator<PhotoFile> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public interface IPhotoCollection : IEnumerable<PhotoFile>
    {
        Task<string> ToJson();

        string ToJsonSync();
    }
}
