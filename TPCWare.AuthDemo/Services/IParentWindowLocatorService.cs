using System;
using System.Collections.Generic;
using System.Text;

namespace TPCWare.AuthDemo.Services
{
    /// <summary>
    /// Simple platform specific service that is responsible for locating a 
    /// </summary>
    public interface IParentWindowLocatorService
    {
       object GetCurrentParentWindow();
    }
}
