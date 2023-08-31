using System;
using System.IO;
using System.Text.Json;
using Google.Protobuf;


class Program
{
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    static void Main()
    {
        string folderPath = "/Users/jc/ExampleFolder";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Console.WriteLine("文件夹已创建。");
        }
        else
        {
            Console.WriteLine("文件夹已经存在。");
            string filePath =  "/Users/jc/ExampleFolder/11111.txt";

            FileInfo fileInfo = new FileInfo(filePath);
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            Console.WriteLine("文件夹路径: " + fileInfo.DirectoryName);
            Console.WriteLine("文件名: " + fileInfo.Name);
            Console.WriteLine("扩展名: " + fileInfo.Extension);
            Console.WriteLine();
            
            Console.WriteLine("文件名: " + fileInfo.Name);
            Console.WriteLine("文件路径: " + fileInfo.FullName);
            Console.WriteLine("文件大小: " + fileInfo.Length + " 字节");
            Console.WriteLine("创建时间: " + fileInfo.CreationTime);
            Console.WriteLine("最后访问时间: " + fileInfo.LastAccessTime);
            Console.WriteLine("最后修改时间: " + fileInfo.LastWriteTime);
            Console.WriteLine();
            
            Console.WriteLine("文件夹名称: " + directoryInfo.Name);
            Console.WriteLine("文件夹路径: " + directoryInfo.FullName);
            Console.WriteLine("创建时间: " + directoryInfo.CreationTime);
            Console.WriteLine("最后访问时间: " + directoryInfo.LastAccessTime);
            Console.WriteLine("最后修改时间: " + directoryInfo.LastWriteTime);

            long size = GetDirectorySize(directoryInfo);
            Console.WriteLine("文件夹大小: " + size + " 字节");
            Console.WriteLine();

            WriteFile(filePath);
            ReadFile(filePath);
            
            BinaryWriteFile(filePath);
            BinaryReadFile(filePath);
            
            ReadFile(filePath);
            
            WriteJsonFile(filePath);
            ReadJsonFile(filePath);

            ReadFile(filePath);
            
            //WriteProtoBuf(filePath);
            //ReadProtoBuf(filePath);

        }
    }

    static long GetDirectorySize(DirectoryInfo directoryInfo)
    {
        long size = 0;
        FileInfo[] infos = directoryInfo.GetFiles();
        foreach (var info in infos)
        {
            size += info.Length;
        }

        return size;
    }

    static void WriteFile(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Append))
        {
            string content = "2222";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
            fileStream.Write(bytes, 0, bytes.Length);
        }
    }

    static void ReadFile(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            byte[] buffer = new byte[fileStream.Length];
            int byteRead = fileStream.Read(buffer, 0, buffer.Length);
            string content = System.Text.Encoding.UTF8.GetString(buffer, 0, byteRead);
            Console.WriteLine("读取的数据：" + content);
        }
    }

    static void BinaryWriteFile(string filePath)
    {
        Person person1 = new Person { Name = "A", Age = 30 };
        Person person2 = new Person { Name = "B", Age = 25 };
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            var writer = new BinaryWriter(fileStream);
            writer.Write(person1.Name);
            writer.Write(person1.Age);

            writer.Write(person2.Name);
            writer.Write(person2.Age);
        }
    }

    static void BinaryReadFile(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            var reader = new BinaryReader(fileStream);
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string name = reader.ReadString();
                int age = reader.ReadInt32();

                Console.WriteLine("Name: " + name);
                Console.WriteLine("Age: " + age);
                Console.WriteLine();
            }
        }
    }

    static void WriteJsonFile(string filePath)
    {
        Person person1 = new Person { Name = "A", Age = 30 };
        Person person2 = new Person { Name = "B", Age = 25 };

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(new[] { person1, person2 }, options);
        File.WriteAllText(filePath, json);
    }

    static void ReadJsonFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        Person[] people = JsonSerializer.Deserialize<Person[]>(json);

        foreach (Person person in people)
        {
            Console.WriteLine("Name: " + person.Name);
            Console.WriteLine("Age: " + person.Age);
        }
    }

    /*static void WriteProtoBuf(string filePath)
    {
        filePath = "people.dat";
        var person1 = new Person { Name = "C", Age = 30 };
        var person2 = new Person { Name = "D", Age = 25 };

        using (var output = File.Create(filePath))
        {
            Serializer.Serialize(output, person1);
            Serializer.Serialize(output, person2);
        }
    }

    static void ReadProtoBuf(string filePath)
    {
        filePath = "people.dat";
        using (var input = File.OpenRead(filePath))
        {
            Person[] people = new Person[2];
            for (int i = 0; i < 2; i++)
            {
                people[i] = Serializer.Deserialize<Person>(input);
                Console.WriteLine("Name: " + people[i].Name);
                Console.WriteLine("Age: " + people[i].Age);
            }
        }
    }*/
}