using System;
using System.Windows;
using PdfiumViewer;
using System.Drawing.Printing;
using Microsoft.Win32; // Пространство имен для WPF OpenFileDialog

namespace PrintomatApp
{
    public partial class MainWindow : Window
    {
        private string selectedFilePath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалог для выбора файла (WPF OpenFileDialog)
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Выберите PDF файл для печати"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                txtFilePath.Text = selectedFilePath;
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Пожалуйста, выберите PDF файл для печати.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем количество копий
            if (!int.TryParse(txtCopies.Text, out int copies) || copies <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество копий (целое число больше нуля).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Получаем выбор пользователя для двусторонней печати
                bool duplex = rbDoubleSided.IsChecked == true;

                PrintPdfFile(selectedFilePath, duplex, copies);

                // Уведомление об успешной печати (по желанию)
                // MessageBox.Show("Файл отправлен на печать.", "Печать", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintPdfFile(string filePath, bool duplex, int copies)
        {
            using (var document = PdfDocument.Load(filePath))
            {
                using (var printDocument = document.CreatePrintDocument())
                {
                    // Подавляем диалоговые окна печати
                    printDocument.PrintController = new StandardPrintController();

                    // Настройки принтера
                    printDocument.PrinterSettings = new PrinterSettings()
                    {
                        // Укажите имя принтера, если требуется
                        // PrinterName = "Имя вашего принтера",
                        Copies = (short)copies, // Устанавливаем количество копий
                    };

                    // Настройка двусторонней печати
                    if (duplex)
                    {
                        printDocument.PrinterSettings.Duplex = Duplex.Vertical; // Двусторонняя печать по длинному краю
                    }
                    else
                    {
                        printDocument.PrinterSettings.Duplex = Duplex.Simplex; // Односторонняя печать
                    }

                    // Отправляем на печать
                    printDocument.Print();
                }
            }
        }
    }
}