﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.UnitOfWork
{
    interface IUnitOfWork : IDisposable
    {
        ILocationRepository
    }
}
