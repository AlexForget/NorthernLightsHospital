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
    /// Interaction logic for FenetreAdmin.xaml
    /// </summary>
    public partial class FenetreAdmin : Window {
        NorthernLightsHospitalEntities myBd;

        public FenetreAdmin() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            RafraichirGridMedecin();
        }


        //----------------------------------------------------------------------------------
        //************************** SECTION GESTION DES MÉDECINS **************************
        //----------------------------------------------------------------------------------



        private void btnDeconnection_Click(object sender, RoutedEventArgs e) {
            Accueil accueil = new Accueil();
            Close();
            accueil.ShowDialog();
        }

        // Ouvre une nouvelle fenêtre pour l'ajout d'un nouveau médecin
        private void btnAjouterMedecin_Click(object sender, RoutedEventArgs e) {
            AjouterMedecin ajoutMed = new AjouterMedecin();
            ajoutMed.ShowDialog();
        }

        // Recherche le médecin à modifier et ouvre une nouvelle fenêtre
        private void btnModifierMedecin_Click(object sender, RoutedEventArgs e) {
            // Vérifier qu'un médecin a bien été sélectionner dans le dataGrid avant de faire une modification
            if (dataMedecin.SelectedItems.Count != 1) {
                MessageBox.Show("Vous devez sélectionner un médecin.",
                    "Selectionnez un médecin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Pour obtenir la propiété IDMedecin de l'objet anonyme obtenu avec le selectedItem du dataGrid
            var ligneDataMedecin = dataMedecin.SelectedItem;
            Type type = ligneDataMedecin.GetType();
            int idMedecin = (int)type.GetProperty("IDMedecin").GetValue(ligneDataMedecin, null);

            // Pour obtenir la ligne de la BD correspondent au ID du médecin à moddifier
            Medecin medecin =
                (from m in myBd.Medecins
                 where m.IDMedecin == idMedecin
                 select m).SingleOrDefault();

            ModifierMedecin modMedecin = new ModifierMedecin(medecin);
            modMedecin.ShowDialog();

        }

        private void btnSupprimerMedecin_Click(object sender, RoutedEventArgs e) {
            // Vérifier qu'un médecin a bien été sélectionner dans le dataGrid avant de supprimer
            if (dataMedecin.SelectedItems.Count != 1) {
                MessageBox.Show("Vous devez sélectionner un médecin.",
                    "Selectionnez un médecin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Pour obtenir la propiété IDMedecin de l'objet anonyme obtenu avec le selectedItem du dataGrid
            var ligneDataMedecin = dataMedecin.SelectedItem;
            Type type = ligneDataMedecin.GetType();
            int idMedecin = (int)type.GetProperty("IDMedecin").GetValue(ligneDataMedecin, null);

            string nomMedecin = (string)type.GetProperty("Nom").GetValue(ligneDataMedecin, null);
            string prenomMedecin = (string)type.GetProperty("Prenom").GetValue(ligneDataMedecin, null);

            // Pour obtenir la ligne de la BD correspondent au ID du médecin à supprimer
            Medecin medecin =
                (from m in myBd.Medecins
                where m.IDMedecin == idMedecin
                 select m).SingleOrDefault();

            medecin.Statut = false;

            // Suppression de la ligne das la BD et sauvegarde des changements
            try {
                myBd.SaveChanges();
                MessageBox.Show($"ID : {idMedecin} - {nomMedecin.Trim()}, {prenomMedecin.Trim()} a bien été supprimé.", 
                    "Supprimer un Médecin", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);

                RafraichirGridMedecin();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        // Permet de mettre à jour la dataGrid des médecins lors du retour à la fenêtre
        // de l'administrateur après les modification ou ajout de médecin
        private void FenetreAdmin1_Activated(object sender, EventArgs e) {
            RafraichirGridMedecin();
        }

        // Mise à jour du dataGrid des médecins
        public void RafraichirGridMedecin() {
            myBd = new NorthernLightsHospitalEntities();
            var query =
                from m in myBd.Medecins
                where m.Statut == true
                select new { m.IDMedecin, m.NomMedecin, m.PrenomMedecin };

            dataMedecin.DataContext = query.ToList();
        }



        //---------------------------------------------------------------------------------
        //************************ SECTION TABLEAU DE CONSULTATION ************************
        //---------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------
        //Rapport des patients
        //---------------------------------------------------------------------------------

        private void cboxRapportPatient_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selectedIndex = cboxRapportPatient.SelectedIndex;

            RemettreComboboxIndexZero(cboxRapportPatient, selectedIndex);

            switch (selectedIndex) {
                case 0:
                    AfficherListePatient();
                    break;
                case 1:
                    AfficherListePatientHospitalise();
                    break;
            }
        }



        private void AfficherListePatient() {
            var listePatient =
                from pat in myBd.Patients
                join ass in myBd.Assurances on pat.IDAssurance equals ass.IDAssurance
                join adm in myBd.Admissions on pat.NSS equals adm.NSS
                join med in myBd.Medecins on adm.IDMedecin equals med.IDMedecin
                orderby adm.NSS
                select new { pat.NSS, pat.DateNaissance, pat.Nom, pat.Prenom, pat.Adresse, pat.Ville, pat.Province, pat.CodePostal, pat.telephone, ass.NomCompagnie, adm.DateAdmission, adm.IDAdmission, adm.NumeroLit, med.NomMedecin, med.PrenomMedecin};

            var listePatientSansDoublon = listePatient.ToList()
                                                      .GroupBy(i => i.NSS)
                                                      .Select(j => j.First());

            dataGridPatientListe.DataContext = listePatientSansDoublon;
            CacherDataGrid();
            dataGridPatientListe.Visibility = Visibility.Visible;
        }

        private void AfficherListePatientHospitalise() {
            var listePatient =
                from pat in myBd.Patients
                join ass in myBd.Assurances on pat.IDAssurance equals ass.IDAssurance
                join adm in myBd.Admissions on pat.NSS equals adm.NSS
                join med in myBd.Medecins on adm.IDMedecin equals med.IDMedecin
                where adm.DateConge == null
                select new { pat.NSS, pat.DateNaissance, pat.Nom, pat.Prenom, pat.Adresse, pat.Ville, pat.Province, pat.CodePostal, pat.telephone, ass.NomCompagnie, adm.DateAdmission, adm.IDAdmission, adm.NumeroLit, med.NomMedecin, med.PrenomMedecin };

            dataGridPatientListe.DataContext = listePatient.ToList();
            CacherDataGrid();
            dataGridPatientListe.Visibility = Visibility.Visible;
        }



        //---------------------------------------------------------------------------------
        //Rapport des médecins
        //---------------------------------------------------------------------------------

        private void cboxRapportMedecin_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selectedIndex = cboxRapportMedecin.SelectedIndex;

            RemettreComboboxIndexZero(cboxRapportMedecin, selectedIndex);

            switch (selectedIndex) {
                case 0:
                    AfficherListeMedecin();
                    break;
            }
        }

        private void AfficherListeMedecin() {
            var listeMedecin =
                from med in myBd.Medecins
                select new { med.IDMedecin, med.NomMedecin, med.PrenomMedecin };


            dataGridMedecinListe.DataContext = listeMedecin.ToList();
            CacherDataGrid();
            dataGridMedecinListe.Visibility = Visibility.Visible;
        }



        //---------------------------------------------------------------------------------
        //Rapport des lits
        //---------------------------------------------------------------------------------

        private void cboxRapportLit_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selectedIndex = cboxRapportLit.SelectedIndex;

            RemettreComboboxIndexZero(cboxRapportLit, selectedIndex);

            switch (selectedIndex) {
                case 0:
                    AfficherListeLit();
                    break;
            }
        }


        private void AfficherListeLit() {
            var listeLit =
                from lit in myBd.Lits
                join dep in myBd.Departements on lit.IDDepartement equals dep.IDDepartement
                join typ in myBd.TypeLits on lit.IDTypeLit equals typ.IDTypeLit
                select new { lit.NumeroLit, lit.Occupe, typ.description, dep.NomDepartement };

            dataGridLitListe.DataContext = listeLit.ToList();
            CacherDataGrid();
            dataGridLitListe.Visibility = Visibility.Visible;
        }


        //---------------------------------------------------------------------------------
        //Rapport des assurances
        //---------------------------------------------------------------------------------

        private void cboxRapportAssurance_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selectedIndex = cboxRapportAssurance.SelectedIndex;

            RemettreComboboxIndexZero(cboxRapportAssurance, selectedIndex);

            switch (selectedIndex) {
                case 0:
                    AfficherListeAssurance();
                    break;
            }
        }

        private void AfficherListeAssurance() {
            var listeAssurance =
                from ass in myBd.Assurances
                select new { ass.IDAssurance, ass.NomCompagnie };

            dataGridAssuranceListe.DataContext = listeAssurance.ToList();
            CacherDataGrid();
            dataGridAssuranceListe.Visibility = Visibility.Visible;
        }





        //---------------------------------------------------------------------------------
        //Rapport des Départements
        //---------------------------------------------------------------------------------

        private void cboxRapportDepartement_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selectedIndex = cboxRapportDepartement.SelectedIndex;

            RemettreComboboxIndexZero(cboxRapportDepartement, selectedIndex);

            switch (selectedIndex) {
                case 0:
                    AfficherListeDepartement();
                    break;
            }
        }

        private void AfficherListeDepartement() {
            var listeDepartement =
                from dep in myBd.Departements
                select new { dep.IDDepartement, dep.NomDepartement };

            dataGridDepartementListe.DataContext = listeDepartement.ToList();
            CacherDataGrid();
            dataGridDepartementListe.Visibility = Visibility.Visible;
        }



        //---------------------------------------------------------------------------------
        //Rapport des Admissions
        //---------------------------------------------------------------------------------

        private void cboxRapportAdmission_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selectedIndex = cboxRapportAdmission.SelectedIndex;

            RemettreComboboxIndexZero(cboxRapportAdmission, selectedIndex);

            switch (selectedIndex) {
                case 0:
                    AfficherListeAdmission();
                    break;
                case 1:
                    AfficherListeAdmissionChirurgie();
                    break;
            }
        }

        private void AfficherListeAdmission() {
            var listeAdmission =
                from adm in myBd.Admissions
                join pat in myBd.Patients on adm.NSS equals pat.NSS
                join med in myBd.Medecins on adm.IDMedecin equals med.IDMedecin
                join lit in myBd.Lits on adm.NumeroLit equals lit.NumeroLit
                join dep in myBd.Departements on lit.IDDepartement equals dep.IDDepartement
                select new { adm.IDAdmission, pat.Nom, pat.Prenom, adm.chirurgieProgramme, adm.DateChirurgie, adm.DateAdmission, adm.DateConge, adm.Televiseur, adm.Telephone, adm.NSS, dep.NomDepartement, adm.NumeroLit, adm.IDMedecin, med.NomMedecin, med.PrenomMedecin, adm.TarifLitQuotidien };

            dataGridAdmissionListe.DataContext = listeAdmission.ToList();
            CacherDataGrid();
            dataGridAdmissionListe.Visibility = Visibility.Visible;
        }

        private void AfficherListeAdmissionChirurgie() {
            var listeAdmission =
                from adm in myBd.Admissions
                join pat in myBd.Patients on adm.NSS equals pat.NSS
                join med in myBd.Medecins on adm.IDMedecin equals med.IDMedecin
                join lit in myBd.Lits on adm.NumeroLit equals lit.NumeroLit
                join dep in myBd.Departements on lit.IDDepartement equals dep.IDDepartement
                where adm.chirurgieProgramme == true
                select new { adm.IDAdmission, pat.Nom, pat.Prenom, adm.chirurgieProgramme, adm.DateChirurgie, adm.DateAdmission, adm.DateConge, adm.Televiseur, adm.Telephone, adm.NSS, dep.NomDepartement, adm.NumeroLit, adm.IDMedecin, med.NomMedecin, med.PrenomMedecin, adm.TarifLitQuotidien };

            dataGridAdmissionListe.DataContext = listeAdmission.ToList();
            CacherDataGrid();
            dataGridAdmissionListe.Visibility = Visibility.Visible;
        }




        // rendre tous les dataGrid Hidden
        private void CacherDataGrid() {
            dataGridPatientListe.Visibility = Visibility.Hidden;
            dataGridPatientHospitalise.Visibility = Visibility.Hidden;
            dataGridMedecinListe.Visibility = Visibility.Hidden;
            dataGridLitListe.Visibility = Visibility.Hidden;
            dataGridAssuranceListe.Visibility = Visibility.Hidden;
            dataGridDepartementListe.Visibility = Visibility.Hidden;
            dataGridAdmissionListe.Visibility = Visibility.Hidden;
        }

        // Remettre tous les comboBox à l'index zéro
        private void RemettreComboboxIndexZero(Control c, int selectedIndex) {

            if (c != cboxRapportAssurance) {
                cboxRapportAssurance.SelectedIndex = -1;
            }

            if (c != cboxRapportLit) {
                cboxRapportLit.SelectedIndex = -1;
            }

            if (c != cboxRapportMedecin) {
                cboxRapportMedecin.SelectedIndex = -1;
            }

            if (c != cboxRapportPatient) {
                cboxRapportPatient.SelectedIndex = -1;
            }

            if (c != cboxRapportDepartement) {
                cboxRapportPatient.SelectedIndex = -1;
            }

            if (c != cboxRapportAdmission) {
                cboxRapportPatient.SelectedIndex = -1;
            }
        }

        
    }
}
