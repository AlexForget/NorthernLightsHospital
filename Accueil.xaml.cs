/*
    @Accueil.xaml.cs    
    @ Projet : NorthernLightsHospital
    @ auteur : Alexandre Forget
    @ contact : alexandreqc26@gmail.com
    @ date 28/08/2021
    @ Projet scolaire. Programme de gestion des admissions des patients dans un hopital
*/




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
    /// Interaction logic for Accueil.xaml
    /// </summary>
    public partial class Accueil : Window {
        NorthernLightsHospitalEntities myBd;

        public Accueil() {
            InitializeComponent();
        }

        private void Login_Loaded(object sender, RoutedEventArgs e) {
            myBd = new NorthernLightsHospitalEntities();
        }

        private void btnConnexion_Click(object sender, RoutedEventArgs e) {
            string nomUtilisateur = txtNomUtilisateur.Text;
            string motDePasse = txtMotDePasse.Password;

            // Validation qu'un nom d'utilisateur a bien été entré par l'utilisateur
            if (nomUtilisateur == "") {
                MessageBox.Show("Vous devez saisir un nom d'utilisateur",
                                "Invalide",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtNomUtilisateur.Focus();
                return;
            }

            // Validation qu'un mot de passe a bien été entré par l'utilisateur
            if (motDePasse == "") {
                MessageBox.Show("Vous devez saisir un mot de passe",
                                "Invalide",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                txtMotDePasse.Focus();
                return;
            }

            // Validation de l'utilisateur admin
            if (nomUtilisateur.Equals("admin") && motDePasse.Equals("admin")) {
                FenetreAdmin admin = new FenetreAdmin();
                Close();
                admin.ShowDialog();
                return;
            }

            // Validation de l'utilisateur prepose
            if (nomUtilisateur.Equals("prepose") && motDePasse.Equals("prepose")) {
                FenetrePrepose prepose = new FenetrePrepose();
                Close();
                prepose.ShowDialog();
                return;
            }

            // Validation de l'utilisateur medecin selon sonIDMedecin
            if (!int.TryParse(nomUtilisateur, out int idMedecin)) {
                MessageBox.Show("Nom d'utilisateur et/ou mot de passe invalide",
                                "Invalide",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (ValiderIDMedecin(idMedecin) && motDePasse.Equals("medecin")) {
                DonnerConge donnerConge = new DonnerConge(idMedecin);
                Close();
                donnerConge.ShowDialog();
                return;
            } else {
                MessageBox.Show("Nom d'utilisateur et/ou mot de passe invalide",
                                "Invalide",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        // Recherche si le IDMedecin entré à l'accueil est valide
        private bool ValiderIDMedecin(int idMedecin) {
            var query =
                from m in myBd.Medecins
                 where m.IDMedecin == idMedecin
                 select new {m.IDMedecin };

            if (query != null) {
                return true;
            }
            return false;
        }

        
    }
}
