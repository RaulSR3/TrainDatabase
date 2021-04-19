﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Model
{
    public partial class Vehicle
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string ImageName { get; set; }

        public long? Type { get; set; }

        public long? MaxSpeed { get; set; }

        public long? Address { get; set; }

        public long? Active { get; set; }

        public long? Position { get; set; }

        public string DriversCab { get; set; }

        public string FullName { get; set; }

        public long? SpeedDisplay { get; set; }

        public string Railway { get; set; }

        public string BufferLenght { get; set; }

        public string ModelBufferLenght { get; set; }

        public string ServiceWeight { get; set; }

        public string ModelWeight { get; set; }

        public string Rmin { get; set; }

        public string ArticleNumber { get; set; }

        public string DecoderType { get; set; }

        public string Owner { get; set; }

        public string BuildYear { get; set; }

        public string OwningSince { get; set; }

        public long? TractionDirection { get; set; }

        public string Description { get; set; }

        public long? Dummy { get; set; }

        public string Ip { get; set; }

        public long? Video { get; set; }

        public long? DirectSteering { get; set; }

        public long? Crane { get; set; }

        public Category Category { get; set; } = new();

        public int CategoryId { get; set; } = new();

        public List<Function> Functions { get; set; }

    }
}
