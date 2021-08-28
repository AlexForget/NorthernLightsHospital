using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NorthernLightsHospital {
    static class util
    {
        
        public static bool ValiderNumeroTelephone(string numeroTelephone) {
            bool validation = false;
            Regex regexUn = new Regex(@"^\((\d{3})\)(\d{3})\-(\d{4})$");
            Regex regexDeux = new Regex(@"^\d{0}(?:\d{10})?$");

            if (regexUn.IsMatch(numeroTelephone)) {
                validation = true;
            }
            if (regexDeux.IsMatch(numeroTelephone)) {
                validation = true;
            }
            return validation;
        }

        public static string FormatterNumeroTelephone(string telephone) {
            if (!telephone.Equals("") && !telephone[0].Equals('(')) {
                telephone = telephone.Insert(0, "(");
                telephone = telephone.Insert(4, ")");
                telephone = telephone.Insert(8, "-");
            }
            return telephone;
        }

        public static bool ValiderCodePostal(string codePostal) {
            Regex myRegex = new Regex(@"^([A-Za-z]\d[A-Za-z]\s?\d[A-Za-z]\d)?$");

            if (myRegex.IsMatch(codePostal)) {
                return true;
            }
            return false;
        }

        public static string FormatterCodePostal(string codePostal) {
            if (!codePostal.Equals("") && !codePostal[4].Equals(' ')) {
                codePostal = codePostal.Insert(3, " ");
                codePostal = codePostal.ToUpper();
            }
            return codePostal;
        }

        public static bool ValiderNom(string nom) {
            Regex myRegex = new Regex(@"^[a-zA-ZÀ-ÿ\-\']*$");
            return myRegex.IsMatch(nom);
        }

        public static bool ValiderNSS(string nss) {
            Regex myRegex = new Regex(@"^\d{0}(?:\d{9})?$");
            return myRegex.IsMatch(nss);
        }
    }
}