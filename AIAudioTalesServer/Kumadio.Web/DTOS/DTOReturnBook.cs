﻿namespace Kumadio.Web.DTOS
{
    public class DTOReturnBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
    }
}
