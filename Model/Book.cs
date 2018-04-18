//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class Book
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Book_Name { get; set; }
        public Nullable<int> Category_Id { get; set; }
        public Nullable<int> Publisher_Id { get; set; }
        public Nullable<int> Author_Id { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<int> Amount { get; set; }
        [JsonIgnore]
        public virtual Author Author { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual Publisher Publisher { get; set; }
    }
}