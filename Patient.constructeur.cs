using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthernLightsHospital {
    public partial class Patient {
        public Patient(int nSS, DateTime dateNaissance, string nom, string prenom, string adresse, string ville, string province, string codePostal, string telephone, int iDAssurance) {
            NSS = nSS;
            DateNaissance = dateNaissance;
            Nom = nom;
            Prenom = prenom;
            Adresse = adresse;
            Ville = ville;
            Province = province;
            CodePostal = codePostal;
            this.telephone = telephone;
            IDAssurance = iDAssurance;
        }
    }
}
