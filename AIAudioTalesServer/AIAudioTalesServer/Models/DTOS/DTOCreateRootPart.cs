﻿namespace AIAudioTalesServer.Models.DTOS
{
    public class DTONewRootPart
    {
        public int BookId { get; set; }
        public string PartAudioLink { get; set; }
        public IList<DTOCreateAnswer> Answers { get; set; }
    }
}