//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jwt_Template.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class tblFileDetails
    {
        public int FileId { get; set; }
        [Required]
        [Display(Name = "Supported Files .doc | .docx")]
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public List<tblFileDetails> FileList { get; set; }
    }
}
