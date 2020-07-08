﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

using Arriba.Communication;

namespace Arriba.Server.Owin
{
    public abstract class ArribaRequest : IRequest
    {
        private IContentReaderWriterService _readerWriter;

        private Lazy<RequestVerb> _verbLazy = null;
        private Lazy<string> _pathLazy = null;
        private Lazy<IValueBag> _queryStringLazy = null;
        private Lazy<IValueBag> _headersLazy = null;
        private Lazy<string[]> _acceptLazy = null;

        private static readonly string[] s_emptyStringArray = new string[0];

        protected ArribaRequest(IContentReaderWriterService readerWriter)
        {
            _readerWriter = readerWriter;
            _verbLazy = new Lazy<RequestVerb>(this.GetVerb);
            _acceptLazy = new Lazy<string[]>(this.GetAcceptHeaders);
            _pathLazy = new Lazy<string>(GetRequestPath);
            _queryStringLazy = new Lazy<IValueBag>(GetQueryString);
            _headersLazy = new Lazy<IValueBag>(GetRequestHeaders);
        }

        protected abstract string GetRequestPath();
        protected abstract IValueBag GetQueryString();
        protected abstract IValueBag GetRequestHeaders();

        public RequestVerb Method
        {
            get
            {
                return _verbLazy.Value;
            }
        }

        public string Resource
        {
            get
            {
                return _pathLazy.Value;
            }
        }

        public IValueBag ResourceParameters
        {
            get
            {
                return _queryStringLazy.Value;
            }
        }

        public IValueBag Headers
        {
            get
            {
                return _headersLazy.Value;
            }
        }

        public abstract IPrincipal User { get; }

        public abstract string Origin { get; }

        public bool HasBody
        {
            get
            {
                return this.Headers.Contains("Content-Length") && this.Headers["Content-Length"] != "0";
            }
        }

        public abstract Stream InputStream { get; }

        protected abstract string HttpRequestVerb { get; }

        public System.Threading.Tasks.Task<T> ReadBodyAsync<T>()
        {
            var reader = _readerWriter.GetReader<T>(this.Headers["Content-Type"]);
            return reader.ReadAsync<T>(this.InputStream);
        }

        public IEnumerable<string> AcceptedResponseTypes
        {
            get
            {
                return _acceptLazy.Value;
            }
        }

        private RequestVerb GetVerb()
        {
            var verb = this.HttpRequestVerb;

            switch (verb)
            {
                case "GET":
                    return RequestVerb.Get;
                case "POST":
                    return RequestVerb.Post;
                case "DELETE":
                    return RequestVerb.Delete;
                case "OPTIONS":
                    return RequestVerb.Options;
                case "PUT":
                    return RequestVerb.Put;
                case "PATCH":
                    return RequestVerb.Patch;
                default:
                    throw new ArgumentException("Unknown HTTP Verb \"" + verb + "\"");
            }
        }

        private string[] GetAcceptHeaders()
        {
            string[] acceptRaw;

            if (this.Headers.TryGetValues("Accept", out acceptRaw) && acceptRaw != null && acceptRaw.Length > 0)
            {
                string[] items = acceptRaw.SelectMany(a => a.Split(',')).ToArray();

                for (int i = 0; i < items.Length; i++)
                {
                    int paramsStartIndex = items[i].IndexOf(';');

                    if (paramsStartIndex != -1)
                    {
                        // Strip accept params (e.g. q=xxx) 
                        items[i] = items[i].Substring(0, paramsStartIndex).Trim();
                    }
                }

                return items;
            }

            return s_emptyStringArray;
        }
    }
}
