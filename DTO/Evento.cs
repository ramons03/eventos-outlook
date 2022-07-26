﻿using System;

namespace eventosoutlook.DTO
{
    public class Evento
    {
        public string Subject { get; set; }
        public string BodyContent { get; set; }
        public string LocationName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}