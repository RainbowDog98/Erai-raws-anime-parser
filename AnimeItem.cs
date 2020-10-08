﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeDownloader
{
    class AnimeItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PubDate { get; set; }
        public bool IsInWatchList { get; set; }
        public bool IsReleasedToday { get; set; }
    }
}
