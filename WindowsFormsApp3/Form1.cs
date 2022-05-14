using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        string path = null;//переменная для хранения пути заданной директории
        SaveOptions options = new SaveOptions() { Path = new List<string>(), NameFile = new List<string>() };// экземпляр класса сохранения параметров введеных пользователем
        BinaryFormatter binFornatter = new BinaryFormatter();//экземпляр класса сериализации данных
        Regex regex;//шаблон имени файла который ищем
        int result;//переменная кол-ва найденных файлов
        int all;// перемнная "всего файлов"
        DateTime date1;//экземпляр класса для отчета времени от начала поиска
        bool block = true;// нужна для запрета повторного использования
        string directory;// переменная для хранеия текущей дирректории
        TreeNode treeNode2 = new TreeNode();//экземпляр текущего узла  в TreeView
        int i = 0;//переменная для хранения индекса папки 
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())//открываем окно для выборра папки
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;//сохраняем путь папки в переменную path
                    comboBox1.Text = path;//выводим ее на экран
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Пауза")//когда программа на паузе
            {
                timer2.Stop();
                timer1.Stop();
                button2.Text = "Продолжить";
                return;
            }
            else if (button2.Text == "Продолжить")//когда сняли паузу
            {
                timer2.Start();
                timer1.Start();
                button2.Text = "Пауза";
                return;
            }

            ClearComponents();//метод для обнудение исходных переменных

            bool c = (String.Compare(comboBox1.Text, "Введитe путь") != 0);//блокировка дальнейшего хода
            bool d = String.Compare(comboBox2.Text, "Введитe имя файла") != 0;//блокировка дальнейшего хода

            if (c && d)//срабатывает если пользователь не ввел данных
            {
                options.Path.Add(comboBox1.Text);
                options.NameFile.Add(comboBox2.Text);// добавляем введеные параметры для последующего сохранения

                int w = comboBox1.Items.Contains(comboBox1.Text) ? 0 : comboBox1.Items.Add((comboBox1.Text));
                w = comboBox2.Items.Contains(comboBox2.Text) ? 0 : comboBox2.Items.Add((comboBox2.Text));// сохраняем запросы пользвателя (времено)


                if (checkBox1.Checked == true)
                {
                    regex = new Regex(comboBox2.Text + "(.*)");//задаем шаблон для поиска
                }
                else
                {
                    regex = new Regex($"^{comboBox2.Text}[.]\\w+$");//задаем шаблон для поиска
                }

                directory = comboBox1.Text;// присваеваем переменной путь поиска
                button2.Text = "Пауза";
                button3.Enabled = true;// разблокируем кнопку "Стоп"

                timer1.Start();//основной таймер поиска (на каждый тик ищет совпадения)
                timer2.Start();// таймер для отсчета времени от начала поиска
            }
        }

        //async Task ResultAsync(string path, TreeNode treeNode)
        //{
        //    await Task.Run(() => GetDirdirectory(path, treeNode));
        //    await Task.Run(() => GetFiles(path, treeNode));
        //    timer1.Stop();
        //    button2.Text = "Начать поиск";
        //}

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            comboBox1.Text = "";
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                comboBox1.Text = "Введитe путь";
            }
        }

        private void comboBox2_Enter(object sender, EventArgs e)
        {
            comboBox2.Text = "";
        }

        private void comboBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                comboBox2.Text = "Введитe имя файла";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;// блокируем кнопку стоп
            try
            {
                using (var stream = new FileStream("options.bin", FileMode.Open))//заргужаем прошлые запросы из файла "options.bin" 
                {
                    options = binFornatter.Deserialize(stream) as SaveOptions;
                }
                comboBox1.Items.AddRange(options.Path.ToArray());// выводим прошлые запросы пользвателя
                comboBox2.Items.AddRange(options.NameFile.ToArray());// выводим прошлые запросы пользвателя
            }
            catch
            { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (var stream = new FileStream("options.bin", FileMode.OpenOrCreate))// сохраняем все ввреденые запросы в файл  "options.bin"
            {
                options.NameFile = options.NameFile.GroupBy(g => g).Select(s => s.Key).ToList();
                options.Path = options.Path.GroupBy(g => g).Select(s => s.Key).ToList();
                binFornatter.Serialize(stream, options);
            }
        }

        //private void GetDirdirectory(string directory, TreeNode treeNodeView)
        //{
        //    string[] directories = Directory.GetDirectories(directory);
        //    for (int i = 0; i < directories.Length; i++)
        //    {
        //        Invoke((Action)(() => { Text = directories[i]; }));
        //        //Text = directories[i];
        //        DirectoryInfo info = new DirectoryInfo(directories[i]);
        //        TreeNode treeNode1 = new TreeNode().Nodes.Add(info.Name);
        //        Invoke((Action)(()=> { treeNodeView.Nodes.Add(treeNode1);}));
        //       // treeNodeView.Nodes.Add(treeNode1);
        //        GetDirdirectory(directories[i], treeNode1);
        //        GetFiles(directories[i], treeNode1);
        //    }
        //}
        private void GetFiles(string Text, TreeNode treeNodeView)// метод поиска файлов в определенной директории и добавления их  в TreeView
        {
            string[] files = Directory.GetFiles(Text);// получаем все файлы
            all += files.Length;//все файлы
            MatchCollection matches = null;// экземпляр успешный совпадений 
            string searchedfile;
            for (int i = 0; i < files.Length; i++)
            {
                searchedfile = Path.GetFileName(files[i]);//поичаем имя файла
                matches = regex.Matches(searchedfile);// примеряем шаблон поиска к имени файла
                if (matches.Count > 0)
                {
                    //Invoke((Action)(() => { treeNodeView.Nodes.Add(searchedfile); }));
                    treeNodeView?.Nodes.Add(searchedfile);//добавляем файл в TreeView
                    result++;
                    //Invoke((Action)(() => { label1.Text = $"Найдено {result} файлов из {all}"; }));
                    label1.Text = $"Найдено {result} файлов из {all}";// отображаем результат
                }
            }
            treeNode2 = treeNodeView?.Parent;// меняем ветвь на уровень ниже (возвращаемся назад)
            if (treeNodeView?.Nodes.Count == 0) // если недобавилось ни одного файла, удаляем ранее добавленную ветвь (каталог)
            {
                treeNode2 = treeNodeView.Parent;// меняем ветвь на уровень ниже (возвращаемся назад)
                //Invoke((Action)(() => { treeNodeView.Remove(); }));
                treeNodeView.Remove();
            }
        }

        //private int Allfiles(string path)
        //{
        //    int i = 0;
        //    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
        //    if (directoryInfo.Exists)
        //    { 
        //        i = directoryInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories).Length;
        //    }
        //    return i;
        //}

        private void timer1_Tick(object sender, EventArgs e)// основной таймер
        {
            if (treeNode2 == null)//выполняется когда закочились папки
            {
                Stop();
                return;
            }
            var r = directory.ToCharArray().Last() == '\\' ? directory : directory += "\\";// в конце каждогшо пути на папку добавляется \\ потому что метод Directory.GetDirectories("С:") не сработает, а Directory.GetDirectories("С:\\") выполнится
            string[] directories = Directory.GetDirectories(directory);// получаем все директории, находящиеся в паппке
            DirectoryInfo info;
            if ((directories.Length != 0 && directories.Length > i) || block)
            {
                if (block)//фрагмент кода который нужно выполнить один раз (добавляет первую, родительскую папку в TreeView)
                {
                    block = false;
                    info = new DirectoryInfo(directory);
                    treeNode2 = new TreeNode().Nodes.Add(info.Name);
                    treeView1.Nodes.Add(treeNode2);
                }
                else
                {
                    try
                    {
                        directory = directories[i];// выбираем определенную папку и индексом i 

                        DirectoryInfo info1 = new DirectoryInfo(directory);
                        TreeNode treeNode = treeNode2.Nodes.Add(info1.Name);
                        treeNode2 = treeNode;// добавляем папку в TreeView
                        i = 0;// обнуляем индекс (на случай если в папке есть другие папки)
                        Text = info1.Name;// выводим текущую папку поиска
                    }
                    catch { }
                }
            }// с помощью этого кода мы идем вглубь по папкам (пока не наткнемся на папку без вложенных  папок)


            else// выполняется когда в рассматриваемой папке нет вложенных папок
            {
                GetFiles(directory, treeNode2);//запускаем метод поиска файлов в папке
                Regex reg = new Regex(@"[^\\]+\\+$");// шаблон для удаления последнего каталога в пути к папке (чтобы переместиться на уровеь ниже)
                string aaa = directory;//прошлая дирректория 
                directory = reg.Replace(directory, "");// дирректория на уровень ниже
                string[] dir = Directory.GetDirectories(directory);// получаем каталоги, находящиеся в дирректории на уровень ниже
                i = Array.IndexOf(dir, aaa.Remove(aaa.Length - 1));// индекс прошлого каталога (чтобы был порядок)(когда будет новый тик в массиве "directories" выберится следующая папка)
                i++;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void ClearComponents()// метод обнудения исходных переменных
        {
            treeView1.Nodes.Clear();
            date1 = new DateTime(0, 0);
            block = true;
            result = 0;
            i = 0;
            treeNode2 = new TreeNode();
            all = 0;
        }
        private void Stop()//метод остановки программы
        {
            label1.Text = $"Найдено {result} файлов из {all}";
            timer1.Stop();
            timer2.Stop();
            button2.Text = "Начать поиск";
            button3.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)//метод подсчета времени от начала поиска
        {
            date1 = date1.AddSeconds(1);
            label2.Text = date1.ToString("mm:ss");
        }
    }

}
