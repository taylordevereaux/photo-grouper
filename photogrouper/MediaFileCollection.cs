using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Managers
{
    public class MediaFileCollection<T> : IFileCollection<T> where T : MediaFile
    {
        private List<T> _list = new List<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public interface IFileCollection<T> : IEnumerable<T> where T : MediaFile
    {

    }
}
