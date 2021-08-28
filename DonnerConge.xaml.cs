using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NorthernLightsHospital {
    /// <summary>
    /// Interaction logic for DonnerConge.xaml
    /// </summary>
    public partial class DonnerConge : Window {
        const double TARIF_TELEVISEUR = 42.50;
        const double TARIF_TELEPHONE = 7.50;


        NorthernLightsHospitalEntities myBd;
        Admission admission;
        int idMedecin;

        public DonnerConge(int idMedecin) {
            InitializeComponent();
            this.idMedecin = idMedecin;
        }

        private void FenetreConge_Loaded(object sender, RoutedEventArgs e) {
            myBd = new NorthernLightsHospitalEntities();

            RafraichirGridPatient();
        }

        // Permet d'afficher les informations des patients selon le médecin qui c'est connecté 
        public void RafraichirGridPatient() {
            var query =
                from p in myBd.Patients
                join ass in myBd.Assurances on p.IDAssurance equals ass.IDAssurance
                join adm in myBd.Admissions on p.NSS equals adm.NSS
                where adm.IDMedecin == idMedecin
                where adm.DateConge == null
                select new { p.NSS, p.DateNaissance, p.Nom, p.Prenom, p.Adresse, p.Ville, p.Province, p.CodePostal, p.telephone, ass.NomCompagnie, adm.DateAdmission ,adm.IDAdmission, adm.NumeroLit };

            dataPatients.DataContext = query.ToList();
        }

        private void btnDeconnecter_Click(object sender, RoutedEventArgs e) {
            Accueil accueil = new Accueil();
            Close();
            accueil.ShowDialog();
        }

        private void btnDonnerConge_Click(object sender, RoutedEventArgs e) {
            
            Lit lit;

            // Vérifier qu'un patient a bien été sélectionner dans le dataGrid avant de donner un congé
            if (dataPatients.SelectedItems.Count != 1) {
                MessageBox.Show("Vous devez sélectionner un patient.",
                    "Selectionnez un patient",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Retrouve l'admission à modifier et change la date de congé
            admission = RetrouverAdmission();

            // Valide qu'une date de congé à bien été saisi
            if (dateDateConge.SelectedDate == null) {
                MessageBox.Show("Vous devez sélectionner une date de congé.",
                    "Selectionnez une date",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Valide que la date de congé ne précède pas la date d'admission
            if (((DateTime)dateDateConge.SelectedDate - admission.DateAdmission).TotalDays < 0) {
                MessageBox.Show("La date de congé ne peut pas précéder la date d'admission",
                    "Date de congé invalide",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            admission.DateConge = (DateTime)dateDateConge.SelectedDate;

            // Retrouve le lit à modifier et change son status pour le rendre disponible
            lit = RetrouverLit();
            lit.Occupe = false;

            // Sauvegarde des modifications aportés à la BD
            try {
                myBd.SaveChanges();
                MessageBox.Show("Le congé a été complété avec succèes!",
                                "Congé complété",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " Erreur ici!");
            }

            RafraichirGridPatient();
            CalculerFacture();
        }

        private void CalculerFacture() {
            int nombreJoursAdmis;
            double tarifTeleviseur = 0;
            double tarifTelephone = 0;
            double tarifLit;
            double factureTotal;
            DateTime dateConge = (DateTime)dateDateConge.SelectedDate;
            DateTime dateAdmission = admission.DateAdmission;

            nombreJoursAdmis = (int)(dateConge - dateAdmission).TotalDays;

            if (admission.Televiseur == true) {
                tarifTeleviseur = nombreJoursAdmis * TARIF_TELEVISEUR;
            }

            if (admission.Telephone == true) {
                tarifTelephone = nombreJoursAdmis * TARIF_TELEPHONE;
            }

            tarifLit = nombreJoursAdmis * (double)admission.TarifLitQuotidien;
            factureTotal = tarifLit + tarifTelephone + tarifTeleviseur;

            MessageBox.Show($"Solde pour le lit : {tarifLit}$ \n" +
                            $"Solde pour le téléviseur : {tarifTeleviseur}$ \n" +
                            $"Solde pour le téléphone : {tarifTelephone}$ \n\n" + 
                            $"Facture total : {factureTotal}$",
                            "Montant à facturer",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private Admission RetrouverAdmission() {
            int idAdmission;

            // Pour obtenir la propiété IDAdmission de l'objet anonyme obtenu avec le selectedItem du dataGrid
            var ligneDataPatient = dataPatients.SelectedItem;
            Type type = ligneDataPatient.GetType();
            idAdmission = (int)type.GetProperty("IDAdmission").GetValue(ligneDataPatient, null);

            // Pour obtenir la ligne de la BD correspondent au ID de l'admission à moddifier
            Admission admission =
                (from adm in myBd.Admissions
                 where adm.IDAdmission == idAdmission
                 select adm).SingleOrDefault();

            return admission;
        }

        private Lit RetrouverLit() {
            int numeroLit;

            // Pour obtenir la propiété NumeroLit de l'objet anonyme obtenu avec le selectedItem du dataGrid
            var ligneDataPatient = dataPatients.SelectedItem;
            Type type = ligneDataPatient.GetType();
            numeroLit = (int)type.GetProperty("NumeroLit").GetValue(ligneDataPatient, null);

            // Pour obtenir la ligne de la BD correspondent au Numero de lit de l'admission à moddifier
            Lit lit =
                (from l in myBd.Lits
                 where l.NumeroLit == numeroLit
                 select l).SingleOrDefault();

            return lit;
        }
    }
}
