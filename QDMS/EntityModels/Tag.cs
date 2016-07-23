// -----------------------------------------------------------------------
// <copyright file="Tag.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProtoBuf;

namespace QDMS
{
    [ProtoContract]
    public class Tag : IEquatable<Tag>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ProtoMember(1)]
        public int ID { get; set; }

        [ProtoMember(2)]
        [MaxLength(255)]
        public string Name { get; set; }
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Tag other)
        {
            return other.ID == ID && other.Name == Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
