using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Lütfen bir isim girin.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Lütfen geçerli bir e-mail girin.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Lütfen bir şifre girin.")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$",
            ErrorMessage = "En az bir büyük karakter ve rakam içeren 8 harflik şifre girin")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Şifre eşleşmiyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPasword { get; set; }
        [Required(ErrorMessage = "Lütfen müşteri yada kurum seçeneklerinden birini seçiniz.")]
        public string Role { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        public double? Distance { get; set; }
        public bool Status { get; set; }
    }
}