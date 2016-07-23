// -----------------------------------------------------------------------
// <copyright file="SessionsSource.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace QDMS
{
    public enum SessionsSource : int
    {
        /// <summary>
        /// Exchange
        /// </summary>
        Exchange = 0,
        /// <summary>
        /// Template
        /// </summary>
        Template = 1,
        /// <summary>
        /// Custom
        /// </summary>
        Custom = 2,
    }
}
