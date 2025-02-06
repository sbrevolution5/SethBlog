using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Enums
{
    public enum ModerationReason
    {
        None,//Unmoderated default
        Profanity,
        [Display(Name = "Sexual Content")]
        Sexual,
        Fraud,
        Phishing,
        [Display(Name = "Hate")]
        HateSpeech,
        [Display(Name = "Drug References")]
        Drugs,
        [Display(Name = "Threatening another user")]
        Threatening,
        [Display(Name = "Attempt to shame or expose a public Identity")]
        Doxx

    }
}
