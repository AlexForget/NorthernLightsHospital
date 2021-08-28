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
    /// Interaction logic for AjouterMedecin.xaml
    /// </summary>
    public partial class AjouterMedecin : Window {
        NorthernLightsHospitalEntities myBd;

        public AjouterMedecin() {
            InitializeComponent();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void AjouterMedecin1_Loaded(object sender, RoutedEventArgs e) {
            myBd = new NorthernLightsHospitalEntities();
            AfficherIDMedecin();
        }


        private void btnAjouter_Click(object sender, RoutedEventArgs e) {
            string nom = txtNom.Text;
            string prenom = txtPrenom.Text;

            // Validation de l'entré dans le textBox nom médecin
            if (!util.ValiderNom(nom)) {
                MessageBox.Show("Le nom doit être composé de lettre, trait d'union uniquement(-) ou d'apostrophe(').",
                                "Attention", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                return;
            }
            // Validation de l'entré dans le textBox prénom médecin
            if (!util.ValiderNom(prenom)) {
                MessageBox.Show("Le nom doit être composé de lettre, trait d'union uniquement(-) ou d'apostrophe(').",
                                "Attention", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                return;
            }

            //Création de l'instance médecin à ajouter à la BD
            Medecin med = new Medecin {
                IDMedecin = int.Parse(txtIDMedecin.Text),
                NomMedecin = nom,
                PrenomMedecin = prenom,
                Statut = true
            };

            // Ajout à la BD du nouveau médecin et sauvegarde
            myBd.Medecins.Add(med);
            try {
                myBd.SaveChanges();
                MessageBox.Show("Nouveau médecin ajouté avec succès.",
                                "Ajout Médecin",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            Close();
        }

        private void txtGotFocus(object sender, RoutedEventArgs e) {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private void AfficherIDMedecin() {
            // Permet de trouver le dernier IDMedecin de la base de donnée. Ajoute 1 pour afficher 
            // le IDMedecin du prochain médecin qui sera ajouté
            myBd = new NorthernLightsHospitalEntities();
            int query =
                myBd.Medecins
                    .OrderByDescending(m => m.IDMedecin)
                    .Select(m => m.IDMedecin)
                    .First();
            query++;
            txtIDMedecin.Text = query.ToString();
        }
    }
}
