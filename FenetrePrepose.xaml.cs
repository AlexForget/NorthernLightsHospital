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
    /// Interaction logic for FenetrePrepose.xaml
    /// </summary>
    public partial class FenetrePrepose : Window {
        const int DEPARTEMENT_CHIRURGIE = 0;
        const int DEPARTEMENT_PEDIATRIE = 2;

        NorthernLightsHospitalEntities myBd;
        Patient patient;
        Admission admission;
        Lit lit;

        bool enregistrementPatient = false;
        bool patientDejaExistant = false;
        double agePatient;
        

        public FenetrePrepose() {
            InitializeComponent();
        }

        private void FenetrePrepose1_Loaded(object sender, RoutedEventArgs e) {
            myBd = new NorthernLightsHospitalEntities();

            gboxPatient.DataContext = myBd.Assurances.ToList();
            cboxAssuranceAdmission.SelectedIndex = -1;
            RafraichirGridPatient();
            ListeMedecin();
            AfficherIDAdmission();
            AffichageDepartementLit();
            AfficherTypeLitAdmission();
        }

        


        //------------------------------------------------------------------------
        //*******************  SECTION INFORMATIONS ADMISSIOM  *******************
        //------------------------------------------------------------------------





        private void btnReinitialiserAdmission_Click(object sender, RoutedEventArgs e) {
            ReinitialiserInformationsAdmission();
        }

        private void btnEnregistrerAdmission_Click(object sender, RoutedEventArgs e) {
            

            // Condition pour valider que les champs obligatoire ont bien été rempli
            if (!ValidationChampsObligatoireAdmission()) {
                return;
            }

            // Enregistrement des entrées utilisateur pour construire l'objet Admission
            int idAdmission = int.Parse(txtIDAdmission.Text);
            DateTime dateAdmission = (DateTime)dateDateAdmission.SelectedDate;
            int nss = patient.NSS;
            int numeroLit = int.Parse(cboxNumeroLit.Text);
            bool chirurgieProgramme = false;
            bool televiseur = false;
            bool telephone = false;
            decimal tarifQuotidienLit;

            // Pour obtenir la propiété IDMedecin de l'objet anonyme obtenu avec le comboBox Medecin
            var medecin = cboxMedecinAdmission.SelectedItem;
            Type typeMedecin = medecin.GetType();
            int idMedecin = (int)typeMedecin.GetProperty("IDMedecin").GetValue(medecin, null);


            // Condition pour valider si une chirurgie est prévu lors de l'admission
            if (checkChirurgie.IsChecked == true) {
                chirurgieProgramme = true;

            // Condition pour valider si un téléviseur est prévu lors de l'admission
            }
            if (checkTeleviseur.IsChecked == true) {
                televiseur = true;
            }

            // Condition pour valider si un téléphone est prévu lors de l'admission
            if (checkTelephone.IsChecked == true) {
                telephone = true;
            }

            lit = TrouverObjetLit(numeroLit);
            tarifQuotidienLit = CalculerTarifQuotidienLit();
            MessageBox.Show(tarifQuotidienLit.ToString());

            // Try avec condition. Si une chirurgie est prévu ou non un différent constructeur sera appellé
            try {
                if (chirurgieProgramme == false) {
                    admission = new Admission(idAdmission, chirurgieProgramme, dateAdmission, null, televiseur, telephone, nss, numeroLit, idMedecin, tarifQuotidienLit);
                } else {
                    DateTime dateChirurgie = (DateTime)dateDateChirurgie.SelectedDate;
                    admission = new Admission(idAdmission, chirurgieProgramme, dateAdmission, dateChirurgie, televiseur, telephone, nss, numeroLit, idMedecin, tarifQuotidienLit);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " Erreur date chirurgie!");
            }

            // Modifier le status du lit choisi lors de l'admission
            lit.Occupe = true;
            myBd.Admissions.Add(admission);

            // Condition à l'aide d'un booléan qui évite de créer un nouveau patient dans la BD
            // si le patient existe déjà. Si ce n'est pas une première admission pour un même patient
            if (!patientDejaExistant) {
                myBd.Patients.Add(patient);
            }

            // Sauvegarde de la BD
            try {
                myBd.SaveChanges();
                MessageBox.Show("L'admission a été complété avec succès!",
                                "Admission complété", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
            }
            catch (Exception) {
                MessageBox.Show("Ce numéro d'assurance social existe déjà!",
                                "Numéro d'assurance social déjà existant",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            // Modification des booléan ou de l'accessibilité de certains contrôles de la fenêtre
            // suite à une nouvelle admission
            enregistrementPatient = false;
            gboxAdmission.IsEnabled = false;
            gboxPatient.IsEnabled = true;

            // Mise à jour des informations de certains contrôle de la fenêtre
            // suite à une nouvelle admission
            AfficherIDAdmission();
            ReinitialiserInformationPatient();
            ReinitialiserInformationsAdmission();

        }

        // Permet de calculer le tarif quotidien du lit
        private decimal CalculerTarifQuotidienLit() {
            int assurance = patient.IDAssurance;
            int departement = lit.IDDepartement;
            int typeLit = lit.IDTypeLit;
            bool litStandardDisponible = RechercherLitStandardDisponible(departement);
            bool litSemiPriveDisponible = RechercherLitSemiPriveDisponible(departement);

            if (assurance > 1) {
                return 0;
            }

            if (!litStandardDisponible && assurance == 1) {
                return 0;
            }

            if (!litSemiPriveDisponible && assurance == 1) {
                return 0;
            }

            if (litStandardDisponible && assurance == 1 && typeLit == 2) {
                return (decimal)267.00;
            }

            if (litStandardDisponible && assurance == 1 && typeLit == 3) {
                return (decimal)571.00;
            }

            return 0;
        }

        // Permet de chercher si un lit standard est disponible dans un département
        private bool RechercherLitStandardDisponible(int departement) {

            var query =
                (from l in myBd.Lits
                 where l.IDDepartement == departement
                 where l.IDTypeLit == 1
                 where l.Occupe == false
                 select l).Count();

            if (query > 0) {
                return true;
            }
            return false;
        }

        private bool RechercherLitSemiPriveDisponible(int departement) {

            var query =
                (from l in myBd.Lits
                 where l.IDDepartement == departement
                 where l.IDTypeLit == 2
                 where l.Occupe == false
                 select l).Count();

            if (query > 0) {
                return true;
            }
            return false;
        }

        // Permet de récupérer un objet lit de la BD à partir d'un numéro de lit
        private Lit TrouverObjetLit(int numeroLit) {
            var lit =
                (from l in myBd.Lits
                where l.NumeroLit == numeroLit
                select l).SingleOrDefault();

            return lit;
        }

        // Rend le contrôle selectDate disponible si la case chirurgie est coché
        private void checkChirurgie_Checked(object sender, RoutedEventArgs e) {
            dateDateChirurgie.IsEnabled = true;
            cboxDepartementLit.SelectedIndex = 0;
            cboxDepartementLit.IsEnabled = false;
            validerLitDisponibleDepartement(DEPARTEMENT_CHIRURGIE);

            if (validerLitDisponibleDepartement(DEPARTEMENT_CHIRURGIE) == true) {
                cboxDepartementLit.IsEnabled = false;
            }
            if (validerLitDisponibleDepartement(DEPARTEMENT_CHIRURGIE) == false) {
                cboxDepartementLit.IsEnabled = true;
            }
            if (validerLitDisponibleDepartement(DEPARTEMENT_CHIRURGIE) == false && agePatient <= 16) {
                cboxDepartementLit.IsEnabled = false;
                cboxDepartementLit.SelectedIndex = DEPARTEMENT_PEDIATRIE;
            }
        }

        // Rend le contrôle selectDate indisponible si la case chirurgie est décoché
        // et remet la date sélectionné à null
        private void checkChirurgie_Unchecked(object sender, RoutedEventArgs e) {
            dateDateChirurgie.IsEnabled = false;
            dateDateChirurgie.SelectedDate = null;
            dateDateChirurgie.DisplayDate = DateTime.Today;
            cboxDepartementLit.IsEnabled = true;
            if (agePatient <= 16) {
                cboxDepartementLit.SelectedIndex = DEPARTEMENT_PEDIATRIE;
                cboxDepartementLit.IsEnabled = false;
            }
        }

        // Permet de valider si un lit est disponible dans un département
        private bool validerLitDisponibleDepartement(int departement) {
            var query =
                (from l in myBd.Lits
                 where l.IDDepartement == departement
                 where l.Occupe == false
                 select l).Count();

            if (query == 0) {
                return true;
            }
            return false;
        }


        // Permet de vider les champs du formulaire des informations d'admission
        private void ReinitialiserInformationsAdmission() {
            dateDateAdmission.SelectedDate = null;
            dateDateAdmission.DisplayDate = DateTime.Today;
            cboxMedecinAdmission.SelectedIndex = -1;
            checkChirurgie.IsChecked = false;
            dateDateChirurgie.SelectedDate = null;
            dateDateChirurgie.DisplayDate = DateTime.Today;
            cboxDepartementLit.SelectedIndex = -1;
            cboxTypeLit.SelectedIndex = 0;
            cboxNumeroLit.DataContext = null;
            checkTeleviseur.IsChecked = false;
            checkTelephone.IsChecked = false;
        }

        // Permet de valider que les champs obligatoire du formulaire d'admission
        // ont bien été rempli
        private bool ValidationChampsObligatoireAdmission() {

            try {
                DateTime dateAdmission = (DateTime)dateDateAdmission.SelectedDate;
            }
            catch (Exception) {
                MessageBox.Show("La saisie de la date d'admission est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            if (cboxMedecinAdmission.SelectedIndex < 0) {
                MessageBox.Show("Le choix d'un médecin lors de l'admission est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            if ((bool)checkChirurgie.IsChecked) {
                try {
                    DateTime dateChirurgie = (DateTime)dateDateChirurgie.SelectedDate;
                }
                catch (Exception) {
                    MessageBox.Show("Si une chirurgie est prévu la date doit être saisie lors de l'admission.",
                                    "Attention",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return false;
                }
            }

            if (cboxDepartementLit.SelectedIndex < 0) {
                MessageBox.Show("Le choix du département est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            if (cboxTypeLit.SelectedIndex < 0) {
                MessageBox.Show("Le choix du type de lit est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            if (cboxNumeroLit.SelectedIndex < 0) {
                MessageBox.Show("Le choix du numéro de lit est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }
            return true;
        }





        //------------------------------------------------------------------------
        //********************  SECTION INFORMATIONS PATIENT  ********************
        //------------------------------------------------------------------------




        private void btnEnregistrerPatient_Click(object sender, RoutedEventArgs e) {

            // Condition servant à valider que les entrées utilisateurs soit bonne
            if (!ValidationEntreUtilisateuPatient()) {
                return;
            }

            // Condition qui vérifie que les champs obligatoire ont bien été remplis
            if (!ValidationChampsObligatoirePatient()) {
                return;
            }

            // Modification des booléan ou de l'accessibilité de certains contrôles de la fenêtre
            // suite à l'enregistrement d'un patient
            enregistrementPatient = true;
            gboxPatient.IsEnabled = false;

            // Une fois la fiche patient bien rempli la fiche d'admission devient accessible
            gboxAdmission.IsEnabled = true;


            // Enregistrement d'un objet patient. Les informations seront envoyés
            // à la BD seulement losque les informations de l'admission seront enregistrés
            int nss = int.Parse(txtAssuranceSocialAdmission.Text);
            DateTime dateNaissance = (DateTime)dateDateNaissanceAdmission.SelectedDate;
            string nom = txtNomAdmission.Text;
            string prenom = txtPrenomAdmission.Text;
            string adresse = txtAdresseAdmission.Text;
            string ville = txtVilleAdmission.Text;
            string province = cboxProvinceAdmission.Text;
            string codePostal = txtCodePostalAdmission.Text;
            string telephone = txtTelephoneAdmission.Text;
            int assureur = cboxAssuranceAdmission.SelectedIndex + 1;

            // Formattage des entrées utilisateur
            telephone = util.FormatterNumeroTelephone(telephone);
            codePostal = util.FormatterCodePostal(codePostal);

            patient = new Patient(nss, dateNaissance, nom, prenom, adresse, ville, province, codePostal, telephone, assureur);

            agePatient = CalculerAgePatient();
            if (agePatient <= 16) {
                cboxDepartementLit.SelectedIndex = DEPARTEMENT_PEDIATRIE;
                cboxDepartementLit.IsEnabled = validerLitDisponibleDepartement(DEPARTEMENT_PEDIATRIE);
            }
            
        }

        // Permet de déterminer l'age du patient
        private double CalculerAgePatient() {
            DateTime dateNaissance = (DateTime)dateDateNaissanceAdmission.SelectedDate;
            DateTime dateAujourdhui = DateTime.Today;

            return ((dateAujourdhui - dateNaissance).TotalDays) / 365.25;
        }

        // Avant de réinitialiser les informations du patient. Valide si l'utilisateur veut
        // réellement arrêter l'admission. Si c'est le cas rend de nouveau le groupBox
        // des informations de l'admission inaccessible
        private void btnReinitialiserPatient_Click(object sender, RoutedEventArgs e) {

            if (enregistrementPatient) {
                var reponse = MessageBox.Show("Êtes-vous de de vouloir réinitialiser les informations patient? \n" +
                                               "L'admission n'est pas complété et les informations devront être entrées à nouveau",
                                               "Attention",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Question);
                if (reponse == MessageBoxResult.Yes) {
                    ReinitialiserInformationPatient();
                    gboxAdmission.IsEnabled = false;
                    btnEnregistrerPatient.IsEnabled = true;
                    btnRechercherPatient.IsEnabled = true;
                    patientDejaExistant = false;
                } else {
                    return;
                }
            }

            // Modification des booléan ou de l'accessibilité de certains contrôles de la fenêtre
            enregistrementPatient = false;
            gboxPatient.IsEnabled = true;
            gboxAdmission.IsEnabled = false;
            ReinitialiserInformationPatient();
            ReinitialiserInformationsAdmission();
        }

        // Remet les champs d'entrée utlisateur vide
        private void ReinitialiserInformationPatient() {
            txtNomAdmission.Text = "";
            txtPrenomAdmission.Text = "";
            txtAssuranceSocialAdmission.Text = "";
            txtAdresseAdmission.Text = "";
            txtVilleAdmission.Text = "";
            txtCodePostalAdmission.Text = "";
            cboxProvinceAdmission.SelectedIndex = -1;
            txtTelephoneAdmission.Text = "";
            cboxAssuranceAdmission.SelectedIndex = -1;
            dateDateNaissanceAdmission.SelectedDate = null;
            dateDateNaissanceAdmission.DisplayDate = DateTime.Today;
        }

        // Permet de valider que les entrées de l'utilisateur de la fiche patient respectent les critères de la BD
        private bool ValidationEntreUtilisateuPatient() {

            if (!util.ValiderNom(txtNomAdmission.Text)) {
                MessageBox.Show("Le nom doit être composé de lettre, trait d'union (-) ou d'apostrophe(').",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtNomAdmission.Focus();
                txtNomAdmission.SelectAll();
                return false;
            }

            if (!util.ValiderNom(txtPrenomAdmission.Text)) {
                MessageBox.Show("Le prénom doit être composé de lettre, trait d'union (-) ou d'apostrophe(').",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtPrenomAdmission.Focus();
                txtPrenomAdmission.SelectAll();
                return false;
            }

            if (!util.ValiderNSS(txtAssuranceSocialAdmission.Text)) {
                MessageBox.Show("Le numéro d'assurance social doit être composé de 9 chiffres sans espace.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtAssuranceSocialAdmission.Focus();
                txtAssuranceSocialAdmission.SelectAll();
                return false;
            }

            if (!util.ValiderNumeroTelephone(txtTelephoneAdmission.Text)) {
                MessageBox.Show("Format acceptés pour le numéro de téléphone : (###)###-#### ou les 10 nombres sans espaces(##########).",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtTelephoneAdmission.Focus();
                txtTelephoneAdmission.SelectAll();
                return false;
            }

            if (!util.ValiderCodePostal(txtCodePostalAdmission.Text)) {
                MessageBox.Show("Format acceptés pour le code postal \n\n" +
                                "C#C C#C\n" +
                                "C#C#C#",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtCodePostalAdmission.Focus();
                txtCodePostalAdmission.SelectAll();
                return false;
            }

            return true;
        }


        // Permet de valider que tous les champs obligatoires de la fiche patient sont remplis
        private bool ValidationChampsObligatoirePatient() {

            if (txtNomAdmission.Text == "") {
                MessageBox.Show("La saisie du nom est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtNomAdmission.Focus();
                txtNomAdmission.SelectAll();
                return false;
            }

            if (txtPrenomAdmission.Text == "") {
                MessageBox.Show("La saisie du prénom est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtPrenomAdmission.Focus();
                txtPrenomAdmission.SelectAll();
                return false;
            }

            if (txtAssuranceSocialAdmission.Text == "") {
                MessageBox.Show("La saisie du numéro d'assurance social est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtAssuranceSocialAdmission.Focus();
                txtAssuranceSocialAdmission.SelectAll();
                return false;
            }

            if (cboxAssuranceAdmission.SelectedIndex < 0) {
                MessageBox.Show("Le choix d'assurance est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                cboxAssuranceAdmission.Focus();
                return false;
            }

            try {
                DateTime dateNaissance = (DateTime)dateDateNaissanceAdmission.SelectedDate;
            }
            catch (Exception) {
                MessageBox.Show("La saisie de la date de naissance est obligatoire.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                dateDateNaissanceAdmission.Focus();
                return false;
            }
            return true;
        }




        //-------------------------------------------------------------
        // Fonctions pour peupler les différents contrôles du formulaire
        //-------------------------------------------------------------



        // Pour peupler le comboBox du choix de médecin lors de l'admission
        private void ListeMedecin() {
            var query =
                from m in myBd.Medecins
                where m.Statut == true
                select new { m.IDMedecin, m.NomMedecin, m.PrenomMedecin };

            cboxMedecinAdmission.DataContext = query.ToList();
        }

        // Pour peupler le comboBox du choix de département du lit lors de l'admission
        private void AffichageDepartementLit() {
            cboxDepartementLit.DataContext = myBd.Departements.ToList();
        }

        private void AfficherIDAdmission() {
            // Permet de trouver le dernier IDAdmission de la base de donnée. Ajoute 1 pour afficher 
            // le IDAdmission de la prochaine admission qui sera ajoutée
            int query =
                myBd.Admissions
                    .OrderByDescending(a => a.IDAdmission)
                    .Select(a => a.IDAdmission)
                    .First();
            query++;
            txtIDAdmission.Text = query.ToString();
        }

        // Pour peupler le comboBox du type de lit lors de l'admission
        private void AfficherTypeLitAdmission() {
            cboxTypeLit.DataContext = myBd.TypeLits.ToList();
        }

        // Permet de peupler le comboBox du numéro de lit selon le choix de département et de type de lit
        private void btnAfficherLitDisponible_Click(object sender, RoutedEventArgs e) {
            string departementLit = cboxDepartementLit.Text;
            string typeLit = cboxTypeLit.Text;

            var query =
                from l in myBd.Lits
                join d in myBd.Departements on l.IDDepartement equals d.IDDepartement
                join t in myBd.TypeLits on l.IDTypeLit equals t.IDTypeLit
                where d.NomDepartement == departementLit
                where t.description == typeLit
                where l.Occupe == false
                select new { l.NumeroLit };

            cboxNumeroLit.DataContext = query.ToList();
            cboxNumeroLit.IsEnabled = true;
        }







        //-----------------------------------------------------------------------
        //********************   SECTION RECHERCHE PATIENT   ********************
        //-----------------------------------------------------------------------


        // Pour l'affichage de la liste des patient avec un DataContext
        public void RafraichirGridPatient() {
            var query =
                from p in myBd.Patients
                join a in myBd.Assurances on p.IDAssurance equals a.IDAssurance
                select new { p.NSS, p.DateNaissance, p.Nom, p.Prenom, p.Adresse, p.Ville, p.Province, p.CodePostal, p.telephone, a.NomCompagnie };

            dataPatients.DataContext = query.ToList();
        }

        private void btnDeconnecter_Click(object sender, RoutedEventArgs e) {
            Accueil accueil = new Accueil();
            Close();
            accueil.ShowDialog();
        }


        // Recherche d'un patient selon les information entré par l'utilisateur
        // Permet de rechercher avec des informations parielles
        private void btnRechercher_Click(object sender, RoutedEventArgs e) {
            string nom = txtNom.Text;
            string prenom = txtPrenom.Text;
            string NSS = txtNumAssSocial.Text;
            string telephone = txtTelephone.Text;
            string adresse = txtAdresse.Text;
            string ville = txtVille.Text;
            string province = txtProvince.Text;
            string codePostal = txtCodePostal.Text;
            string assureur = txtAssureur.Text;
            //DateTime dateNaiss = (DateTime)dateNaissance.SelectedDate;


            var query =
                from p in myBd.Patients
                join a in myBd.Assurances on p.IDAssurance equals a.IDAssurance
                where p.Nom.Contains(nom) &&
                p.Prenom.Contains(prenom) &&
                p.NSS.ToString().Contains(NSS) &&
                p.telephone.Contains(telephone) &&
                p.Adresse.Contains(adresse) &&
                p.Ville.Contains(ville) &&
                p.Province.Contains(province) &&
                p.CodePostal.Contains(codePostal) &&
                a.NomCompagnie.Contains(assureur)
                select new { p.NSS, p.DateNaissance, p.Nom, p.Prenom, p.Adresse, p.Ville, p.Province, p.CodePostal, p.telephone, a.NomCompagnie };

            dataPatients.DataContext = query.ToList();
        }

        private void btnReinitialiser_Click(object sender, RoutedEventArgs e) {
            txtNom.Text = "";
            txtPrenom.Text = "";
            txtNumAssSocial.Text = "";
            txtTelephone.Text = "";
            txtAdresse.Text = "";
            txtVille.Text = "";
            txtProvince.Text = "";
            txtCodePostal.Text = "";
            txtAssureur.Text = "";
            dateNaissance.SelectedDate = null;
            dateNaissance.DisplayDate = DateTime.Today;
            RafraichirGridPatient();
        }

        private void btnRechercherPatient_Click(object sender, RoutedEventArgs e) {
            tabRecherchePatient.Focus();
        }

        private void btnSelectionner_Click(object sender, RoutedEventArgs e) {
            if (!RecuperationPatient()) {
                return;
            }
            tabAdmissionPatient.Focus();
            patientDejaExistant = true;
            RemplirChampsAdmissionPatient();
        }

        private bool RecuperationPatient() {
            // Vérifier qu'un patient a bien été sélectionner dans le dataGrid avant de faire une modification
            if (dataPatients.SelectedItems.Count != 1) {
                MessageBox.Show("Vous devez sélectionner un patient.",
                    "Selectionnez un patient",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }
            // Pour obtenir la propiété NSS de l'objet anonyme obtenu avec le selectedItem du dataGrid
            var ligneDataPatient = dataPatients.SelectedItem;
            Type type = ligneDataPatient.GetType();
            int nss = (int)type.GetProperty("NSS").GetValue(ligneDataPatient, null);

            // Pour obtenir la ligne de la BD correspondent au NSS du patient 
            patient =
                (from p in myBd.Patients
                 where p.NSS == nss
                 select p).SingleOrDefault();

            return true;
        }

        // Fonction pour remplir les champs du formulaire d'enregistrement d'un patient
        // déjà existant qui a été sélectionné dans la recherche des patients
        private void RemplirChampsAdmissionPatient() {
            txtAssuranceSocialAdmission.Text = patient.NSS.ToString();
            txtNomAdmission.Text = patient.Nom.Trim();
            txtPrenomAdmission.Text = patient.Prenom.Trim();
            dateDateNaissanceAdmission.SelectedDate = patient.DateNaissance;
            txtAdresseAdmission.Text = patient.Adresse.Trim();
            txtVilleAdmission.Text = patient.Ville.Trim();
            txtCodePostalAdmission.Text = patient.CodePostal.Trim();
            cboxProvinceAdmission.Text = patient.Province.Trim();
            txtTelephoneAdmission.Text = patient.telephone.Trim();
            cboxAssuranceAdmission.SelectedIndex = patient.IDAssurance - 1;
            cboxDepartementLit.IsEnabled = true;
        }

    }
}
