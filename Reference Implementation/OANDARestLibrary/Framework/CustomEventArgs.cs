using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.Framework
{
    public class CustomEventArgs<T>
    {
        public CustomEventArgs(T content)
        {
            Item = content;
        }

        public T Item { get; private set; }
    }
}
