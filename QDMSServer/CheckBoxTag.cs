// -----------------------------------------------------------------------
// <copyright file="CheckBoxTag.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using QDMS;
using QDMSServer.Helpers;

namespace QDMSServer
{
    public class CheckBoxTag : CheckableItem<Tag>
    {
        public CheckBoxTag(Tag item, bool isChecked = false) : base(item, isChecked)
        {
        }
    }
}
