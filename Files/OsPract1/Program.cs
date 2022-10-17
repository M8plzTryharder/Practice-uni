using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace OsPract1
{
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Company {get; set; }
        public Person(string name, int age, string company)
        {
            Name = name;
            Age = age;
            Company = company;
        }
    }

    class Program
    {
        // Вывести информацию в консоль о логических дисках, именах, метке тома, размере и типе файловой системы.
        static void PrintDrives()
        {
            Console.WriteLine("1. Работа с дисками.");
            Console.WriteLine("Вывод информации о логических дисках, именах, метке тома, размере и типе файловой системы.");
            var drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                Console.WriteLine($"Имя: {drive.Name}");
                Console.WriteLine($"Тип: {drive.DriveType}");
                if (drive.IsReady)
                {
                    Console.WriteLine($"Метка тома: {drive.VolumeLabel}");
                    Console.WriteLine($"Размер: {drive.TotalSize}");
                    Console.WriteLine($"Тип Файловой Системы: {drive.DriveFormat}");
                }
                Console.WriteLine();
            }
        }

        // Сохранить файл
        static void SaveToFile(string path, string str)
        {
            using (var fs = File.OpenWrite(path))
            {
                var info =
                    new UTF8Encoding(true).GetBytes(str);

                fs.Write(info, 0, info.Length);
            }
        }

        // Прочитать файл
        static string ReadFromFile(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                byte[] b = new byte[fs.Length];
                UTF8Encoding temp = new UTF8Encoding(true);
                fs.Read(b, 0, b.Length);

                return temp.GetString(b);
            }
        }

        // Работа с файлами ( класс File, FileInfo, FileStream и другие)
        static void FilesWorker()
        {
            Console.WriteLine("2. Работа с файлами.");
            var path = "test.txt";
            Console.WriteLine("Запись строки \"Строка.\" в файл " + path + ".");
            SaveToFile(path, "Строка.");
            Console.WriteLine("Чтение строки из файла " + path + ":");

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не может быть прочитан");
                return;
            }

            Console.WriteLine(ReadFromFile(path));
            Console.WriteLine("Удаление файла " + path + ".");

            if (!File.Exists(path))
            {
                Console.WriteLine("файл не найден");
                return;
            }
            File.Delete(path);
        }

        // Работа с форматом JSON
        static void JsonWorker()
        {
            Console.WriteLine("3. Работа с форматом JSON.");
            Console.WriteLine("Чтение файла example.json.");

            var ex_j = ReadFromFile("example.json");
            var ex_p = JsonSerializer.Deserialize<Person>(ex_j);
            Console.WriteLine($"Имя человека из example.json: {ex_p.Name}");
            Console.WriteLine($"Возраст человека из example.json: {ex_p.Age}");
            Console.WriteLine($"Компания человека из example.json: {ex_p.Company}");

            var path = "test.json";
            Console.WriteLine("Создание нового объекта и запись в файл " + path + ".");

            Console.WriteLine("Введите имя человека:");
            var name = Console.ReadLine();
            if (name == null)
            return;

            Console.WriteLine("Введите компанию человека:");
            var company = Console.ReadLine();
            if (company == null)
            return;

            Console.WriteLine("Введите возраст человека:");

            int age = 0;
            try
            {
                age = Convert.ToInt32(Console.ReadLine());
            }
            catch(System.FormatException)
            {
                Console.WriteLine("Возраст может содержать только цифры!");
                return;
            }

            var p = new Person(name, age, company);

            string json = JsonSerializer.Serialize<Person>(p);
            SaveToFile(path, json);
            Console.WriteLine("Чтение файла " + path + ".");
            if (!File.Exists(path))
            {
                Console.WriteLine("файл не был сохранен");
                return;
            }
            Console.WriteLine(ReadFromFile(path));
            Console.WriteLine("Удаление файла " + path + ".");
            File.Delete(path);
        }

        // Работа с форматом XML
        static void XmlWorker()
        {
            Console.WriteLine("4. Работа с форматом XML.");
            var path = "example.xml";
            Console.WriteLine("Чтение файла " + path + ".");

            if (!File.Exists(path))
            {
                Console.WriteLine("Не найден файл " + path + ".");
                return;
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            if (xDoc.DocumentElement == null)
                return;
            Console.WriteLine("Запись в файл " + path + " новых данных.");

            Console.WriteLine("Введите имя человека:");
            var new_name = Console.ReadLine();
            if (new_name == null)
            return;
                

            Console.WriteLine("Введите компанию человека:");
            var new_company = Console.ReadLine();
            if (new_company == null)
            return;            

            Console.WriteLine("Введите возраст человека:");

            int new_age;
            try
            {
                new_age = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Возраст может содержать только цифры!");
                return;
            }

            XmlElement new_elem = xDoc.CreateElement("person");
            XmlAttribute new_attr = xDoc.CreateAttribute("name");
            new_attr.Value = new_name;
            new_elem.Attributes.Append(new_attr);

            XmlElement new_comp_elem = xDoc.CreateElement("company");
            new_comp_elem.InnerText = new_company;
            new_elem.AppendChild(new_comp_elem);

            XmlElement new_age_elem = xDoc.CreateElement("age");
            new_age_elem.InnerText = new_age.ToString();
            new_elem.AppendChild(new_age_elem);

            xDoc.DocumentElement.AppendChild(new_elem);
            xDoc.Save(path);

            Console.WriteLine("Чтение файла " + path + " в консоль.");
            var persons = new List<Person>();

            foreach (XmlNode xnode in xDoc.DocumentElement)
            {
                var new_p = new Person("", 0, "");
                if (xnode.Name != "person" ||
                    xnode.Attributes == null || xnode.Attributes.Count == 0)
                    continue;

                var name_attr = xnode.Attributes.GetNamedItem("name");
                if (name_attr == null || name_attr.Value == null)
                    continue;

                new_p.Name = name_attr.Value;

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "company")
                        new_p.Company = childnode.InnerText;
                    try
                    {
                        if (childnode.Name == "age")
                            new_p.Age = Convert.ToInt32(childnode.InnerText);
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                }

                persons.Add(new_p);
            }
            

            foreach (var p in persons)
            {
                Console.WriteLine($"Имя человека: {p.Name}");
                Console.WriteLine($"Возраст человека: {p.Age}");
                Console.WriteLine($"Компания человека: {p.Company}");
            }

            var pathcopy = "example_copy.xml";
            Console.WriteLine("Копирование файла " + path + " в " + pathcopy +" для показа удаления XML файла.");
            File.Copy(path,pathcopy);
            Console.WriteLine("Удаление файла " + pathcopy + ".");
            File.Delete(pathcopy);
        }
        // Сжатие файла в Zip
        static void Compress(string sourceFile, string compressedFile)
        {
            using (var sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                using (var targetStream = File.Create(compressedFile))
                {
                    using (var compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                        Console.WriteLine($"Сжат файл: {sourceFile} -> {compressedFile}. Исходный размер: {sourceStream.Length}, сжатый размер: {targetStream.Length}.");
                    }
                }
            }
        }
        // Восстановление файла из Zip
        static void Decompress(string compressedFile, string targetFile)
        {
            // поток для чтения из сжатого файла
            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
            {
                // поток для записи восстановленного файла
                using (FileStream targetStream = File.Create(targetFile))
                {
                    // поток разархивации
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                        Console.WriteLine($"Восстановлен файл: {compressedFile} -> {targetFile}. Исходный размер: {sourceStream.Length}, восстановленный размер: {targetStream.Length}.");
                    }
                }
            }
        }

        // Создание zip архива, добавление туда файла, определение размера архива
        static void ZipWorker()
        {
            Console.WriteLine("5. Работа с Zip.");
            // Создание zip архива, добавление туда файла, определение размера архива
            Console.WriteLine("Создание Zip test-zip.zip и добавление в него test-zip.txt.");
            Compress("test-zip.txt", "test-zip.zip");

            Console.WriteLine("Создание файла out-zip.zip и добавление в него произвольного файла.");
            // Создать архив в формате zip
            // Добавить файл, выбранный пользователем, в архив
            Console.WriteLine("Введите имя файла для сжатия:");
            var f_name = Console.ReadLine();
            if (f_name == null)
            {
                Console.WriteLine("введено пустое имя файла");
                return;
            }

            Compress(f_name, "out-zip.zip");

            Console.WriteLine("Разархивирование out-zip.zip и вывод данных о нем.");
            Decompress("out-zip.zip", "uncomp_" + f_name);

            Console.WriteLine("Удаление файла и архива.");
            // Удалить файл и архив
            File.Delete("out-zip.zip");
            File.Delete("uncomp_" + f_name);
        }

        static void Main(string[] args)
        {
            PrintDrives();
            FilesWorker();
            JsonWorker();
            XmlWorker();
            ZipWorker();
        }
    }
}