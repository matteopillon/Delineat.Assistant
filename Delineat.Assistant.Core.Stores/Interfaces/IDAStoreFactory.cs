﻿using Delineat.Assistant.Core.Interfaces;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Stores.Intefaces
{
    public interface IDAStoreFactory
    {
        IDAStore MakeStore();
    }
}
