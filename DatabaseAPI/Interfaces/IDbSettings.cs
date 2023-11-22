using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAPI.Interfaces;

public interface IDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}
