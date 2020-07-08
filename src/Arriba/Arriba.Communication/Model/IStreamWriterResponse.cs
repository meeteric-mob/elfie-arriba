﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arriba.Communication
{
    public interface IStreamWriterResponse : IResponse
    {
        string ContentType { get; }

        Task WriteToStreamAsync(Stream outputStream);
    }
}
