﻿using System;
using System.Collections.Generic;

namespace Rinsen.InnovationBoost.Models
{
    public class LoggerModel
    {


        public SelectionOptions SelectionOptions { get; set; }

    }


    public class SelectionOptions
    {
        public IEnumerable<SelectionLogApplication> LogApplications { get; set; }

        public IEnumerable<SelectionLogEnvironment> LogEnvironments { get; set; }

        public IEnumerable<SelectionLogLevel> LogLevels { get; set; }

        public IEnumerable<SelectionLogSource> LogSources { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }

    }

    public class SelectionLogApplication
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }
    }

    public class SelectionLogEnvironment
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

    }

    public class SelectionLogLevel
    {
        public int Level { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

    }

    public class SelectionLogSource
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

    }
}
