using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthernLightsHospital {
    public partial class Admission {

        public Admission() {
        }

        public Admission(int iDAdmission, bool chirurgieProgramme, DateTime dateAdmission, DateTime? dateChirurgie, bool televiseur, bool telephone, int nSS, int numeroLit, int? iDMedecin, decimal tarifLitQuotidien) {
            IDAdmission = iDAdmission;
            this.chirurgieProgramme = chirurgieProgramme;
            DateAdmission = dateAdmission;
            DateChirurgie = dateChirurgie;
            Televiseur = televiseur;
            Telephone = telephone;
            NSS = nSS;
            NumeroLit = numeroLit;
            IDMedecin = iDMedecin;
            TarifLitQuotidien = tarifLitQuotidien;
        }
    }
}

