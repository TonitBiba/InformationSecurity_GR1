using InformationSecurity_GR1.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InformationSecurity_GR1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> keys;
        DecryptionAES decryptionAES;
        List<StatisticsCondition> statisticsConditions;

        public MainWindow()
        {
            InitializeComponent();
            statisticsConditions = new List<StatisticsCondition>();
            keys = new List<string>();
        }

        private async void BtnAttach_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".txt";
            fileDialog.Filter = "Text documents (.txt)|*.txt";
            if (fileDialog.ShowDialog() == true)
            {
                txtKeyFile.Text = fileDialog.SafeFileName;
                await Task.Factory.StartNew(() =>
                {
                    keys = loadKeys(fileDialog.FileName);
                }).ContinueWith(T =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        lstKeysAttached.ItemsSource = keys;
                    });
                });

                decryptionAES = new DecryptionAES(keys, txtEncryptedTekst.Text);
            }
        }

        private async void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            ResultOfDecryption decryptedResult = new ResultOfDecryption();
            if (String.IsNullOrEmpty(txtEncryptedTekst.Text) || String.IsNullOrWhiteSpace(txtEncryptedTekst.Text))
            {
                MessageBox.Show("Ju lutem vendosni tekstin e enkriptuar per te vazhduar me procesin e dekriptimit", "Teksti enkriptimit", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                DateTime startDate = DateTime.Now;
                prgState.IsIndeterminate = true;
                await Task.Factory.StartNew(() =>
                {
                    decryptedResult = decryptionAES.resultOfDecryption(lstGeneratedKeys);
                }).ContinueWith(T =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        txtPlaintext.Text = decryptedResult.PlainText;
                        txtSecretKey.Text = decryptedResult.Key;
                        lblEncryptedTime.Content = "Koha dekriptimit: " + DateTime.Now.Subtract(startDate).ToString(@"hh\:mm\:ss\:fff");
                        prgState.IsIndeterminate = false;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ka ndodhur nje gabim gjate dekriptimit.\n" + ex, "Gabim", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtEncryptedTekst_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                decryptionAES.CiperText = txtEncryptedTekst.Text.Trim();
            }
            catch { }
        }

        private void TxtFirstCondition_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (keys.Count == 0)
                {
                    MessageBox.Show("Ju lutem bashkengjitni vargun e çelësave për të vazhduar me vendosjen e kritereve.", "Celësat mungojnë", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string[] firstCondition = txtFirstCondition.Text.Split('>', '<').Select(S => S.Trim().ToUpper()).ToArray(); ;
                if (statisticsConditions.Where(SC => SC.conditionNumber == 1).Any())
                    statisticsConditions.Remove(statisticsConditions.Where(SC => SC.conditionNumber == 1).FirstOrDefault());
                if (firstCondition.Count() == 2)
                {
                    statisticsConditions.Add(new StatisticsCondition
                    {
                        condition = firstCondition,
                        sign = txtFirstCondition.Text.Contains('>') ? 1 : 2,
                        conditionNumber = 1
                    });
                    lstOfFilteredKeys.ItemsSource = decryptionAES.GetFilteredKeys(statisticsConditions).Select(S => DecryptionAES.ByteArrayToString(S));
                    lblNrFilteredKeys.Content = lstOfFilteredKeys.Items.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kushti shtypur nuk eshte ne formatin e duhur.\n" + ex, "Gabim", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSecondCondition_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (keys.Count == 0)
                {
                    MessageBox.Show("Ju lutem bashkengjitni vargun e çelësave për të vazhduar me vendosjen e kritereve.", "Celësat mungojnë", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string[] firstCondition = txtSecondCondition.Text.Split('>', '<').Select(S => S.Trim().ToUpper()).ToArray();
                if (statisticsConditions.Where(SC => SC.conditionNumber == 2).Any())
                    statisticsConditions.Remove(statisticsConditions.Where(SC => SC.conditionNumber == 2).FirstOrDefault());
                if (firstCondition.Count() == 2)
                {
                    statisticsConditions.Add(new StatisticsCondition
                    {
                        condition = firstCondition,
                        sign = txtSecondCondition.Text.Contains('>') ? 1 : 2,
                        conditionNumber = 2
                    });
                    lstOfFilteredKeys.ItemsSource = decryptionAES.GetFilteredKeys(statisticsConditions).Select(S => DecryptionAES.ByteArrayToString(S));
                    lblNrFilteredKeys.Content = lstOfFilteredKeys.Items.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kushti shtypur nuk eshte ne formatin e duhur.\n" + ex, "Gabim", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtThirdCondition_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (keys.Count == 0)
                {
                    MessageBox.Show("Ju lutem bashkengjitni vargun e çelësave për të vazhduar me vendosjen e kritereve.", "Celësat mungojnë", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string[] firstCondition = txtThirdCondition.Text.Split('>', '<').Select(S => S.Trim().ToUpper()).ToArray(); ;
                if (statisticsConditions.Where(SC => SC.conditionNumber == 3).Any())
                    statisticsConditions.Remove(statisticsConditions.Where(SC => SC.conditionNumber == 3).FirstOrDefault());
                if (firstCondition.Count() == 2)
                {
                    statisticsConditions.Add(new StatisticsCondition
                    {
                        condition = firstCondition,
                        sign = txtThirdCondition.Text.Contains('>') ? 1 : 2,
                        conditionNumber = 3
                    });
                    lstOfFilteredKeys.ItemsSource = decryptionAES.GetFilteredKeys(statisticsConditions).Select(S => DecryptionAES.ByteArrayToString(S));
                    lblNrFilteredKeys.Content = lstOfFilteredKeys.Items.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kushti shtypur nuk eshte ne formatin e duhur.\n" + ex, "Gabim", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<string> loadKeys(string fileName)
        {
            StreamReader streamReader = new StreamReader(fileName);
            return streamReader.ReadToEnd().Split(Environment.NewLine.ToCharArray()).Where(k => k != "").Select(K => K.ToUpper()).ToList();
        }

    }
}