﻿using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOPurchase
    {
        public int BookId { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }

}
