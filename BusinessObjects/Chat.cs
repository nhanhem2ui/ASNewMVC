using BussinessObject;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Chat
    {
        public string ChatId { get; set; }

        public string Message { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{HH:mm dd/MM/yyyy}")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public short? SenderId { get; set; }

        [ValidateNever]
        public virtual SystemAccount Sender { get; set; }

        public short? ReceiverId { get; set; }
        [ValidateNever]
        public virtual SystemAccount Receiver { get; set; }
    }
}
