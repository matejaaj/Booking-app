﻿using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Model
{
    public class Language : ISerializable
    {
        public int languageId;

        public string Name { get; set; }

        public Language() { }

        public Language(string name)
        {
            Name = name;
        }


        public void FromCSV(string[] values)
        {
            languageId = Convert.ToInt32(values[0]);
            Name = values[1];
        }

        public string[] ToCSV()
        {
            string[] csvValues = { languageId.ToString(), Name };
            return csvValues;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}
