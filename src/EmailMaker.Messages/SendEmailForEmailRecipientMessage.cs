using System;

namespace EmailMaker.Messages
{
    [Serializable]
    public class SendEmailForEmailRecipientMessage
    {
        public int EmailId { get; set; }
        public int RecipientId { get; set; }
        public string FromAddress { get; set; }
        public string RecipientEmailAddress { get; set; }
        public string RecipientName { get; set; }
        public string Subject { get; set; }
        public string EmailHtml { get; set; }
    }
}