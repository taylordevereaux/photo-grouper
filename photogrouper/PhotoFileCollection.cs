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

        /// <summary>
        /// Groups Each Photo into a folder based on the specific text value.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Dictionary<string, List<PhotoFile>> GroupBy(Func<PhotoFile, string> predicate)
        {
            Dictionary<string, List<PhotoFile>> result = new Dictionary<string, List<PhotoFile>>();

            // Overflow check
            checked
            {
                foreach (var file in this._list)
                {
                    string groupBy = predicate(file);
                    result[groupBy] = result.ContainsKey(groupBy) ?  result[groupBy] : new List<PhotoFile>();
                    result[groupBy].Add(file);
                }
            }

            return result;
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

        Dictionary<string, List<PhotoFile>> GroupBy(Func<PhotoFile, string> predicate);
    }
}
