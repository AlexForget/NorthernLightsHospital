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
    /// Interaction logic for ModifierMedecin.xaml
    /// </summary>
    public partial class ModifierMedecin : Window {
        NorthernLightsHospitalEntities myBd;
        public ModifierMedecin(Medecin medecin) {
            InitializeComponent();
            DataContext = medecin;
        }

        private void ModifierMedecin1_Loaded(object sender, RoutedEventArgs e) {
            myBd = new NorthernLightsHospitalEntities();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e) {
            string nom = txtNom.Text;
            string prenom = txtPrenom.Text;
            int idMedecin = int.Parse(txtIDMedecin.Text);

            // Validation de l'entré dans le textBox nom médecin
            if (!util.ValiderNom(nom)) {
                MessageBox.Show("Le nom doit être composé de lettre ou trait d'union uniquement.", "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Validation de l'entré dans le textBox prénom médecin
            if (!util.ValiderNom(prenom)) {
                MessageBox.Show("Le nom doit être composé de lettre ou trait d'union uniquement.",
                                "Attention",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // Pour obtenir la ligne de la BD correspondent au ID du médecin à moddifier
            Medecin medecin =
                (from m in myBd.Medecins
                 where m.IDMedecin == idMedecin
                 select m).SingleOrDefault();

            medecin.NomMedecin = nom.Trim();
            medecin.PrenomMedecin = prenom.Trim();

            // Sauvegarde des modifications apportées
            try {
                myBd.SaveChanges();
                MessageBox.Show("Les informations ont été modifiées avec succès.",
                                "Modification",
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
    }
}
