using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP.Data.Interfaces;

public interface IFileReader<T>
{
    T Read();
}
