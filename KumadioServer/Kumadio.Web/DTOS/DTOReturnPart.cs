﻿namespace Kumadio.Web.DTOS
{
    public class DTOReturnPart
    {
        public int Id { get; set; }
        public string PartAudioLink { get; set; }
        public bool IsRoot { get; set; } = false;
        public int BookId { get; set; }
        public DTOReturnAnswer? ParentAnswer { get; set; }
        public IEnumerable<DTOReturnAnswer?> Answers { get; set; }
    }
}
