//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NorthernLightsHospital
{
    using System;
    using System.Collections.Generic;
    
    public partial class Admission
    {
        public int IDAdmission { get; set; }
        public bool chirurgieProgramme { get; set; }
        public System.DateTime DateAdmission { get; set; }
        public Nullable<System.DateTime> DateChirurgie { get; set; }
        public Nullable<System.DateTime> DateConge { get; set; }
        public bool Televiseur { get; set; }
        public bool Telephone { get; set; }
        public int NSS { get; set; }
        public int NumeroLit { get; set; }
        public Nullable<int> IDMedecin { get; set; }
        public decimal TarifLitQuotidien { get; set; }
    
        public virtual Lit Lit { get; set; }
        public virtual Medecin Medecin { get; set; }
        public virtual Patient Patient { get; set; }
    }
}