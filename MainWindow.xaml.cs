using SberPank.View;

using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Path = System.IO.Path;


namespace SberPank
{
    public partial class MainWindow : Window
    {
        List<Field> fields;
        List<FieldModelView> fieldModelViews;

        Task parseTask;
        bool inParse = false;

        string dirPath = "";
        string outPath = "";
        bool doLog = true;

        int startLine = 1;

        string[] parseLog = { "", "" };

        private readonly OpenFileDialog openFileDialog; 
        private readonly FolderBrowserDialog selectFolderDialog;
        private readonly SaveFileDialog savePatternDialog;
        private readonly OpenFileDialog openPatternDialog;
        private readonly OpenFileDialog openExelFileDialog;

        public MainWindow()
        {
            InitializeComponent();

            selectFolderDialog = new FolderBrowserDialog();
            savePatternDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            openPatternDialog = new OpenFileDialog();
            openExelFileDialog = new OpenFileDialog();

            ViewHelp.setTBNumericOnly(startLineTB);
            startLineTB.TextChanged += (e, sender) => { startLine = int.Parse(startLineTB.Text); };

            textBox_folderPath.IsReadOnly = true;
            textBox_filePath.IsReadOnly = true;

            fields = new List<Field>();
            fieldModelViews = new List<FieldModelView>();

            savePatternDialog.DefaultExt = "pank";
            openPatternDialog.Filter = "Шаблон парсера (*.pank)|*.pank";
            openExelFileDialog.Filter = " Exel (*.xlsx)|*.xlsx";

            doLogCB.Checked += (sender, e) => { doLog = true; };
            doLogCB.Unchecked += (sender, e) => { doLog = false; };

            startAnimatronics();
        }

        //---------------------------------------  Paths

        void setDirectory(string path)
        {
            dirPath = path;
            textBox_folderPath.Text = dirPath;
        }
        private void outFolderSetBut_Click(object sender, RoutedEventArgs e)
        {
            if (selectFolderDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            setDirectory(selectFolderDialog.SelectedPath);
        }

        void setExelPath(string path)
        {
            outPath = path;
            textBox_filePath.Text = path;
        }
        private void outFileSetBut_Click(object sender, RoutedEventArgs e)
        {
            if (openExelFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            setExelPath(openExelFileDialog.FileName);
        }
        //--------------------------------------- 

        //---------------------------------------  pattern Save Load
        void savePattern(string path)
        {
            string patternContent = "";
            foreach (var field in fields)
            {
                patternContent += field.name + "ໃ";
                patternContent += field.From + "ໃ";
                patternContent += field.To + "ໃ";
                patternContent += field.index + "ໃ";
                patternContent += field.offset + "ໃ";
                patternContent += field.checkRepeats + "ໃ";
                patternContent += field.optional + "ໃ";
                patternContent += field.cellChars + "\n";
            }

            File.WriteAllText(path, patternContent);
        }
        private void saveTemplateBut_Click(object sender, RoutedEventArgs e)
        {
            if (savePatternDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            savePattern(savePatternDialog.FileName);
        }

        void loadPattern(string path)
        {
            removeFields();
            foreach (string str in File.ReadAllLines(path))
            {
                string[] fieldParams = str.Split('ໃ');
                var field = new Field(0);
                field.name = fieldParams[0];
                field.From = fieldParams[1];
                field.To = fieldParams[2];
                field.index = int.Parse(fieldParams[3]);
                field.offset = int.Parse(fieldParams[4]);
                field.checkRepeats = bool.Parse(fieldParams[5]);
                field.optional = bool.Parse(fieldParams[6]);
                field.cellChars = fieldParams[7];

                fields.Add(field);
            }

            writeFields();
        }
        private void loadTemplateBut_Click(object sender, RoutedEventArgs e)
        {
            if (openPatternDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            loadPattern(openPatternDialog.FileName);
        }
        //---------------------------------------



        //---------------------------------------  openFile

        private void viewMessageVeiewer(string[] content)
        {
            MesageView mw = new MesageView(content);
            mw.Show();
        }

        private void openFile(string filePath)
        {
            string[] info = MessageConverter.convertToMessage(filePath);
            viewMessageVeiewer(info);
        }

        private void openFileBut_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            openFile(openFileDialog.FileName);
        }
        //--------------------------------------- 


        //--------------------------------------- Fileds

        void removeFields()
        {
            fields.Clear();
            foreach (var afieldMV in fieldModelViews)
                fieldsPanel.Children.Remove(afieldMV);
            fieldModelViews.Clear();
        }

        void writeFields()
        {
            foreach (var afield in fields)
                createFieldModelView(afield);
        }

        void createFieldModelView(Field nField)
        {
            FieldModelView nFieldModelView = new FieldModelView(nField);
            fieldModelViews.Add(nFieldModelView);

            fieldsPanel.Children.Insert(fieldsPanel.Children.Count - 1, nFieldModelView);

            nFieldModelView.FieldRemoved += () => {
                fields.Remove(nField);
                fieldsPanel.Children.Remove(nFieldModelView);
                fieldModelViews.Remove(nFieldModelView);
            };
        }

        void addNewField()  
        {
            Field nField = new Field(fields.Count);
            fields.Add(nField);

            createFieldModelView(nField);
        }


        private void addFieldBut_Click(object sender, RoutedEventArgs e)
        {
            addNewField();
        }

        //---------------------------------------


        //--------------------------------------- Uploading

        string replaceSpec(string orig)
        {
            string n = orig.Replace("&quot;", "\"");
            n = n.Replace("&frasl;", "⁄");
            n = n.Replace("&times;", "×");
            n = n.Replace("&lt;", "<");
            n = n.Replace("&gt;", ">");
            n = n.Replace("&amp;", "&");
            n = n.Replace("&laquo;", "«");
            n = n.Replace("&raquo;", "»");
            n = n.Replace("&prime;", "′");
            n = n.Replace("&Prime;", "″");
            n = n.Replace("&lsquo;", "‘");
            n = n.Replace("&rsquo;", "’");
            n = n.Replace("&sbquo;", "‚");
            n = n.Replace("&ldquo;", "“");
            n = n.Replace("&rdquo;", "”");
            n = n.Replace("&bdquo;", "„");
            n = n.Replace("&copy;", "©");
            n = n.Replace("&trade;", "®");
            return n;
        }

        void Upload()
        {

            Workbook workbook = new Workbook();
            try { workbook.LoadFromFile(outPath); }
            catch { System.Windows.MessageBox.Show("Не удалось получить доступ к файлу Excel"); return; }

            Worksheet worksheet = workbook.Worksheets[0];

            string succsesDir = Path.Combine(dirPath, "Парсинг успешен");
            string failDir = Path.Combine(dirPath, "Парсинг провален");

            if (!Directory.Exists(succsesDir)) { Directory.CreateDirectory(succsesDir); }
            if (!Directory.Exists(failDir)) { Directory.CreateDirectory(failDir); }

            parseLog = new[] { "Лог парсинга", "" };

            string[] workFiles = Directory.GetFiles(dirPath);

            int workFileIndex = 0;

            // -------------------- База повторов

            Dictionary<string, Dictionary<string, bool>> repeats = new Dictionary<string, Dictionary<string, bool>>();
            foreach (var fld in fields)
            {
                if (fld.checkRepeats)
                {
                    repeats.Add(fld.name, new Dictionary<string, bool>());
                }
            }


            // -------------------- Для каждого файла

            foreach (var workfilePath in workFiles)
            {
                workFileIndex += 1;
                string[] message = MessageConverter.convertToMessage(workfilePath);
                string messageContent = message[1];

                if (doLog) parseLog[1] += "___________________________________\nФайл: " + message[0] + "\n";

                string[] cells = new string[fields.Count];
                string[] values = new string[fields.Count];

                bool error = false;

                int strL = messageContent.Length;

                // -------------------- Для каждого поля

                for (int i = 0; i < fields.Count; i++)
                {
                    values[i] = "";

                    Field fld = fields[i];

                    cells[i] = fld.cellChars;

                    int start = -1;
                    for (int j = 0; j < fld.index; j++)
                    {
                        start += 1;
                        start = messageContent.IndexOf(fld.From, start);
                    }
                    int end = messageContent.IndexOf(fld.To, start + fld.From.Length);

                    if ((start == -1) || (end == -1))
                    {
                        if (fld.optional)
                        {
                            if (doLog) parseLog[1] += "[ _ ] Прорущено поле " + fld.name + "\n";
                            continue;
                        }
                        error = true;
                        if (doLog) parseLog[1] += "[ X ] Ошибка в поле " + fld.name + "\n";
                        break;
                    }

                    start += fld.From.Length + fld.offset;

                    values[i] += messageContent.Substring(start, end - start);
                    fileProgress.Dispatcher.Invoke(() => {
                        fieldProgressLabel.Content = "ПОЛЕ " + (i+1) + "/" + fields.Count;
                        fieldProgress.Value = i+1;
                    });
                }

                if (doLog)
                {
                    parseLog[1] += "\n";
                    for (int i = 0; i < values.Length; i++)
                    {
                        parseLog[1] += "->" + values[i] + "\n";
                    }
                }

                // -------------------- Инфа с файла выгружена

                if (error)
                {
                    File.Move(workfilePath, Path.Combine(failDir, Path.GetFileName(workfilePath)));
                }
                else
                {
                    try
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if ( (values[i] == "") && fields[i].optional) continue;

                            values[i] = replaceSpec(values[i]);

                            string cell = cells[i] + startLine;
                            worksheet.Range[cell].Text = values[i];
                            worksheet.Range[cell].Style.Color = System.Drawing.Color.White;

                            if (fields[i].checkRepeats)
                            {
                                Dictionary<string, bool> rep;
                                repeats.TryGetValue(fields[i].name, out rep);

                                bool b;
                                bool exist = rep.TryGetValue(values[i], out b);

                                if (exist) {
                                    worksheet.Range[cell].Style.Color = System.Drawing.Color.LightYellow;
                                    if (doLog) parseLog[1] += "[R] Повтор в поле " + fields[i].name + "\n";
                                }
                                else
                                    rep.Add(values[i], true);
                            }
                        }
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Ошибка записи данных...");
                        return;
                    }

                    startLine += 1;
                    File.Move(workfilePath, Path.Combine(succsesDir, Path.GetFileName(workfilePath)));
                }

                fileProgress.Dispatcher.Invoke(() =>
                {
                    fileProgressLabel.Content = "ФАЙЛ " + workFileIndex + "/" + workFiles.Length;
                    fileProgress.Value = workFileIndex;
                });

            }

            // -------------------- Файлы обработаны

            try
            { 
                workbook.SaveToFile(outPath, ExcelVersion.Version2016);
                System.Windows.MessageBox.Show("Парсинг завершен");
            }
            catch {
                System.Windows.MessageBox.Show("Парсинг завершен, но программе не удалось получить доступ файлу *(файл открыт?). Результат записан в новый файл.");
                string nFilePath = outPath.Substring(0, outPath.Length - 5) + "_Parsed.xlsx";
                workbook.SaveToFile(nFilePath, ExcelVersion.Version2016);
            }
            inParse = false;
            this.Dispatcher.Invoke(() => StopParsing());
        }

        private void StartParsing()
        {
            if ((!File.Exists(outPath)) ||
               (!Directory.Exists(dirPath)) ||
               (fields.Count == 0))
            { return; }

            fileProgress.Maximum = Directory.GetFiles(dirPath).Length;
            fieldProgress.Maximum = fields.Count;

            AllGrid.IsEnabled = false;
            ProcessGrid.Visibility = Visibility.Visible;
            uploadBut.Content = "Прервать";
            uploadBut.Background = new SolidColorBrush(Color.FromRgb(157, 20, 6));

            inParse = true;
            parseTask = Task.Factory.StartNew(() => Upload());
        }

        private void StopParsing()
        {
            if (inParse) parseTask.Dispose();
            inParse = false;

            startLineTB.Text = startLine.ToString();

            if (doLog) viewMessageVeiewer(parseLog);

            AllGrid.IsEnabled = true;
            ProcessGrid.Visibility = Visibility.Collapsed;
            uploadBut.Content = "Начать выгрузку";
            uploadBut.Background = new SolidColorBrush(Color.FromRgb(6, 157, 28));
            uploadBut.UpdateLayout();
        }

        private void uploadClick(object sender, RoutedEventArgs e)
        {
            if(!inParse)
                StartParsing();
            else
                StopParsing();
        }
        //--------------------------------------- 






        //--------------------------------------- Window hanlders

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        //---------------------------------------

        //////////////////////////////////////////////////////////////////////
        /// Анимация
        //////////////////////////////////////////////////////////////////////
        private void startAnimatronics()
        {
            CompositionTarget.Rendering += perFrame;
            BackgroundLayer.Effect.SetValue(GlassnoEffect.TimeScaleProperty, 0.001d);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan((long)60);
            dispatcherTimer.Start();
        }

        private double frame = 0;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            frame += 0.0013;
        }

        private void perFrame(object sender, EventArgs e)
        {
            BackgroundLayer.Effect.SetValue(GlassnoEffect.TimeScaleProperty, frame);
        }


        //---------------------------------------------------------------- Drugs Drop
        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Link;
            var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            DragDropLabel.Content = "⮞ файлы (" + files.Length + ") ⮜";

            DragDropGrid.Visibility = Visibility.Visible;
        }
        private void Window_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            DragDropGrid.Visibility = Visibility.Collapsed;
        }
        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            DragDropGrid.Visibility = Visibility.Collapsed;

            var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                foreach (var filep in files)
                {
                    if (filep.Substring(filep.Length - 5) == ".pank")
                    {
                        loadPattern(filep);
                    }
                    else if (filep.Substring(filep.Length - 5) == ".xlsx")
                    {
                        setExelPath(filep);
                    }
                    else if (Directory.Exists(filep))
                    {
                        setDirectory(filep);
                    }
                    else
                    {
                        openFile(filep);
                    }
                }
            }
        }

        private void helpBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] readmeMessage = { "readme.txt", File.ReadAllText("readme.txt") };
                viewMessageVeiewer(readmeMessage);
            }
            catch
            {
                System.Windows.MessageBox.Show("Не удалось открыть readme файл. Куда вы его дели, э?");
            }
        }
    }
}
